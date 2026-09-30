import sys
from pathlib import Path

import pytest

openpyxl = pytest.importorskip("openpyxl")

ROOT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(ROOT / "scripts"))

from make_template import revise  # noqa: E402

from issue_log_agent.config import load_config  # noqa: E402
from issue_log_agent.sheet import SheetIO  # noqa: E402


class _Ws:
    def __init__(self, ws):
        self.v = [["" if c is None else str(c) for c in r] for r in ws.iter_rows(values_only=True)]

    def get_all_values(self):
        return self.v


def test_v1_to_v2_keeps_data_and_matches_config(tmp_path, monkeypatch):
    monkeypatch.setenv("GITLAB_TOKEN", "x")
    v1 = tmp_path / "v1.xlsx"
    wb = openpyxl.load_workbook(ROOT / "templates" / "Template_Backlog_v1.xlsx")
    wb["BUG"].append(["bg", "SIT env", "1. a", "exp", "act", "imp", "ac", "ev", "ref", "Testing"])
    wb.save(v1)

    out = tmp_path / "v2.xlsx"
    revise(v1, out, ROOT / "config.example.yaml")
    cfg = load_config(ROOT / "config.example.yaml")
    v2 = openpyxl.load_workbook(out)

    for t in cfg.types:
        sheet = SheetIO(_Ws(v2[t.worksheet]), cfg, t)
        assert sheet.missing == [], t.name

    bug = next(t for t in cfg.types if t.name == "BUG")
    row = SheetIO(_Ws(v2["BUG"]), cfg, bug).rows()[0]
    assert row.get("Environment") == "SIT env"
    assert row.get("Reference") == "ref"
    assert row.get("status") == "Testing"
    assert row.get("Description") == ""
    # header style and tab colour come from v1
    assert v2["BUG"]["A1"].fill.fgColor.rgb == "FFA61C00"
    assert v2["BUG"].sheet_properties.tabColor.rgb == "FF980000"


def test_committed_v2_is_up_to_date(tmp_path, monkeypatch):
    monkeypatch.setenv("GITLAB_TOKEN", "x")
    out = tmp_path / "v2.xlsx"
    revise(ROOT / "templates" / "Template_Backlog_v1.xlsx", out, ROOT / "config.example.yaml")
    fresh, committed = openpyxl.load_workbook(out), openpyxl.load_workbook(ROOT / "templates" / "Template_Backlog_v2.xlsx")
    for name in fresh.sheetnames:
        assert [c.value for c in fresh[name][1]] == [c.value for c in committed[name][1]], name
