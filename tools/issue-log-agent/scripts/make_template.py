"""Generate Template Issue Backlog v2 (.xlsx) from the config.

Kolom tiap tab = Title, Source, <sections>, Issue Board Status, <writeback>.
Source & Issue Board Status diberi dropdown. Upload hasilnya ke Google Drive lalu
"Open with Google Sheets", atau salin header-nya ke spreadsheet yang sudah ada.

    pip install openpyxl
    python scripts/make_template.py --config config.example.yaml --out templates/Template_Backlog_v2.xlsx
"""

from __future__ import annotations

import argparse
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parents[1]))

from openpyxl import Workbook  # noqa: E402
from openpyxl.styles import Alignment, Font, PatternFill  # noqa: E402
from openpyxl.utils import get_column_letter  # noqa: E402
from openpyxl.worksheet.datavalidation import DataValidation  # noqa: E402

from issue_log_agent.config import load_config  # noqa: E402

INPUT_FILL = PatternFill("solid", fgColor="1F4E78")
AGENT_FILL = PatternFill("solid", fgColor="7F7F7F")
MAX_ROWS = 1000


def main() -> None:
    ap = argparse.ArgumentParser()
    ap.add_argument("--config", default="config.example.yaml")
    ap.add_argument("--out", default="templates/Template_Backlog_v2.xlsx")
    args = ap.parse_args()
    cfg = load_config(args.config, require_token=False)

    wb = Workbook()
    wb.remove(wb.active)
    statuses = list(cfg.statuses)
    for t in cfg.types:
        ws = wb.create_sheet(t.worksheet)
        inputs = [cfg.common_columns["title"], cfg.common_columns["source"]]
        inputs += [s.column for s in t.sections] + [cfg.common_columns["status"]]
        agent = list(cfg.writeback.values())
        required = {s.column for s in t.sections if s.required} | {inputs[1]}
        for i, name in enumerate(inputs + agent, 1):
            c = ws.cell(row=1, column=i, value=name + (" *" if name in required else ""))
            c.font = Font(bold=True, color="FFFFFF")
            c.fill = INPUT_FILL if i <= len(inputs) else AGENT_FILL
            c.alignment = Alignment(wrap_text=True, vertical="center")
            ws.column_dimensions[get_column_letter(i)].width = 18 if i > len(inputs) else 32
        ws.freeze_panes = "B2"

        def dropdown(col_name: str, values: list[str]) -> None:
            col = get_column_letter(inputs.index(col_name) + 1)
            dv = DataValidation(type="list", formula1='"' + ",".join(values) + '"', allow_blank=True)
            dv.add(f"{col}2:{col}{MAX_ROWS}")
            ws.add_data_validation(dv)

        dropdown(inputs[1], t.sources)
        dropdown(inputs[-1], statuses)

    Path(args.out).parent.mkdir(parents=True, exist_ok=True)
    wb.save(args.out)
    print(f"Template ditulis ke {args.out}")


if __name__ == "__main__":
    main()
