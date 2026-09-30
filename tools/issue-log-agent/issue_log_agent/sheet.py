"""Google Sheets access: read rows as logical fields and write sync results back."""

from __future__ import annotations

from typing import Protocol

from .config import Config
from .formatter import Row


class Worksheet(Protocol):
    def get_all_values(self) -> list[list[str]]: ...
    def batch_update(self, data: list[dict], **kw) -> object: ...


def open_worksheet(cfg: Config) -> Worksheet:
    import gspread

    gc = gspread.service_account(filename=cfg.sheet.credentials_file) if cfg.sheet.credentials_file \
        else gspread.service_account()
    return gc.open_by_key(cfg.sheet.spreadsheet_id).worksheet(cfg.sheet.worksheet)


def _col_letter(idx: int) -> str:
    """0-based column index -> A1 letter."""
    s = ""
    idx += 1
    while idx:
        idx, r = divmod(idx - 1, 26)
        s = chr(65 + r) + s
    return s


class SheetIO:
    def __init__(self, ws: Worksheet, cfg: Config):
        self.ws = ws
        self.cfg = cfg
        self.values = ws.get_all_values()
        hr = cfg.sheet.header_row - 1
        if len(self.values) <= hr:
            raise ValueError(f"Worksheet has no header at row {cfg.sheet.header_row}")
        headers = [h.strip().lower() for h in self.values[hr]]
        self.col_index: dict[str, int] = {}
        self.missing: list[str] = []
        for key, header in {**cfg.columns, **{f"wb:{k}": v for k, v in cfg.writeback.items()}}.items():
            try:
                self.col_index[key] = headers.index(header.strip().lower())
            except ValueError:
                self.missing.append(header)
        self._pending: list[dict] = []

    def rows(self) -> list[Row]:
        out: list[Row] = []
        start = self.cfg.sheet.header_row
        for offset, raw in enumerate(self.values[start:]):
            if not any(c.strip() for c in raw):
                continue

            def cell(key: str) -> str:
                i = self.col_index.get(key)
                return raw[i].strip() if i is not None and i < len(raw) else ""

            out.append(
                Row(
                    number=start + offset + 1,
                    values={k: cell(k) for k in self.cfg.columns},
                    writeback={k: cell(f"wb:{k}") for k in self.cfg.writeback},
                )
            )
        return out

    def set(self, row: Row, updates: dict[str, str]) -> None:
        for key, value in updates.items():
            i = self.col_index.get(f"wb:{key}")
            if i is None:
                continue
            self._pending.append({"range": f"{_col_letter(i)}{row.number}", "values": [[value]]})
            row.writeback[key] = value

    def flush(self) -> None:
        if self._pending:
            self.ws.batch_update(self._pending, value_input_option="USER_ENTERED")
            self._pending = []


class CsvWorksheet:
    """Read-only stand-in for a worksheet, used with --csv for offline dry-runs."""

    def __init__(self, path: str):
        import csv

        with open(path, newline="", encoding="utf-8-sig") as f:
            self._values = [list(r) for r in csv.reader(f)]

    def get_all_values(self) -> list[list[str]]:
        return self._values

    def batch_update(self, data: list[dict], **kw) -> object:
        raise RuntimeError("CSV source is read-only; use --dry-run")
