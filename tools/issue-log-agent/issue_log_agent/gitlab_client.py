"""Minimal GitLab REST v4 client (only what the agent needs)."""

from __future__ import annotations

from typing import Any
from urllib.parse import quote

import requests

from .formatter import IssuePayload


class GitLabError(RuntimeError):
    pass


class GitLabClient:
    def __init__(self, url: str, project: str, token: str, verify_ssl: bool = True, timeout: int = 30):
        self.base = f"{url}/api/v4/projects/{quote(project, safe='')}"
        self.session = requests.Session()
        self.session.headers["PRIVATE-TOKEN"] = token
        self.session.verify = verify_ssl
        self.timeout = timeout

    def _req(self, method: str, path: str, **kw) -> Any:
        r = self.session.request(method, f"{self.base}{path}", timeout=self.timeout, **kw)
        if r.status_code >= 400:
            raise GitLabError(f"{method} {path} -> {r.status_code}: {r.text[:300]}")
        return r.json() if r.content else None

    def find_issue_by_marker(self, marker: str) -> dict | None:
        """Look for an existing issue whose description contains `marker` (dedup guard)."""
        issues = self._req("GET", "/issues", params={"search": marker, "in": "description", "per_page": 5})
        for i in issues or []:
            if marker in (i.get("description") or ""):
                return i
        return None

    def get_issue(self, iid: int) -> dict:
        return self._req("GET", f"/issues/{iid}")

    def create_issue(self, p: IssuePayload, confidential: bool = False) -> dict:
        return self._req(
            "POST",
            "/issues",
            json={"title": p.title, "description": p.description, "labels": ",".join(p.labels),
                  "confidential": confidential},
        )

    def update_issue(
        self,
        iid: int,
        *,
        title: str | None = None,
        description: str | None = None,
        add_labels: list[str] | None = None,
        remove_labels: list[str] | None = None,
        state_event: str | None = None,
    ) -> dict:
        data: dict[str, Any] = {}
        if title is not None:
            data["title"] = title
        if description is not None:
            data["description"] = description
        if add_labels:
            data["add_labels"] = ",".join(add_labels)
        if remove_labels:
            data["remove_labels"] = ",".join(remove_labels)
        if state_event:
            data["state_event"] = state_event
        return self._req("PUT", f"/issues/{iid}", json=data)
