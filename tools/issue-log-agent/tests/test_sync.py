import csv
from pathlib import Path

import pytest

from issue_log_agent.config import load_config
from issue_log_agent.formatter import build_issue
from issue_log_agent.sheet import SheetIO
from issue_log_agent.sync import run_sync

ROOT = Path(__file__).resolve().parents[1]


class FakeWorksheet:
    def __init__(self, values):
        self.values = values
        self.updates = []

    def get_all_values(self):
        return self.values

    def batch_update(self, data, **kw):
        self.updates.extend(data)


class FakeGitLab:
    def __init__(self, existing=None):
        self.created = []
        self.updated = []
        self.existing = existing or {}

    def find_issue_by_marker(self, marker):
        return self.existing.get(marker)

    def create_issue(self, payload, confidential=False):
        self.created.append(payload)
        iid = 100 + len(self.created)
        return {"iid": iid, "web_url": f"https://gitlab.example/p/-/issues/{iid}"}

    def update_issue(self, iid, payload, state_event=None):
        self.updated.append((iid, payload, state_event))
        return {"iid": iid, "web_url": f"https://gitlab.example/p/-/issues/{iid}"}


@pytest.fixture
def cfg(monkeypatch):
    monkeypatch.setenv("GITLAB_TOKEN", "test-token")
    return load_config(ROOT / "config.example.yaml")


@pytest.fixture
def values():
    with open(ROOT / "examples" / "sample_issue_log.csv", newline="", encoding="utf-8") as f:
        return [list(r) for r in csv.reader(f)]


def cell_updates(ws):
    return {u["range"]: u["values"][0][0] for u in ws.updates}


def test_creates_valid_rows_and_writes_back(cfg, values):
    ws = FakeWorksheet(values)
    gl = FakeGitLab()
    report = run_sync(SheetIO(ws, cfg), gl, cfg)

    assert report.created == [2, 3]
    assert list(report.invalid) == [4]
    assert report.skipped == [5, 6]  # 5 already has an issue, 6 is Draft
    assert [p.title for p in gl.created] == [
        "[Transport Order] Tombol Save tidak merespon saat input order",
        "[Master Data] Tambah filter lokasi pada list master kilometer",
    ]
    cells = cell_updates(ws)
    # writeback columns: GitLab Issue=R, GitLab URL=S, Sync Status=T, Sync Message=U
    assert cells["R2"] == "#101"
    assert cells["S2"].endswith("/issues/101")
    assert cells["T2"] == "CREATED"
    assert cells["T4"] == "INVALID"
    assert "Severity" in cells["U4"]


def test_issue_format(cfg, values):
    sheet = SheetIO(FakeWorksheet(values), cfg)
    rows = sheet.rows()
    p = build_issue(rows[0], cfg)
    assert p.labels == ["issue-log", "type::bug", "severity::Major", "priority::High", "module::Transport Order"]
    assert p.assignee_username == "budi.santoso"
    assert p.due_date == "2026-09-15"
    assert "## Langkah Reproduksi\n1. Buka menu Transport Order" in p.description

    p2 = build_issue(rows[1], cfg)
    assert p2.assignee_username == "citra"
    # empty sections are dropped
    assert "## Expected Result" not in p2.description
    assert "## Deskripsi" in p2.description


def test_dedup_via_marker(cfg, values):
    gl = FakeGitLab()
    marker = "<!-- issue-log:%s:%s:IL-001 -->" % (cfg.sheet.spreadsheet_id, cfg.sheet.worksheet)
    gl.existing[marker] = {"iid": 7, "web_url": "https://gitlab.example/p/-/issues/7"}
    ws = FakeWorksheet(values)
    run_sync(SheetIO(ws, cfg), gl, cfg, only_rows={2})
    assert gl.created == []
    assert cell_updates(ws)["R2"] == "#7"


def test_update_existing_and_close(cfg, values):
    values[4][15] = "Closed"  # row 5 status
    cfg.trigger_values.append("Closed")
    gl = FakeGitLab()
    run_sync(SheetIO(FakeWorksheet(values), cfg), gl, cfg, update_existing=True, only_rows={5})
    assert [(i, s) for i, _, s in gl.updated] == [(12, "close")]


def test_dry_run_does_not_touch_anything(cfg, values):
    ws = FakeWorksheet(values)
    report = run_sync(SheetIO(ws, cfg), None, cfg, dry_run=True)
    assert report.created == [2, 3]
    assert ws.updates == []
