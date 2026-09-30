"""CLI entry point: python -m issue_log_agent --config config.yaml [--dry-run]"""

from __future__ import annotations

import argparse
import logging
import sys
import time

from .config import ConfigError, load_config
from .gitlab_client import GitLabClient
from .sheet import CsvWorksheet, SheetIO, open_worksheet
from .sync import run_sync


def main(argv: list[str] | None = None) -> int:
    p = argparse.ArgumentParser(description="Sync Issue Log dari Google Spreadsheet ke GitLab Issue Board")
    p.add_argument("--config", default="config.yaml")
    p.add_argument("--dry-run", action="store_true", help="Validasi & tampilkan preview tanpa membuat issue")
    p.add_argument("--update-existing", action="store_true", help="Update issue yang sudah pernah dibuat")
    p.add_argument("--row", type=int, action="append", help="Hanya proses nomor baris tertentu (bisa diulang)")
    p.add_argument("--watch", type=int, metavar="SECONDS", help="Jalan terus, polling tiap N detik")
    p.add_argument("--csv", help="Baca dari file CSV lokal (hasil export sheet), otomatis --dry-run")
    p.add_argument("-v", "--verbose", action="store_true")
    args = p.parse_args(argv)

    logging.basicConfig(
        level=logging.DEBUG if args.verbose else logging.INFO,
        format="%(asctime)s %(levelname)s %(message)s",
    )
    if args.csv:
        args.dry_run = True
    try:
        cfg = load_config(args.config, require_token=not args.dry_run)
    except (ConfigError, FileNotFoundError) as e:
        logging.error("Config error: %s", e)
        return 2

    gitlab = None if args.dry_run else GitLabClient(
        cfg.gitlab.url, cfg.gitlab.project, cfg.gitlab.token, verify_ssl=cfg.gitlab.verify_ssl
    )

    while True:
        sheet = SheetIO(CsvWorksheet(args.csv) if args.csv else open_worksheet(cfg), cfg)
        report = run_sync(
            sheet, gitlab, cfg,
            dry_run=args.dry_run,
            update_existing=args.update_existing,
            only_rows=set(args.row) if args.row else None,
        )
        logging.info("Selesai: %s", report.summary())
        if not args.watch:
            return 1 if report.failed else 0
        time.sleep(args.watch)


if __name__ == "__main__":
    sys.exit(main())
