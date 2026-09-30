"""Alur Pelaporan Backlog (slide 15).

    Template Issue Backlog (1 tab per tipe) -> agent -> GitLab Issue Board
      - baris baru      : lengkapi judul/source -> validasi standar -> buat issue -> tulis balik link
      - baris tidak valid: tandai INVALID + alasan di sheet, reporter merevisi manual
      - baris yang sudah punya issue: sinkron Issue Board Status (default: GitLab -> sheet)
"""

from __future__ import annotations

import logging
from dataclasses import dataclass, field, replace
from datetime import datetime
from zoneinfo import ZoneInfo

from .config import Config, IssueType
from .formatter import build_issue, content_marker, full_title, resolve_title, validate
from .gitlab_client import GitLabClient, GitLabError
from .sheet import Row, SheetIO, SpreadsheetSource

log = logging.getLogger(__name__)

STATUS_CREATED = "CREATED"
STATUS_INVALID = "INVALID"
STATUS_ERROR = "ERROR"


@dataclass
class Report:
    created: list[str] = field(default_factory=list)
    existing: list[str] = field(default_factory=list)  # read-only mode: issue already in GitLab
    status_synced: list[str] = field(default_factory=list)
    skipped: list[str] = field(default_factory=list)
    invalid: dict[str, list[str]] = field(default_factory=dict)
    failed: dict[str, str] = field(default_factory=dict)

    def summary(self) -> str:
        return (
            f"created={len(self.created)} existing={len(self.existing)} status_synced={len(self.status_synced)} skipped={len(self.skipped)} "
            f"invalid={len(self.invalid)} failed={len(self.failed)}"
        )


def status_from_issue(cfg: Config, issue: dict) -> str:
    """Map a GitLab issue (state + labels) back to an Issue Board Status value."""
    if issue.get("state") == "closed":
        for name, rule in cfg.statuses.items():
            if rule.state == "close":
                return name
    labels = set(issue.get("labels") or [])
    for name, rule in cfg.statuses.items():
        if rule.labels and set(rule.labels) <= labels:
            return name
    return cfg.default_status


