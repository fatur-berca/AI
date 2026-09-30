"""Turn a spreadsheet row into a GitLab issue payload, following the writing standard.

All formatting rules (title/description templates, labels, required fields,
allowed values) come from the config, so the standard can be tuned without
touching code.
"""

from __future__ import annotations

import re
import string
from dataclasses import dataclass, field
from datetime import datetime
from typing import Any

from .config import Config


@dataclass
class Row:
    number: int  # 1-based row number in the sheet
    values: dict[str, str]  # logical field -> cell value (trimmed)
    writeback: dict[str, str] = field(default_factory=dict)  # current write-back cells

    def get(self, key: str) -> str:
        return self.values.get(key, "")


@dataclass
class IssuePayload:
    title: str
    description: str
    labels: list[str]
    assignee_username: str | None = None
    due_date: str | None = None  # YYYY-MM-DD


class _SafeDict(dict):
    def __missing__(self, key: str) -> str:
        return ""


_formatter = string.Formatter()


def render(template: str, values: dict[str, Any]) -> str:
    return _formatter.vformat(template, (), _SafeDict(values))


def _clean_blank_sections(text: str) -> str:
    """Drop markdown sections whose body is empty and collapse extra blank lines."""
    lines = text.splitlines()
    out: list[str] = []
    i = 0
    while i < len(lines):
        line = lines[i]
        if line.lstrip().startswith("#"):
            j = i + 1
            while j < len(lines) and not lines[j].lstrip().startswith("#"):
                j += 1
            body = [l for l in lines[i + 1 : j] if l.strip() and l.strip() != "-"]
            if body:
                out.extend(lines[i:j])
            i = j
        else:
            out.append(line)
            i += 1
    return re.sub(r"\n{3,}", "\n\n", "\n".join(out)).strip() + "\n"


def validate(row: Row, cfg: Config) -> list[str]:
    errors: list[str] = []
    for f in cfg.required_fields:
        if not row.get(f):
            errors.append(f"'{cfg.columns.get(f, f)}' wajib diisi")
    for f, allowed in cfg.allowed_values.items():
        v = row.get(f)
        if v and v.lower() not in {a.lower() for a in allowed}:
            errors.append(
                f"'{cfg.columns.get(f, f)}' bernilai '{v}', harus salah satu dari: {', '.join(allowed)}"
            )
    title = render(cfg.title_template, row.values).strip()
    if len(title) > cfg.max_title_length:
        errors.append(f"Judul terlalu panjang ({len(title)} > {cfg.max_title_length} karakter)")
    return errors


def _labels(row: Row, cfg: Config) -> list[str]:
    labels = list(cfg.gitlab.default_labels)
    for f, rule in cfg.label_rules.items():
        v = row.get(f)
        if not v:
            continue
        mapping = {k.lower(): m for k, m in (rule.get("map") or {}).items()}
        mapped = mapping.get(v.lower(), v)
        if mapped:
            labels.append(render(rule.get("template", "{value}"), {"value": mapped}))
    seen: set[str] = set()
    return [l for l in labels if not (l in seen or seen.add(l))]


def _parse_date(value: str, fmt: str) -> str | None:
    if not value:
        return None
    for f in (fmt, "%Y-%m-%d", "%d/%m/%Y", "%d-%m-%Y", "%m/%d/%Y"):
        try:
            return datetime.strptime(value, f).strftime("%Y-%m-%d")
        except ValueError:
            continue
    return None


def build_issue(row: Row, cfg: Config, sheet_url: str = "") -> IssuePayload:
    ctx = dict(row.values)
    ctx["row_number"] = row.number
    ctx["sheet_url"] = sheet_url
    title = re.sub(r"\s+", " ", render(cfg.title_template, ctx)).strip()
    description = _clean_blank_sections(render(cfg.description_template, ctx))

    assignee = row.get("assignee")
    username = cfg.assignee_map.get(assignee, assignee.lstrip("@") if assignee.startswith("@") else None)

    return IssuePayload(
        title=title,
        description=description,
        labels=_labels(row, cfg),
        assignee_username=username or None,
        due_date=_parse_date(row.get("due_date"), cfg.date_format),
    )
