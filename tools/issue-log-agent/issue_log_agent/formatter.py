"""Turn a backlog row into a GitLab issue following the Development Guideline.

Title (slide 5):   [PREFIX 1][PREFIX 2]-<judul>, e.g. [FEATURE][FSD]-Penambahan column salary
Body  (slide 6-13): "## <Section>" blocks in the order defined per type in the config.
"""

from __future__ import annotations

import hashlib
import re
from dataclasses import dataclass, field

from .config import Config, IssueType, Section
from .sheet import Row

_TITLE_PREFIX = re.compile(r"^\s*\[\s*([A-Za-z]+)\s*\]\s*\[\s*([A-Za-z]+)\s*\]\s*[-–—]?\s*(.*)$", re.S)
_LIST_MARKER = re.compile(r"^\s*(?:-\s*\[[ xX]\]|[-*•●▪]|\d+[.)])\s*")
_CHECKED = re.compile(r"^\s*-?\s*\[[xX]\]")


@dataclass
class ResolvedTitle:
    title: str = ""  # bare title, without prefixes
    source: str = ""
    errors: list[str] = field(default_factory=list)
    title_from_cell: bool = False
    source_from_cell: bool = False


@dataclass
class IssuePayload:
    title: str
    description: str
    labels: list[str]
    marker: str


def resolve_title(row: Row, itype: IssueType) -> ResolvedTitle:
    """Read Title/Source cells. Accepts a title that already carries the [TYPE][SOURCE]- prefix."""
    r = ResolvedTitle(title=row.get("title"), source=row.get("source").upper())
    r.title_from_cell, r.source_from_cell = bool(r.title), bool(r.source)
    m = _TITLE_PREFIX.match(r.title)
    if m:
        p1, p2, rest = m.group(1).upper(), m.group(2).upper(), m.group(3).strip()
        if p1 != itype.name:
            r.errors.append(f"Prefix judul [{p1}] tidak sesuai tab {itype.name}")
        if r.source and r.source != p2:
            r.errors.append(f"Prefix judul [{p2}] berbeda dengan kolom Source ({r.source})")
        r.source = r.source or p2
        r.title = rest
    if not r.source:
        r.source = infer_source(row, itype)
    return r


def infer_source(row: Row, itype: IssueType) -> str:
    """Pick the source when exactly one allowed value appears as a word in the hint columns."""
    found = set()
    for col in itype.infer_source_from:
        text = row.get(col).upper()
        for s in itype.sources:
            if re.search(rf"(?<![A-Z]){re.escape(s)}(?![A-Z])", text):
                found.add(s)
    return found.pop() if len(found) == 1 else ""


def format_section(value: str, style: str) -> str:
    lines = [l.rstrip() for l in value.replace("\r\n", "\n").split("\n")]
    if style == "text":
        return "\n".join(lines).strip()
    items = [l for l in lines if l.strip()]
    out = []
    for i, line in enumerate(items, 1):
        text = _LIST_MARKER.sub("", line).strip()
        if style == "checklist":
            out.append(f"- [{'x' if _CHECKED.match(line) else ' '}] {text}")
        elif style == "numbered":
            out.append(f"{i}. {text}")
        else:
            out.append(f"- {text}")
    return "\n".join(out)


def full_title(cfg: Config, itype: IssueType, title: str, source: str) -> str:
    t = cfg.title_format.format(type=itype.name, source=source, title=title.strip())
    return re.sub(r"\s+", " ", t).strip()


def validate(row: Row, cfg: Config, itype: IssueType, rt: ResolvedTitle) -> list[str]:
    errors = list(rt.errors)
    if not rt.title:
        errors.append("Title wajib diisi")
    if not rt.source:
        errors.append(f"Source wajib diisi (salah satu dari: {', '.join(itype.sources)})")
    elif rt.source not in itype.sources:
        errors.append(f"Source '{rt.source}' tidak valid untuk {itype.name} (pilih: {', '.join(itype.sources)})")
    for sec in itype.sections:
        if sec.required and not row.get(sec.column):
            errors.append(f"'{sec.column}' wajib diisi")
    status = row.get("status")
    if status and cfg.statuses and cfg.status_rule(status) is None:
        errors.append(f"Issue Board Status '{status}' tidak dikenal (pilih: {', '.join(cfg.statuses)})")
    if rt.title and rt.source:
        n = len(full_title(cfg, itype, rt.title, rt.source))
        if n > cfg.max_title_length:
            errors.append(f"Judul terlalu panjang ({n} > {cfg.max_title_length} karakter)")
    return errors


def build_body(row: Row, sections: list[Section]) -> str:
    blocks = []
    for sec in sections:
        value = row.get(sec.column)
        if value:
            blocks.append(f"## {sec.heading}\n{format_section(value, sec.style)}")
    return "\n\n".join(blocks) + "\n"


def content_marker(cfg: Config, itype: IssueType, row: Row) -> str:
    """Stable id for a row's content, embedded in the issue body to prevent duplicates."""
    content = "\x1f".join([itype.name] + [row.get(s.column) for s in itype.sections])
    digest = hashlib.sha1(content.encode("utf-8")).hexdigest()[:12]
    return f"<!-- issue-backlog:{cfg.sheet.spreadsheet_id}:{itype.worksheet}:{digest} -->"


def labels_for(cfg: Config, itype: IssueType, source: str, status: str) -> list[str]:
    labels = list(cfg.default_labels) + list(itype.labels)
    if cfg.source_label and source:
        labels.append(cfg.source_label.format(source=source))
    rule = cfg.status_rule(status)
    if rule:
        labels.extend(rule.labels)
    seen: set[str] = set()
    return [l for l in labels if not (l in seen or seen.add(l))]


def build_issue(row: Row, cfg: Config, itype: IssueType, rt: ResolvedTitle) -> IssuePayload:
    marker = content_marker(cfg, itype, row)
    return IssuePayload(
        title=full_title(cfg, itype, rt.title, rt.source),
        description=f"{build_body(row, itype.sections).rstrip()}\n\n{marker}\n",
        labels=labels_for(cfg, itype, rt.source, row.get("status")),
        marker=marker,
    )
