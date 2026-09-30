"""Google Sheets access: read a tab as rows and write sync results back."""

from __future__ import annotations

import csv
from dataclasses import dataclass, field
from pathlib import Path
from typing import Protocol

from .config import Config, IssueType


class Worksheet(Protocol):
    def get_all_values(self) -> list[list[str]]: ...
    def batch_update(self, data: list[dict], **kw) -> object: ...


@dataclass
class Row:
    number: int  # 1-based row number in the sheet
    values: dict[str, str]  # key -> trimmed cell value (see SheetIO for keys)
    writeback: dict[str, str] = field(default_factory=dict)

    def get(self, key: str) -> str:
        return self.values.get(key, "")


class SpreadsheetSource(Protocol):
    def worksheet(self, name: str) -> Worksheet: ...


class GoogleSpreadsheet:
    def __init__(self, cfg: Config):
        import gspread

        creds = cfg.sheet.credentials_file
        gc = gspread.service_account(filename=creds) if creds else gspread.service_account()
        self._book = gc.open_by_key(cfg.sheet.spreadsheet_id)

    def worksheet(self, name: str) -> Worksheet:
        return self._book.worksheet(name)


class CsvWorksheet:
    """Read-only stand-in for a worksheet, used with --csv-dir for offline dry-runs."""

    def __init__(self, path: Path):
        with open(path, newline="", encoding="utf-8-sig") as f:
            self._values = [list(r) for r in csv.reader(f)]

    def get_all_values(self) -> list[list[str]]:
        return self._values

    def batch_update(self, data: list[dict], **kw) -> object:
        raise RuntimeError("CSV source is read-only; use --dry-run")


class CsvSpreadsheet:
    """Directory with one `<worksheet>.csv` per tab."""

    def __init__(self, directory: str | Path):
        self.dir = Path(directory)

    def worksheet(self, name: str) -> Worksheet:
        path = self.dir / f"{name}.csv"
        if not path.exists():
            raise FileNotFoundError(path)
        return CsvWorksheet(path)


def col_letter(idx: int) -> str:
    """0-based column index -> A1 letter."""
    s = ""
    idx += 1
    while idx:
        idx, r = divmod(idx - 1, 26)
        s = chr(65 + r) + s
    return s


class SheetIO:
    """One tab of the backlog template.

    Row values are keyed by the section column header (e.g. "Background") and by the
    logical name of the common columns (title, source, status). Write-back cells are
    keyed by the logical write-back name (issue_iid, issue_url, ...).
    """

    def __init__(self, ws: Worksheet, cfg: Config, itype: IssueType):
        self.ws = ws
        self.cfg = cfg
        self.itype = itype
        self.values = ws.get_all_values()
        hr = cfg.sheet.header_row - 1
        headers = [h.strip().rstrip("*").strip().lower() for h in self.values[hr]] if len(self.values) > hr else []

        wanted = {sec.column: sec.column for sec in itype.sections}
        wanted.update(cfg.common_columns)
        wanted.update({f"wb:{k}": v for k, v in cfg.writeback.items()})
        self.col_index: dict[str, int] = {}
        self.missing: list[str] = []
        for key, header in wanted.items():
            try:
                self.col_index[key] = headers.index(header.strip().lower())
            except ValueError:
                self.missing.append(header)
        self._pending: list[dict] = []

    def has(self, key: str) -> bool:
        return key in self.col_index

    def rows(self) -> list[Row]:
        out: list[Row] = []
        start = self.cfg.sheet.header_row
        data_keys = [k for k in self.col_index if not k.startswith("wb:")]
        for offset, raw in enumerate(self.values[start:]):

            def cell(key: str) -> str:
                i = self.col_index.get(key)
                return raw[i].strip() if i is not None and i < len(raw) else ""

            values = {k: cell(k) for k in data_keys}
            # A row counts only if it has real content, not just a status/write-back value.
            if not any(v for k, v in values.items() if k != "status"):
                continue
            out.append(
                Row(
                    number=start + offset + 1,
                    values=values,
                    writeback={k: cell(f"wb:{k}") for k in self.cfg.writeback},
                )
            )
        return out

    def set(self, row: Row, updates: dict[str, str]) -> None:
        """Queue cell updates. Keys are write-back names or common column names."""
        for key, value in updates.items():
            i = self.col_index.get(f"wb:{key}", self.col_index.get(key))
            if i is None:
                continue
            self._pending.append({"range": f"{col_letter(i)}{row.number}", "values": [[value]]})

    def flush(self) -> None:
        if self._pending:
            self.ws.batch_update(self._pending, value_input_option="USER_ENTERED")
            self._pending = []
