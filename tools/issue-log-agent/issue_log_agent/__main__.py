"""CLI entry point: python -m issue_log_agent --config config.yaml [--dry-run]"""

from __future__ import annotations

import argparse
import logging
import sys
import time

from .config import ConfigError, load_config
from .gitlab_client import GitLabClient
from .sheet import CsvSpreadsheet, GoogleSpreadsheet
from .sync import Syncer


def main(argv: list[str] | None = None) -> int:
    p = argparse.ArgumentParser(description="Sync Template Issue Backlog (Google Sheet) ke GitLab Issue Board")
    p.add_argument("--config", default="config.yaml")
    p.add_argument("--dry-run", action="store_true", help="Validasi & preview tanpa membuat issue / menulis sheet")
    p.add_argument("--csv-dir", help="Baca dari folder berisi <TAB>.csv (hasil export), otomatis --dry-run")
    p.add_argument("--type", action="append", help="Hanya proses tipe tertentu, mis. --type BUG (bisa diulang)")
    p.add_argument("--row", type=int, action="append", help="Hanya proses nomor baris tertentu (bisa diulang)")
    p.add_argument("--status-sync", choices=["pull", "push", "none"], default="pull",
                   help="Baris yang sudah punya issue: pull = status GitLab -> sheet (default), "
                        "push = status sheet -> GitLab, none = lewati")
    p.add_argument("--no-claude", action="store_true", help="Matikan saran judul/source dari Claude")
    p.add_argument("--watch", type=int, metavar="SECONDS", help="Jalan terus, polling tiap N detik")
    p.add_argument("-v", "--verbose", action="store_true")
    args = p.parse_args(argv)

    logging.basicConfig(
        level=logging.DEBUG if args.verbose else logging.INFO,
        format="%(asctime)s %(levelname)s %(message)s",
    )
    if args.csv_dir:
        args.dry_run = True
    try:
        cfg = load_config(args.config, require_token=not args.dry_run)
    except (ConfigError, FileNotFoundError) as e:
        logging.error("Config error: %s", e)
        return 2

    types = cfg.types
    if args.type:
        wanted = {t.upper() for t in args.type}
        types = [t for t in cfg.types if t.name in wanted]
        if not types:
            logging.error("Tipe %s tidak ada di config", ", ".join(sorted(wanted)))
            return 2

    gitlab = None if args.dry_run else GitLabClient(
        cfg.gitlab.url, cfg.gitlab.project, cfg.gitlab.token, verify_ssl=cfg.gitlab.verify_ssl
    )
    assistant = None
    if cfg.claude.enabled and not args.no_claude:
        from .claude_assist import ClaudeAssistant

        # the prompt limit excludes the "[TYPE][SOURCE]-" prefix
        assistant = ClaudeAssistant(cfg.claude, cfg.max_title_length - len("[PERFORMANCE][EXT]-"))

    while True:
        book = CsvSpreadsheet(args.csv_dir) if args.csv_dir else GoogleSpreadsheet(cfg)
        report = Syncer(
            cfg, gitlab,
            assistant=assistant,
            dry_run=args.dry_run,
            status_sync=args.status_sync,
            only_rows=set(args.row) if args.row else None,
        ).run(book, types)
        logging.info("Selesai: %s", report.summary())
        if not args.watch:
            return 1 if report.failed else 0
        time.sleep(args.watch)


if __name__ == "__main__":
    sys.exit(main())
