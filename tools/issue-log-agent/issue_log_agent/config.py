"""Load and validate the YAML configuration for the Issue Log agent."""

from __future__ import annotations

import os
from dataclasses import dataclass, field
from pathlib import Path
from typing import Any

import yaml


class ConfigError(ValueError):
    pass


@dataclass
class SheetConfig:
    spreadsheet_id: str
    worksheet: str
    header_row: int = 1
    credentials_file: str | None = None


@dataclass
class GitLabConfig:
    url: str
    project: str
    token: str
    default_labels: list[str] = field(default_factory=list)
    confidential: bool = False
    verify_ssl: bool = True


@dataclass
class Config:
    sheet: SheetConfig
    gitlab: GitLabConfig
    # logical field name -> header text in the spreadsheet
    columns: dict[str, str]
    # logical write-back field (issue_iid, issue_url, sync_status, ...) -> header text
    writeback: dict[str, str]
    title_template: str
    description_template: str
    # logical field -> {"template": "priority::{value}", "map": {...}}
    label_rules: dict[str, dict[str, Any]]
    required_fields: list[str]
    allowed_values: dict[str, list[str]]
    max_title_length: int
    # Only rows whose `trigger_field` value is in `trigger_values` are synced.
    trigger_field: str | None
    trigger_values: list[str]
    # sheet name -> GitLab username
    assignee_map: dict[str, str]
    # sheet status value -> "close" | "reopen" (applied with --update-existing)
    state_map: dict[str, str]
    date_format: str
    timezone: str


def _require(d: dict, key: str, ctx: str) -> Any:
    if key not in d or d[key] in (None, ""):
        raise ConfigError(f"Missing required config key '{ctx}.{key}'")
    return d[key]


def _expand_env(value: Any) -> Any:
    if isinstance(value, str):
        return os.path.expandvars(value)
    if isinstance(value, dict):
        return {k: _expand_env(v) for k, v in value.items()}
    if isinstance(value, list):
        return [_expand_env(v) for v in value]
    return value


def load_config(path: str | Path, require_token: bool = True) -> Config:
    raw = yaml.safe_load(Path(path).read_text(encoding="utf-8")) or {}
    raw = _expand_env(raw)

    s = _require(raw, "sheet", "")
    g = _require(raw, "gitlab", "")
    fmt = raw.get("format", {})
    rules = raw.get("rules", {})

    token = g.get("token") or os.environ.get("GITLAB_TOKEN", "")
    if token.startswith("$"):
        token = ""
    if require_token and not token:
        raise ConfigError("GitLab token not set (gitlab.token or env GITLAB_TOKEN)")

    columns = _require(raw, "columns", "")
    for section in ("columns", "writeback"):
        for k, v in (raw.get(section) or {}).items():
            if not isinstance(v, str):
                raise ConfigError(
                    f"{section}.{k} harus berupa teks (beri tanda kutip, mis. \"No\"), bukan {v!r}"
                )
    if "summary" not in columns:
        raise ConfigError("columns.summary is required (used for the issue title)")

    trigger = rules.get("trigger", {}) or {}

    return Config(
        sheet=SheetConfig(
            spreadsheet_id=_require(s, "spreadsheet_id", "sheet"),
            worksheet=_require(s, "worksheet", "sheet"),
            header_row=int(s.get("header_row", 1)),
            credentials_file=s.get("credentials_file")
            or os.environ.get("GOOGLE_APPLICATION_CREDENTIALS"),
        ),
        gitlab=GitLabConfig(
            url=_require(g, "url", "gitlab").rstrip("/"),
            project=str(_require(g, "project", "gitlab")),
            token=token,
            default_labels=list(g.get("default_labels", [])),
            confidential=bool(g.get("confidential", False)),
            verify_ssl=bool(g.get("verify_ssl", True)),
        ),
        columns=dict(columns),
        writeback=dict(raw.get("writeback", {})),
        title_template=fmt.get("title", "{summary}"),
        description_template=fmt.get("description", "{description}"),
        label_rules=dict(fmt.get("labels", {})),
        required_fields=list(rules.get("required", ["summary"])),
        allowed_values={k: [str(x) for x in v] for k, v in (rules.get("allowed_values") or {}).items()},
        max_title_length=int(rules.get("max_title_length", 255)),
        trigger_field=trigger.get("field"),
        trigger_values=[str(v) for v in trigger.get("values", [])],
        assignee_map=dict(raw.get("assignees", {})),
        state_map=dict(raw.get("state_map", {})),
        date_format=fmt.get("date_format", "%d/%m/%Y"),
        timezone=raw.get("timezone", "Asia/Jakarta"),
    )
