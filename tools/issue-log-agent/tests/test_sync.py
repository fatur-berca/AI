from pathlib import Path

import pytest

from issue_log_agent.claude_assist import TitleSuggestion
from issue_log_agent.config import load_config
from issue_log_agent.formatter import format_section
from issue_log_agent.sheet import CsvSpreadsheet, col_letter
from issue_log_agent.sync import Syncer, status_from_issue

ROOT = Path(__file__).resolve().parents[1]


class FakeWorksheet:
    def __init__(self, values):
        self.values = values
        self.updates = []

    def get_all_values(self):
        return self.values

    def batch_update(self, data, **kw):
        self.updates.extend(data)

    def cells(self):
        """A1 -> value, using header names for readability: {(row, header): value}."""
        headers = self.values[0]
        out = {}
        for u in self.updates:
            col = "".join(c for c in u["range"] if c.isalpha())
            row = int("".join(c for c in u["range"] if c.isdigit()))
            idx = next(i for i in range(len(headers)) if col_letter(i) == col)
            out[(row, headers[idx])] = u["values"][0][0]
        return out


class FakeBook:
    def __init__(self):
        csv_book = CsvSpreadsheet(ROOT / "examples")
        self.sheets = {}
        for tab in ("FEATURE", "BUG", "SECURITY", "PERFORMANCE"):
            self.sheets[tab] = FakeWorksheet(csv_book.worksheet(tab).get_all_values())

    def worksheet(self, name):
        return self.sheets[name]


class FakeGitLab:
    def __init__(self):
        self.created = []
        self.updates = []
        self.existing = {}
        self.issues = {12: {"iid": 12, "state": "opened", "labels": ["backlog", "status::Ready to Deploy"]}}

    def find_issue_by_marker(self, marker):
        return self.existing.get(marker)

    def create_issue(self, payload, confidential=False):
        self.created.append(payload)
        iid = 100 + len(self.created)
        return {"iid": iid, "web_url": f"https://gitlab.example/p/-/issues/{iid}"}

    def get_issue(self, iid):
        return self.issues[iid]

    def update_issue(self, iid, **kw):
        self.updates.append((iid, kw))
        return {"iid": iid, "web_url": f"https://gitlab.example/p/-/issues/{iid}"}


class FakeClaude:
    def __init__(self):
        self.calls = 0

    def suggest(self, row, itype, need_source):
        self.calls += 1
        return TitleSuggestion(title="Judul dari Claude", source="BRD" if need_source else "")


@pytest.fixture
def cfg(monkeypatch):
    monkeypatch.setenv("GITLAB_TOKEN", "test-token")
    return load_config(ROOT / "config.example.yaml")


def test_creates_issues_with_guideline_title_and_writes_back(cfg):
    book, gl = FakeBook(), FakeGitLab()
    report = Syncer(cfg, gl).run(book)

    assert [p.title for p in gl.created] == [
        "[FEATURE][FSD]-Upload dokumen pada proses pengajuan",
        "[FEATURE][FSD]-Penambahan column salary pada list debitur multiguna",
        "[BUG][SIT]-Error HTTP 500 saat submit pengajuan dengan data valid",
        "[SECURITY][EXT]-Document API belum melakukan authorization check",
        "[PERFORMANCE][INT]-Response time API Submit Pengajuan tinggi saat 50 concurrent user",
    ]
    assert set(report.invalid) == {"FEATURE!4", "FEATURE!5"}

    feat = book.sheets["FEATURE"].cells()
    assert feat[(2, "GitLab Issue")] == "#101"
    assert feat[(2, "Sync Status")] == "CREATED"
    # source inferred from Reference ("FSD: ...") is written back for review
    assert feat[(3, "Source")] == "FSD"
    assert "Source diambil dari isi issue" in feat[(3, "Sync Message")]
    # empty status defaults to Open
    assert feat[(3, "Issue Board Status")] == "Open"
    assert feat[(4, "Sync Status")] == "INVALID"
    assert "Title wajib diisi" in feat[(4, "Sync Message")]
    assert "tidak sesuai tab FEATURE" in feat[(5, "Sync Message")]


def test_body_follows_slide_structure(cfg):
    gl = FakeGitLab()
    Syncer(cfg, gl).run(FakeBook(), [t for t in cfg.types if t.name == "FEATURE"])
    body = gl.created[0].description
    headings = [l for l in body.splitlines() if l.startswith("## ")]
    assert headings == [
        "## Background", "## Description", "## Scope", "## Business / Functional Requirement",
        "## Acceptance Criteria", "## Technical Notes", "## Out of Scope", "## Reference",
    ]
    assert "- [ ] File > 10 MB ditolak" in body
    assert "- Validasi format file" in body
    assert gl.created[0].labels == ["backlog", "feature", "source::FSD"]


