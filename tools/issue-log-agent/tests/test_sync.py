from pathlib import Path

import pytest

from issue_log_agent.claude_assist import TitleSuggestion
from issue_log_agent.config import load_config
from issue_log_agent.formatter import format_section
from issue_log_agent.sheet import CsvSpreadsheet, SheetIO, col_letter
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
        issue = {"iid": iid, "web_url": f"https://gitlab.example/p/-/issues/{iid}"}
        for m in payload.markers:
            self.existing[m] = issue
        return issue

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
    gl.existing[first.created[0].markers[0]] = {"iid": 7, "web_url": "https://gitlab.example/p/-/issues/7"}
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
    feature = book.sheets["FEATURE"]
    headers = feature.values[0]
    row4 = feature.values[3]  # has Background only, no Title
    row4[headers.index("Description")] = "Deskripsi"
    row4[headers.index("Acceptance Criteria")] = "Bisa dipakai"
    report = Syncer(cfg, gl, assistant=claude).run(book, [t for t in cfg.types if t.name == "FEATURE"])
    cells = feature.cells()
    assert cells[(4, "Title")] == "Judul dari Claude"
    assert "Title dibuat oleh Claude" in cells[(4, "Sync Message")]
    assert "FEATURE!4" in report.created
    assert "[FEATURE][BRD]-Judul dari Claude" in [p.title for p in gl.created]
    # rows that already have a title+source, or are invalid for other reasons, never call Claude
    assert claude.calls == 1


def test_claude_not_called_when_row_is_invalid_anyway(cfg):
    claude = FakeClaude()
    report = Syncer(cfg, FakeGitLab(), assistant=claude).run(FakeBook(), [t for t in cfg.types if t.name == "FEATURE"])
    assert "FEATURE!4" in report.invalid  # missing Description and Acceptance Criteria
    assert claude.calls == 0


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


def test_unset_env_var_in_config_is_treated_as_empty(monkeypatch):
    monkeypatch.delenv("GOOGLE_APPLICATION_CREDENTIALS", raising=False)
    monkeypatch.delenv("GITLAB_TOKEN", raising=False)
    cfg = load_config(ROOT / "config.example.yaml", require_token=False)
    assert cfg.sheet.credentials_file is None
    assert cfg.gitlab.token == ""
    with pytest.raises(Exception, match="GITLAB_TOKEN"):
        load_config(ROOT / "config.example.yaml")


# ---- sheet.access: public (read-only, no Google credentials) ----


def _book_with_untitled_valid_row():
    book = FakeBook()
    feature = book.sheets["FEATURE"]
    headers = feature.values[0]
    feature.values[3][headers.index("Description")] = "Deskripsi"
    feature.values[3][headers.index("Acceptance Criteria")] = "Bisa dipakai"
    return book


def test_public_mode_never_writes_and_does_not_duplicate(cfg):
    gl, claude = FakeGitLab(), FakeClaude()
    book = _book_with_untitled_valid_row()
    first = Syncer(cfg, gl, assistant=claude, read_only=True).run(book)
    assert len(first.created) == 6
    assert all(ws.updates == [] for ws in book.sheets.values())
    assert claude.calls == 1  # the untitled row

    # nothing was written back, so the next run sees every row as new again
    second = Syncer(cfg, gl, assistant=claude, read_only=True).run(_book_with_untitled_valid_row())
    assert second.created == []
    assert len(second.existing) == 6
    assert len(gl.created) == 6
    # rows already in GitLab are recognised before Claude is asked again
    assert claude.calls == 1


def test_public_mode_edited_row_is_still_recognised_by_title(cfg):
    gl = FakeGitLab()
    types = [t for t in cfg.types if t.name == "BUG"]
    Syncer(cfg, gl, read_only=True).run(FakeBook(), types)
    book = FakeBook()
    book.sheets["BUG"].values[1][4] = "Environment: SIT\nBrowser: Firefox"  # reporter edits the row later
    report = Syncer(cfg, gl, read_only=True).run(book, types)
    assert report.existing == ["BUG!2"]
    assert len(gl.created) == 1


def test_public_mode_push_status_to_existing_issue(cfg):
    gl = FakeGitLab()
    types = [t for t in cfg.types if t.name == "SECURITY"]
    Syncer(cfg, gl, read_only=True).run(FakeBook(), types)
    Syncer(cfg, gl, read_only=True, status_sync="push").run(FakeBook(), types)
    iid, kw = gl.updates[-1]
    assert iid == 101 and kw["add_labels"] == ["status::On-Progress"]


class _Resp:
    def __init__(self, status, content):
        self.status_code, self.content = status, content


def test_public_spreadsheet_download(cfg, monkeypatch, tmp_path):
    openpyxl = pytest.importorskip("openpyxl")
    import requests

    from issue_log_agent.sheet import PublicSpreadsheet

    wb = openpyxl.load_workbook(ROOT / "templates" / "Template_Backlog_v2.xlsx")
    wb["BUG"].append(["Judul", "SIT", "bg", None, "env", "1. a", "exp", "act", None, "ac", None, None, "Open", None, None, None, None, None])
    wb["PERFORMANCE"]["D2"] = 5.0
    path = tmp_path / "public.xlsx"
    wb.save(path)
    seen = {}

    def fake_get(url, timeout):
        seen["url"] = url
        return _Resp(200, path.read_bytes())

    monkeypatch.setattr(requests, "get", fake_get)
    book = PublicSpreadsheet(cfg)
    assert seen["url"].endswith(f"/d/{cfg.sheet.spreadsheet_id}/export?format=xlsx")
    bug = next(t for t in cfg.types if t.name == "BUG")
    row = SheetIO(book.worksheet("BUG"), cfg, bug).rows()[0]
    assert (row.get("title"), row.get("source"), row.get("Environment")) == ("Judul", "SIT", "env")
    assert book.worksheet("PERFORMANCE").get_all_values()[1][3] == "5"
    with pytest.raises(RuntimeError):
        book.worksheet("BUG").batch_update([])


def test_public_spreadsheet_not_shared(cfg, monkeypatch):
    import requests

    from issue_log_agent.sheet import PublicSpreadsheet

    monkeypatch.setattr(requests, "get", lambda url, timeout: _Resp(200, b"<!doctype html><title>Sign in</title>"))
    with pytest.raises(PermissionError, match="Anyone with the link"):
        PublicSpreadsheet(cfg)