class Syncer:
    def __init__(
        self,
        cfg: Config,
        gitlab: GitLabClient | None,
        *,
        assistant=None,
        dry_run: bool = False,
        status_sync: str = "pull",  # pull (GitLab -> sheet) | push (sheet -> GitLab) | none
        only_rows: set[int] | None = None,
        read_only: bool = False,  # sheet cannot be written (public access mode)
    ):
        self.cfg = cfg
        self.gitlab = gitlab
        self.assistant = assistant
        self.dry_run = dry_run or gitlab is None
        self.read_only = read_only
        self.write_sheet = not self.dry_run and not read_only
        self.status_sync = status_sync
        self.only_rows = only_rows
        self.now = datetime.now(ZoneInfo(cfg.timezone)).strftime("%Y-%m-%d %H:%M:%S")
        self.report = Report()

    def run(self, book: SpreadsheetSource, types: list[IssueType] | None = None) -> Report:
        for itype in types or self.cfg.types:
            try:
                sheet = SheetIO(book.worksheet(itype.worksheet), self.cfg, itype)
            except Exception as e:  # missing tab should not stop the other tabs
                log.error("Tab '%s' tidak bisa dibuka: %s", itype.worksheet, e)
                self.report.failed[itype.worksheet] = str(e)
                continue
            if sheet.missing:
                log.warning("[%s] kolom tidak ditemukan (diabaikan): %s", itype.worksheet, ", ".join(sheet.missing))
            for row in sheet.rows():
                if self.only_rows and row.number not in self.only_rows:
                    continue
                try:
                    self._process(sheet, itype, row)
                finally:
                    # Flush per row so a crash never loses the link to an already-created issue.
                    if self.write_sheet:
                        sheet.flush()
        return self.report

    def _write(self, sheet: SheetIO, row: Row, updates: dict[str, str]) -> None:
        if self.write_sheet:
            sheet.set(row, updates)

    def _find_existing(self, markers: list[str]) -> dict | None:
        for m in markers:
            issue = self.gitlab.find_issue_by_marker(m)
            if issue:
                return issue
        return None

    def _handle_existing(self, sheet: SheetIO, row: Row, ref: str, issue: dict, fill: dict[str, str]) -> None:
        log.info("[%s] issue #%s sudah ada (marker), tidak dibuat ulang", ref, issue["iid"])
        if self.read_only:
            self.report.existing.append(ref)
            if self.status_sync == "push":
                self._sync_status(sheet, row, ref, int(issue["iid"]))
            return
        self.report.created.append(ref)
        self._write(sheet, row, {
            **fill,
            "issue_iid": f"#{issue['iid']}",
            "issue_url": issue.get("web_url", ""),
            "sync_status": STATUS_CREATED,
            "synced_at": self.now,
        })

    def _only_title_missing(self, row: Row, itype: IssueType, rt) -> bool:
        """Ask Claude only when Title/Source is the sole problem; other errors need the reporter anyway."""
        probe = replace(rt, title=rt.title or "x", source=rt.source or itype.sources[0])
        return not validate(row, self.cfg, itype, probe)

    def _process(self, sheet: SheetIO, itype: IssueType, row: Row) -> None:
        ref = f"{itype.worksheet}!{row.number}"
        status = row.get("status")
        if status and status.lower() in {s.lower() for s in self.cfg.skip_statuses}:
            self.report.skipped.append(ref)
            return

        iid = row.writeback.get("issue_iid", "").lstrip("#")
        if iid:
            # A read-only sheet cannot receive pulled statuses; only push makes sense there.
            if self.status_sync == "none" or not iid.isdigit() or (self.read_only and self.status_sync == "pull"):
                self.report.skipped.append(ref)
            else:
                self._sync_status(sheet, row, ref, int(iid))
            return

        try:
            # Without write-back every run sees every row as new: check GitLab first, before
            # spending a Claude call on a row that was already sent.
            if self.read_only and not self.dry_run:
                issue = self._find_existing([content_marker(self.cfg, itype, row)])
                if issue:
                    self._handle_existing(sheet, row, ref, issue, {})
                    return

            rt = resolve_title(row, itype)
            notes: list[str] = []
            fill: dict[str, str] = {}
            if rt.source and not rt.source_from_cell:
                fill["source"] = rt.source
                notes.append("Source diambil dari isi issue")
            if self.assistant and (not rt.title or not rt.source) and self._only_title_missing(row, itype, rt):
                prefix_len = len(full_title(self.cfg, itype, "", max(itype.sources, key=len)))
                s = self.assistant.suggest(row, itype, need_source=not rt.source)
                if s and not rt.title and s.title:
                    rt.title = s.title[: self.cfg.max_title_length - prefix_len]
                    fill["title"] = rt.title
                    notes.append("Title dibuat oleh Claude")
                if s and not rt.source and s.source:
                    rt.source = s.source
                    fill["source"] = rt.source
                    notes.append("Source ditebak oleh Claude")

            errors = validate(row, self.cfg, itype, rt)
            if errors:
                self.report.invalid[ref] = errors
                log.warning("[%s] tidak valid: %s", ref, "; ".join(errors))
                self._write(sheet, row, {**fill, "sync_status": STATUS_INVALID, "sync_message": "; ".join(errors),
                                         "synced_at": self.now})
                return

            payload = build_issue(row, self.cfg, itype, rt)
            if self.dry_run:
                log.info("[dry-run] CREATE %s: %s | labels=%s%s", ref, payload.title, payload.labels,
                         f" | {'; '.join(notes)}" if notes else "")
                log.debug("[dry-run] body %s:\n%s", ref, payload.description)
                self.report.created.append(ref)
                return

            issue = self._find_existing(payload.markers)
            if issue:
                self._handle_existing(sheet, row, ref, issue, fill)
                return
            issue = self.gitlab.create_issue(payload, confidential=self.cfg.gitlab.confidential)
            rule = self.cfg.status_rule(status)
            if rule and rule.state == "close":
                issue = self.gitlab.update_issue(issue["iid"], state_event="close")
            self.report.created.append(ref)
            self._write(sheet, row, {
                **fill,
                "status": status or self.cfg.default_status,
                "issue_iid": f"#{issue['iid']}",
                "issue_url": issue.get("web_url", ""),
                "sync_status": STATUS_CREATED,
                "sync_message": ("; ".join(notes) + ", mohon dicek") if notes else "",
                "synced_at": self.now,
            })
            log.info("[%s] -> #%s %s", ref, issue["iid"], issue.get("web_url", ""))
        except (GitLabError, OSError) as e:
            self.report.failed[ref] = str(e)
            log.error("[%s] gagal: %s", ref, e)
            self._write(sheet, row, {"sync_status": STATUS_ERROR, "sync_message": str(e)[:500], "synced_at": self.now})

    def _sync_status(self, sheet: SheetIO, row: Row, ref: str, iid: int) -> None:
        if self.dry_run:
            log.info("[dry-run] %s status sync (%s) untuk #%s", ref, self.status_sync, iid)
            self.report.status_synced.append(ref)
            return
        try:
            if self.status_sync == "pull":
                current = status_from_issue(self.cfg, self.gitlab.get_issue(iid))
                if current != row.get("status"):
                    self._write(sheet, row, {"status": current, "synced_at": self.now})
                    log.info("[%s] status #%s: %s -> %s", ref, iid, row.get("status") or "-", current)
            else:  # push
                rule = self.cfg.status_rule(row.get("status"))
                if rule is None:
                    raise GitLabError(f"Issue Board Status '{row.get('status')}' tidak dikenal")
                self.gitlab.update_issue(
                    iid,
                    add_labels=rule.labels,
                    remove_labels=sorted(self.cfg.all_status_labels - set(rule.labels)),
                    state_event=rule.state or "reopen",
                )
                self._write(sheet, row, {"synced_at": self.now})
            self.report.status_synced.append(ref)
        except (GitLabError, OSError) as e:
            self.report.failed[ref] = str(e)
            log.error("[%s] sync status gagal: %s", ref, e)
