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
        self.api = f"{url}/api/v4"
        self.session = requests.Session()
        self.session.headers["PRIVATE-TOKEN"] = token
        self.session.verify = verify_ssl
        self.timeout = timeout
        self._user_cache: dict[str, int | None] = {}

    def _req(self, method: str, path: str, *, base: str | None = None, **kw) -> Any:
        r = self.session.request(method, f"{base or self.base}{path}", timeout=self.timeout, **kw)
        if r.status_code >= 400:
            raise GitLabError(f"{method} {path} -> {r.status_code}: {r.text[:300]}")
        return r.json() if r.content else None

    def user_id(self, username: str) -> int | None:
        if username not in self._user_cache:
            users = self._req("GET", "/users", base=self.api, params={"username": username})
            self._user_cache[username] = users[0]["id"] if users else None
        return self._user_cache[username]

    def find_issue_by_marker(self, marker: str) -> dict | None:
        """Look for an existing issue whose description contains `marker` (dedup guard)."""
        issues = self._req("GET", "/issues", params={"search": marker, "in": "description", "per_page": 5})
        for i in issues or []:
            if marker in (i.get("description") or ""):
                return i
        return None

    def _payload(self, p: IssuePayload, confidential: bool) -> dict[str, Any]:
        data: dict[str, Any] = {
            "title": p.title,
            "description": p.description,
            "labels": ",".join(p.labels),
            "confidential": confidential,
        }
        if p.due_date:
            data["due_date"] = p.due_date
        if p.assignee_username:
            uid = self.user_id(p.assignee_username)
            if uid:
                data["assignee_ids"] = [uid]
        return data

    def create_issue(self, p: IssuePayload, confidential: bool = False) -> dict:
        return self._req("POST", "/issues", json=self._payload(p, confidential))

    def update_issue(self, iid: int, p: IssuePayload, state_event: str | None = None) -> dict:
        data = self._payload(p, confidential=False)
        data.pop("confidential")
        if state_event:
            data["state_event"] = state_event
        return self._req("PUT", f"/issues/{iid}", json=data)
