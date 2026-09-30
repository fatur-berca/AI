"""Sync flow: Spreadsheet -> validate -> GitLab issue -> write result back to the sheet."""

from __future__ import annotations

import logging
from dataclasses import dataclass, field
from datetime import datetime
from zoneinfo import ZoneInfo

from .config import Config
from .formatter import Row, build_issue, validate
from .gitlab_client import GitLabClient, GitLabError
from .sheet import SheetIO

log = logging.getLogger(__name__)

STATUS_CREATED = "CREATED"
STATUS_UPDATED = "UPDATED"
STATUS_INVALID = "INVALID"
STATUS_ERROR = "ERROR"


@dataclass
class Report:
    created: list[int] = field(default_factory=list)
    updated: list[int] = field(default_factory=list)
    skipped: list[int] = field(default_factory=list)
    invalid: dict[int, list[str]] = field(default_factory=dict)
    failed: dict[int, str] = field(default_factory=dict)

    def summary(self) -> str:
        return (
            f"created={len(self.created)} updated={len(self.updated)} skipped={len(self.skipped)} "
            f"invalid={len(self.invalid)} failed={len(self.failed)}"
        )


def marker_for(row: Row, cfg: Config) -> str:
    key = row.get("key") or f"row-{row.number}"
    return f"<!-- issue-log:{cfg.sheet.spreadsheet_id}:{cfg.sheet.worksheet}:{key} -->"


def _is_triggered(row: Row, cfg: Config) -> bool:
    if not cfg.trigger_field:
        return True
    return row.get(cfg.trigger_field).lower() in {v.lower() for v in cfg.trigger_values}


def run_sync(
    sheet: SheetIO,
    gitlab: GitLabClient | None,
    cfg: Config,
    *,
    dry_run: bool = False,
    update_existing: bool = False,
    only_rows: set[int] | None = None,
) -> Report:
    report = Report()
    now = datetime.now(ZoneInfo(cfg.timezone)).strftime("%Y-%m-%d %H:%M:%S")
    sheet_url = f"https://docs.google.com/spreadsheets/d/{cfg.sheet.spreadsheet_id}"

    if sheet.missing:
        log.warning("Kolom tidak ditemukan di sheet (diabaikan): %s", ", ".join(sheet.missing))

    for row in sheet.rows():
        if only_rows and row.number not in only_rows:
            continue
        if not _is_triggered(row, cfg):
            report.skipped.append(row.number)
            continue

        existing_iid = row.writeback.get("issue_iid", "").lstrip("#")
        if existing_iid and not update_existing:
            report.skipped.append(row.number)
            continue

        errors = validate(row, cfg)
        if errors:
            report.invalid[row.number] = errors
            log.warning("Baris %d tidak valid: %s", row.number, "; ".join(errors))
            if not dry_run:
                sheet.set(row, {"sync_status": STATUS_INVALID, "sync_message": "; ".join(errors), "synced_at": now})
            continue

        payload = build_issue(row, cfg, sheet_url=f"{sheet_url}#gid=0&range=A{row.number}")
        marker = marker_for(row, cfg)
        payload.description = f"{payload.description.rstrip()}\n\n{marker}\n"

        if dry_run or gitlab is None:
            action = "UPDATE" if existing_iid else "CREATE"
            log.info("[dry-run] %s baris %d: %s | labels=%s", action, row.number, payload.title, payload.labels)
            (report.updated if existing_iid else report.created).append(row.number)
            continue

        try:
            if existing_iid:
                state = cfg.state_map.get(row.get("status"))
                state_event = {"close": "close", "reopen": "reopen"}.get((state or "").lower())
                issue = gitlab.update_issue(int(existing_iid), payload, state_event)
                report.updated.append(row.number)
                status = STATUS_UPDATED
            else:
                issue = gitlab.find_issue_by_marker(marker)
                if issue:
                    log.info("Baris %d sudah punya issue #%s (ditemukan via marker)", row.number, issue["iid"])
                else:
                    issue = gitlab.create_issue(payload, confidential=cfg.gitlab.confidential)
                report.created.append(row.number)
                status = STATUS_CREATED
            sheet.set(
                row,
                {
                    "issue_iid": f"#{issue['iid']}",
                    "issue_url": issue.get("web_url", ""),
                    "sync_status": status,
                    "sync_message": "",
                    "synced_at": now,
                },
            )
            log.info("Baris %d -> #%s %s", row.number, issue["iid"], issue.get("web_url", ""))
        except (GitLabError, OSError) as e:
            report.failed[row.number] = str(e)
            log.error("Baris %d gagal: %s", row.number, e)
            sheet.set(row, {"sync_status": STATUS_ERROR, "sync_message": str(e)[:500], "synced_at": now})
        finally:
            # Flush per row so a crash mid-run never loses the link to an already-created issue.
            sheet.flush()

    sheet.flush()
    return report