def test_status_label_on_create(cfg):
    gl = FakeGitLab()
    Syncer(cfg, gl).run(FakeBook(), [t for t in cfg.types if t.name == "SECURITY"])
    assert "status::On-Progress" in gl.created[0].labels


def test_dedup_via_marker(cfg):
    gl = FakeGitLab()
    book = FakeBook()
    types = [t for t in cfg.types if t.name == "BUG"]
    # simulate an earlier run that created the issue but crashed before writing the sheet
    first = FakeGitLab()
    Syncer(cfg, first).run(FakeBook(), types)
    gl.existing[first.created[0].marker] = {"iid": 7, "web_url": "https://gitlab.example/p/-/issues/7"}
    Syncer(cfg, gl).run(book, types)
    assert gl.created == []
    assert book.sheets["BUG"].cells()[(2, "GitLab Issue")] == "#7"


def test_status_pull_from_gitlab(cfg):
    book, gl = FakeBook(), FakeGitLab()
    Syncer(cfg, gl, status_sync="pull").run(book, [t for t in cfg.types if t.name == "PERFORMANCE"])
    assert book.sheets["PERFORMANCE"].cells()[(3, "Issue Board Status")] == "Ready to Deploy"


def test_status_push_to_gitlab(cfg):
    gl = FakeGitLab()
    Syncer(cfg, gl, status_sync="push").run(FakeBook(), [t for t in cfg.types if t.name == "PERFORMANCE"])
    iid, kw = gl.updates[-1]
    assert iid == 12
    assert kw["add_labels"] == ["status::Testing"]
    assert "status::Ready to Deploy" in kw["remove_labels"]


def test_status_from_issue(cfg):
    assert status_from_issue(cfg, {"state": "closed", "labels": []}) == "Closed"
    assert status_from_issue(cfg, {"state": "opened", "labels": ["status::Re-work"]}) == "Re-work"
    assert status_from_issue(cfg, {"state": "opened", "labels": []}) == "Open"


def test_claude_fills_missing_title(cfg):
    book, gl, claude = FakeBook(), FakeGitLab(), FakeClaude()
    report = Syncer(cfg, gl, assistant=claude).run(book, [t for t in cfg.types if t.name == "FEATURE"])
    # row 4 has no title: Claude drafts one, but the row is still invalid (missing required sections)
    cells = book.sheets["FEATURE"].cells()
    assert cells[(4, "Title")] == "Judul dari Claude"
    assert "FEATURE!4" in report.invalid
    # rows that already have a title+source never call Claude
    assert claude.calls == 1


def test_dry_run_does_not_touch_anything(cfg):
    book = FakeBook()
    report = Syncer(cfg, None, dry_run=True).run(book)
    assert len(report.created) == 5
    assert all(ws.updates == [] for ws in book.sheets.values())


@pytest.mark.parametrize(
    "value,style,expected",
    [
        ("a\nb", "checklist", "- [ ] a\n- [ ] b"),
        ("- [x] done\n- [ ] todo", "checklist", "- [x] done\n- [ ] todo"),
        ("1. a\n2) b\n\n• c", "numbered", "1. a\n2. b\n3. c"),
        ("* a\n- b", "bullets", "- a\n- b"),
        ("line 1\nline 2", "text", "line 1\nline 2"),
    ],
)
def test_format_section(value, style, expected):
    assert format_section(value, style) == expected


def test_inferred_source_written_back_even_with_claude(cfg):
    book, gl, claude = FakeBook(), FakeGitLab(), FakeClaude()
    Syncer(cfg, gl, assistant=claude).run(book, [t for t in cfg.types if t.name == "FEATURE"])
    cells = book.sheets["FEATURE"].cells()
    assert cells[(3, "Source")] == "FSD"


def test_claude_api_error_does_not_stop_sync(cfg, monkeypatch):
    pytest.importorskip("anthropic")
    from issue_log_agent.claude_assist import ClaudeAssistant

    monkeypatch.setenv("ANTHROPIC_API_KEY", "test")
    monkeypatch.setenv("ANTHROPIC_BASE_URL", "http://127.0.0.1:9")
    assistant = ClaudeAssistant(cfg.claude)
    assistant.client = assistant.client.with_options(max_retries=0)
    book, gl = FakeBook(), FakeGitLab()
    report = Syncer(cfg, gl, assistant=assistant).run(book, [t for t in cfg.types if t.name == "FEATURE"])
    assert "FEATURE!4" in report.invalid  # still processed, just without a Claude title
    assert len(gl.created) == 2
