"""Load and validate the YAML configuration for the Issue Backlog agent."""

from __future__ import annotations

import os
from dataclasses import dataclass, field
from pathlib import Path
from typing import Any

import yaml

# Default body formatting per section heading (see Development Guideline slide 6-13).
_DEFAULT_STYLES = {
    "acceptance criteria": "checklist",
    "steps to reproduce": "numbered",
    "scope": "bullets",
    "business / functional requirement": "bullets",
    "out of scope": "bullets",
    "reference": "bullets",
    "environment": "bullets",
    "evidence": "bullets",
    "evidence / reference": "bullets",
}
STYLES = {"text", "bullets", "numbered", "checklist"}


class ConfigError(ValueError):
    pass


@dataclass
class SheetConfig:
    spreadsheet_id: str
    header_row: int = 1
    credentials_file: str | None = None


@dataclass
class GitLabConfig:
    url: str
    project: str
    token: str
    confidential: bool = False
    verify_ssl: bool = True


@dataclass
class Section:
    heading: str  # "## <heading>" in the issue body
    column: str  # header text in the sheet
    style: str = "text"
    required: bool = False


@dataclass
class IssueType:
    name: str  # FEATURE / BUG / SECURITY / PERFORMANCE -> prefix 1
    worksheet: str
    sources: list[str]  # allowed prefix 2 values
    sections: list[Section]
    labels: list[str] = field(default_factory=list)
    # columns scanned to infer the source when the Source cell is empty
    infer_source_from: list[str] = field(default_factory=list)


@dataclass
class StatusRule:
    labels: list[str] = field(default_factory=list)
    state: str | None = None  # "close" | "reopen" | None


@dataclass
class ClaudeConfig:
    enabled: bool = False
    model: str = "claude-opus-5-5"
    effort: str = "low"


@dataclass
class Config:
    sheet: SheetConfig
    gitlab: GitLabConfig
    types: list[IssueType]
    # logical field (title, source, status) -> header text; same on every tab
    common_columns: dict[str, str]
    # logical write-back field (issue_iid, issue_url, sync_status, ...) -> header text
    writeback: dict[str, str]
    title_format: str
    max_title_length: int
    default_labels: list[str]
    source_label: str  # e.g. "source::{source}", empty to disable
    statuses: dict[str, StatusRule]
    default_status: str
    skip_statuses: list[str]
    claude: ClaudeConfig
    timezone: str

    def status_rule(self, status: str) -> StatusRule | None:
        s = (status or self.default_status).strip().lower()
        for name, rule in self.statuses.items():
            if name.lower() == s:
                return rule
        return None

    @property
    def all_status_labels(self) -> set[str]:
        return {l for r in self.statuses.values() for l in r.labels}


def _require(d: Any, key: str, ctx: str) -> Any:
    if not isinstance(d, dict) or key not in d or d[key] in (None, ""):
        raise ConfigError(f"Missing required config key '{ctx}{key}'")
    return d[key]


def _expand_env(value: Any) -> Any:
    if isinstance(value, str):
        expanded = os.path.expandvars(value)
        # An unset ${VAR} stays literal; treat it as "not configured" instead of a real value.
        return "" if "${" in expanded else expanded
    if isinstance(value, dict):
        return {k: _expand_env(v) for k, v in value.items()}
    if isinstance(value, list):
        return [_expand_env(v) for v in value]
    return value


def _text(value: Any, ctx: str) -> str:
    if not isinstance(value, str):
        # YAML turns bare No/Yes/On/Off into booleans
        raise ConfigError(f"{ctx} harus berupa teks (beri tanda kutip), bukan {value!r}")
    return value


def _section(raw: Any, ctx: str) -> Section:
    if isinstance(raw, str):
        raw = {"column": raw}
    column = _text(_require(raw, "column", ctx + "."), ctx + ".column")
    heading = _text(raw.get("heading", column), ctx + ".heading")
    style = raw.get("style") or _DEFAULT_STYLES.get(heading.lower(), "text")
    if style not in STYLES:
        raise ConfigError(f"{ctx}.style '{style}' tidak dikenal (pilih: {', '.join(sorted(STYLES))})")
    return Section(heading=heading, column=column, style=style, required=bool(raw.get("required", False)))


def load_config(path: str | Path, require_token: bool = True) -> Config:
    raw = _expand_env(yaml.safe_load(Path(path).read_text(encoding="utf-8")) or {})

    s = _require(raw, "sheet", "")
    g = _require(raw, "gitlab", "")

    token = g.get("token") or os.environ.get("GITLAB_TOKEN", "")
    if require_token and not token:
        raise ConfigError("GitLab token belum di-set (gitlab.token atau env GITLAB_TOKEN)")

    types: list[IssueType] = []
    for name, t in (_require(raw, "types", "") or {}).items():
        ctx = f"types.{name}"
        types.append(
            IssueType(
                name=str(name).upper(),
                worksheet=_text(t.get("worksheet", name), ctx + ".worksheet"),
                sources=[str(x).upper() for x in _require(t, "sources", ctx + ".")],
                sections=[
                    _section(x, f"{ctx}.sections[{i}]") for i, x in enumerate(_require(t, "sections", ctx + "."))
                ],
                labels=list(t.get("labels", [])),
                infer_source_from=list(t.get("infer_source_from", [])),
            )
        )

    common = {k: _text(v, f"columns.{k}") for k, v in (raw.get("columns") or {}).items()}
    writeback = {k: _text(v, f"writeback.{k}") for k, v in (raw.get("writeback") or {}).items()}
    title = raw.get("title") or {}
    labels = raw.get("labels") or {}
    claude = raw.get("claude") or {}

    statuses = {
        str(name): StatusRule(labels=list((r or {}).get("labels", [])), state=(r or {}).get("state"))
        for name, r in (raw.get("statuses") or {}).items()
    }

    return Config(
        sheet=SheetConfig(
            spreadsheet_id=_require(s, "spreadsheet_id", "sheet."),
            header_row=int(s.get("header_row", 1)),
            credentials_file=s.get("credentials_file") or os.environ.get("GOOGLE_APPLICATION_CREDENTIALS"),
        ),
        gitlab=GitLabConfig(
            url=_require(g, "url", "gitlab.").rstrip("/"),
            project=str(_require(g, "project", "gitlab.")),
            token=token,
            confidential=bool(g.get("confidential", False)),
            verify_ssl=bool(g.get("verify_ssl", True)),
        ),
        types=types,
        common_columns=common,
        writeback=writeback,
        title_format=title.get("format", "[{type}][{source}]-{title}"),
        max_title_length=int(title.get("max_length", 255)),
        default_labels=list(labels.get("default", [])),
        source_label=labels.get("source", ""),
        statuses=statuses,
        default_status=str(raw.get("default_status", "Open")),
        skip_statuses=[str(x) for x in raw.get("skip_statuses", [])],
        claude=ClaudeConfig(
            enabled=bool(claude.get("enabled", False)),
            model=claude.get("model", "claude-opus-5-5"),
            effort=claude.get("effort", "low"),
        ),
        timezone=raw.get("timezone", "Asia/Jakarta"),
    )
