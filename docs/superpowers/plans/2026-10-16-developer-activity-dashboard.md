# Developer Activity Dashboard Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Menambahkan monitoring aktivitas developer (siapa mengerjakan issue apa, sejak kapan, issue mana yang macet) ke Issue Backlog Agent, dengan output tab data dan tab `Dashboard` di Google Sheet backlog.

**Architecture:** Modul `activity.py` mengambil data read-only dari GitLab (`collect`), menghitung durasi dan flag sebagai logika murni (`compute`), lalu menulis dua tab data. Modul `dashboard.py` membuat tab `Dashboard` sekali saja, berisi formula, chart, dan conditional formatting yang membaca tab data. Setiap run hanya menulis ulang tab data dan sel timestamp. `__main__.py` menjalankan monitor ini setelah sync backlog.

**Tech Stack:** Python 3.10+, `requests` (GitLab REST v4), `gspread` 6 (Google Sheets API v4 `batchUpdate`), PyYAML, pytest.

**Spec:** `docs/superpowers/specs/2026-10-16-developer-activity-design.md`

## Global Constraints

- Semua kode berada di `tools/issue-log-agent/`. Semua perintah dijalankan dari folder itu.
- Teks untuk user (log, header sheet, komentar config, README) ditulis dalam bahasa Indonesia, mengikuti gaya file yang sudah ada. Nama identifier ditulis dalam bahasa Inggris.
- Durasi dihitung dalam hari kerja (Senin–Jumat) di `timezone` config (default `Asia/Jakarta`), dikurangi `activity.holidays`, dan dibulatkan 1 desimal.
- Format waktu di sheet: `YYYY-MM-DD HH:MM`, di `timezone` config.
- Nama tab default: `Developer Activity`, `Developer Summary`, `Dashboard`.
- Flag: `IDLE` (idle ≥ `idle_days`, default 2), `LAMA` (hari di status ≥ `max_days[status]`, default On-Progress 5 / Testing 3 / Re-work 2), `REWORK` (Re-work ≥ `rework_limit`, default 2), `NO ASSIGNEE`. Developer tanpa assignee ditulis `(belum di-assign)`.
- Akses GitLab dari fitur ini hanya read-only.
- Kalau `list_issues` gagal, tidak ada tab yang diubah, dan exit code menjadi 1.
- Fitur ini tidak boleh menambah dependency baru selain `pytest` untuk test.

## Review Focus

1. Title atau nama developer yang diawali `=`, `+`, `-`, atau `@` ditulis dengan `value_input_option=USER_ENTERED`. Tanpa escape, Sheets akan menjalankannya sebagai formula. Yang diharapkan: teks tampil apa adanya. Test ada di Task 4 (`test_values_escape_formulas`).
2. Label yang sudah dihapus di GitLab membuat event berisi `"label": null`. Yang diharapkan: event diabaikan, tidak crash. Test ada di Task 4 (`test_compute_durations_and_flags`, issue 12).
3. Satu issue bisa punya dua label status sekaligus, misalnya On-Progress dan Testing. Yang diharapkan: status dipilih secara deterministik sesuai urutan `statuses` di config. Test ada di Task 4 (`test_two_status_labels_uses_config_order`).
4. Project dengan lebih dari 100 issue atau event. Yang diharapkan: semua halaman diambil lewat `X-Next-Page`. Test ada di Task 3 (`test_get_all_follows_pagination`).
5. Tab Dashboard dihapus manual sementara tab data masih ada, atau dashboard di-rebuild. Yang diharapkan: dashboard dibuat ulang tanpa menggandakan conditional formatting di tab data. Test ada di Task 5 (`test_recreate_dashboard_without_duplicate_activity_rules`, `test_rebuild_recreates_all_tabs`).

---

## File Structure

| File | Status | Tanggung jawab |
|---|---|---|
| `issue_log_agent/workdays.py` | baru | `business_days_between()` |
| `issue_log_agent/config.py` | ubah | `ActivityConfig` + parsing/validasi blok `activity` |
| `config.example.yaml` | ubah | blok `activity` dengan komentar |
| `issue_log_agent/gitlab_client.py` | ubah | `_get_all` (pagination), `list_issues`, `label_events`, `notes`, `related_merge_requests` |
| `issue_log_agent/activity.py` | baru | dataclass, `compute`, `collect`, render tab, `ActivityMonitor` |
| `issue_log_agent/dashboard.py` | baru | `ensure()` (layout, chart, CF) dan `stamp()` |
| `issue_log_agent/sheet.py` | ubah | `GoogleSpreadsheet.ensure_worksheet / delete_worksheet / batch_update` |
| `issue_log_agent/__main__.py` | ubah | flag CLI + `run_activity()` |
| `README.md` | ubah | bagian "Developer Activity Dashboard" + tabel Struktur |
| `tests/test_workdays.py`, `tests/test_activity.py`, `tests/test_dashboard.py`, `tests/test_gitlab_client.py`, `tests/activity_fakes.py` | baru | test + fake bersama |
| `tests/test_sync.py` | ubah | test config activity + `run_activity` |

---

### Task 1: Hari kerja (`workdays.py`)

**Files:**
- Create: `issue_log_agent/workdays.py`
- Test: `tests/test_workdays.py`

**Interfaces:**
- Produces: `business_days_between(start: datetime, end: datetime, tz: str, holidays: Collection[date] = ()) -> float`. `start` dan `end` adalah datetime yang sudah punya timezone. Hasilnya hari kerja yang berlalu, dibulatkan 1 desimal, dan `0.0` kalau `end <= start`.

- [ ] **Step 1: Siapkan environment (sekali saja)**

Run:
```bash
python3 -m venv .venv && .venv/bin/pip install -q -r requirements.txt pytest
.venv/bin/python -m pytest -q tests
```
Expected: semua test yang sudah ada PASS (`.venv/` sudah ada di `.gitignore`). Di Windows pakai `.venv\Scripts\python`.

- [ ] **Step 2: Tulis test yang gagal**

`tests/test_workdays.py`:
```python
from datetime import date, datetime, timezone
from zoneinfo import ZoneInfo

from issue_log_agent.workdays import business_days_between

WIB = ZoneInfo("Asia/Jakarta")
TZ = "Asia/Jakarta"


def wib(*a):
    return datetime(*a, tzinfo=WIB)


def test_same_day_fraction():
    assert business_days_between(wib(2026, 10, 14, 6), wib(2026, 10, 14, 18), TZ) == 0.5


def test_skips_weekend():
    # Jumat 10:00 -> Senin 10:00 = 14 jam Jumat + 10 jam Senin
    assert business_days_between(wib(2026, 10, 16, 10), wib(2026, 10, 19, 10), TZ) == 1.0


def test_skips_holidays():
    # Senin 19 Okt libur: Jumat 14 jam + Selasa 10 jam
    holidays = {date(2026, 10, 19)}
    assert business_days_between(wib(2026, 10, 16, 10), wib(2026, 10, 20, 10), TZ, holidays) == 1.0


def test_converts_utc_input_to_local_days():
    # 2026-10-16T20:00Z = Sabtu 03:00 WIB -> tidak dihitung
    start = datetime(2026, 10, 16, 20, tzinfo=timezone.utc)
    assert business_days_between(start, wib(2026, 10, 18, 23), TZ) == 0.0


def test_end_before_start_is_zero():
    assert business_days_between(wib(2026, 10, 15), wib(2026, 10, 14), TZ) == 0.0
```

- [ ] **Step 3: Jalankan test, pastikan gagal**

Run: `.venv/bin/python -m pytest -q tests/test_workdays.py`
Expected: FAIL, `ModuleNotFoundError: No module named 'issue_log_agent.workdays'`

- [ ] **Step 4: Implementasi**

`issue_log_agent/workdays.py`:
```python
"""Hitung durasi dalam hari kerja (Senin-Jumat, dikurangi tanggal merah)."""

from __future__ import annotations

from collections.abc import Collection
from datetime import date, datetime, time, timedelta
from zoneinfo import ZoneInfo


def business_days_between(
    start: datetime, end: datetime, tz: str, holidays: Collection[date] = ()
) -> float:
    """Hari kerja yang berlalu dari `start` ke `end` (1.0 = 24 jam di hari kerja), 1 desimal."""
    zone = ZoneInfo(tz)
    start, end = start.astimezone(zone), end.astimezone(zone)
    if end <= start:
        return 0.0
    total = 0.0
    day = start.date()
    while day <= end.date():
        if day.weekday() < 5 and day not in holidays:
            lo = max(start, datetime.combine(day, time.min, zone))
            hi = min(end, datetime.combine(day + timedelta(days=1), time.min, zone))
            total += (hi - lo).total_seconds() / 86400
        day += timedelta(days=1)
    return round(total, 1)
```

- [ ] **Step 5: Jalankan test, pastikan lulus**

Run: `.venv/bin/python -m pytest -q tests/test_workdays.py`
Expected: 5 passed

- [ ] **Step 6: Commit**

```bash
git add issue_log_agent/workdays.py tests/test_workdays.py
git commit -m "Add business-day duration helper for developer activity"
```

---

### Task 2: Config `activity`

**Files:**
- Modify: `issue_log_agent/config.py` (dataclass baru setelah `ClaudeConfig`, field baru di akhir `Config`, parsing di `load_config`)
- Modify: `config.example.yaml` (blok baru di akhir file)
- Test: `tests/test_sync.py` (tambah di akhir file)

**Interfaces:**
- Produces: `cfg.activity: ActivityConfig` dengan field `enabled: bool`, `active_statuses: list[str]`, `idle_days: float`, `max_days: dict[str, float]`, `rework_limit: int`, `rework_status: str`, `holidays: set[date]`, `activity_tab: str`, `summary_tab: str`, `dashboard_tab: str`. Nama status di `active_statuses`, `max_days`, dan `rework_status` divalidasi sebagai key `cfg.statuses`.

- [ ] **Step 1: Tulis test yang gagal**

Tambahkan di akhir `tests/test_sync.py`:
```python
import datetime as _dt

import yaml

from issue_log_agent.config import ConfigError


def _example_without(tmp_path, mutate):
    raw = yaml.safe_load((ROOT / "config.example.yaml").read_text(encoding="utf-8"))
    mutate(raw)
    path = tmp_path / "config.yaml"
    path.write_text(yaml.safe_dump(raw, allow_unicode=True), encoding="utf-8")
    return path


def test_activity_config_from_example():
    a = load_config(ROOT / "config.example.yaml", require_token=False).activity
    assert a.enabled is True
    assert a.active_statuses == ["On-Progress", "Testing", "Re-work", "Ready to Deploy"]
    assert a.idle_days == 2
    assert a.max_days == {"On-Progress": 5, "Testing": 3, "Re-work": 2}
    assert a.rework_limit == 2 and a.rework_status == "Re-work"
    assert _dt.date(2026, 12, 25) in a.holidays
    assert (a.activity_tab, a.summary_tab, a.dashboard_tab) == ("Developer Activity", "Developer Summary", "Dashboard")


def test_activity_config_defaults_when_missing(tmp_path):
    path = _example_without(tmp_path, lambda raw: raw.pop("activity"))
    a = load_config(path, require_token=False).activity
    assert a.enabled is True and a.idle_days == 2 and a.max_days == {} and a.holidays == set()


def test_activity_config_rejects_unknown_status(tmp_path):
    path = _example_without(tmp_path, lambda raw: raw["activity"]["max_days"].update({"Review": 1}))
    with pytest.raises(ConfigError, match="activity.max_days.*Review"):
        load_config(path, require_token=False)
```

- [ ] **Step 2: Jalankan test, pastikan gagal**

Run: `.venv/bin/python -m pytest -q tests/test_sync.py -k activity_config`
Expected: FAIL, `AttributeError: 'Config' object has no attribute 'activity'`

- [ ] **Step 3: Implementasi di `config.py`**

Tambahkan `from datetime import date` di blok import. Setelah `class ClaudeConfig`, tambahkan:
```python
@dataclass
class ActivityConfig:
    enabled: bool = True
    active_statuses: list[str] = field(
        default_factory=lambda: ["On-Progress", "Testing", "Re-work", "Ready to Deploy"]
    )
    idle_days: float = 2
    max_days: dict[str, float] = field(default_factory=dict)  # status -> hari kerja
    rework_limit: int = 2
    rework_status: str = "Re-work"
    holidays: set[date] = field(default_factory=set)
    activity_tab: str = "Developer Activity"
    summary_tab: str = "Developer Summary"
    dashboard_tab: str = "Dashboard"
```

Tambahkan field terakhir di `class Config` (setelah `timezone: str`):
```python
    activity: ActivityConfig = field(default_factory=ActivityConfig)
```

Sebelum `def load_config`, tambahkan:
```python
def _activity(raw: Any, statuses: dict[str, StatusRule]) -> ActivityConfig:
    a = raw or {}
    d = ActivityConfig()
    tabs = a.get("tabs") or {}

    def known(name: Any, ctx: str) -> str:
        name = _text(name, ctx)
        if name not in statuses:
            raise ConfigError(f"{ctx}: status '{name}' tidak ada di statuses ({', '.join(statuses)})")
        return name

    try:
        holidays = {date.fromisoformat(str(x)) for x in a.get("holidays") or []}
    except ValueError as e:
        raise ConfigError(f"activity.holidays: {e} (format YYYY-MM-DD)") from e

    return ActivityConfig(
        enabled=bool(a.get("enabled", d.enabled)),
        active_statuses=[known(x, "activity.active_statuses") for x in a.get("active_statuses", d.active_statuses)],
        idle_days=float(a.get("idle_days", d.idle_days)),
        max_days={known(k, "activity.max_days"): float(v) for k, v in (a.get("max_days") or {}).items()},
        rework_limit=int(a.get("rework_limit", d.rework_limit)),
        rework_status=known(a.get("rework_status", d.rework_status), "activity.rework_status"),
        holidays=holidays,
        activity_tab=_text(tabs.get("activity", d.activity_tab), "activity.tabs.activity"),
        summary_tab=_text(tabs.get("summary", d.summary_tab), "activity.tabs.summary"),
        dashboard_tab=_text(tabs.get("dashboard", d.dashboard_tab), "activity.tabs.dashboard"),
    )
```

Di `load_config`, tambahkan argumen terakhir pada `return Config(...)`, setelah `timezone=...`:
```python
        activity=_activity(raw.get("activity"), statuses),
```

- [ ] **Step 4: Tambahkan blok di akhir `config.example.yaml`**

```yaml

# ---------------------------------------------------------------------------
# Developer Activity Dashboard: siapa mengerjakan issue apa dan sudah berapa
# lama. Dibaca dari GitLab (assignee, label status::*, komentar, MR terkait),
# ditulis ke tab "Developer Activity" + "Developer Summary", dan diringkas di
# tab "Dashboard". Durasi dalam hari kerja (Senin-Jumat, timezone di atas).
# ---------------------------------------------------------------------------
activity:
  enabled: true
  active_statuses: [On-Progress, Testing, Re-work, Ready to Deploy]
  idle_days: 2                       # IDLE  : tidak ada aktivitas >= N hari kerja
  max_days:                          # LAMA  : terlalu lama di status tertentu
    On-Progress: 5
    Testing: 3
    Re-work: 2
  rework_limit: 2                    # REWORK: masuk Re-work >= N kali
  rework_status: Re-work
  holidays:                          # tanggal merah (YYYY-MM-DD), tidak dihitung
    - 2026-12-25
  tabs:
    activity: Developer Activity
    summary: Developer Summary
    dashboard: Dashboard
```

- [ ] **Step 5: Jalankan semua test**

Run: `.venv/bin/python -m pytest -q tests`
Expected: semua PASS, termasuk 3 test `activity_config`

- [ ] **Step 6: Commit**

```bash
git add issue_log_agent/config.py config.example.yaml tests/test_sync.py
git commit -m "Add activity config block for developer activity dashboard"
```

---

### Task 3: GitLab read API dengan pagination

**Files:**
- Modify: `issue_log_agent/gitlab_client.py` (method baru setelah `_req`)
- Test: `tests/test_gitlab_client.py`

**Interfaces:**
- Produces (di `GitLabClient`):
  - `list_issues(labels: list[str], state: str = "opened") -> list[dict]`
  - `label_events(iid: int) -> list[dict]`, berisi item `{"created_at", "action": "add"|"remove", "label": {"name"} | None}`
  - `notes(iid: int) -> list[dict]`, hanya komentar non-system
  - `related_merge_requests(iid: int) -> list[dict]`
- Semua method melempar `GitLabError` untuk status ≥ 400.

- [ ] **Step 1: Tulis test yang gagal**

`tests/test_gitlab_client.py`:
```python
import pytest

from issue_log_agent.gitlab_client import GitLabClient, GitLabError


class FakeResp:
    def __init__(self, data, next_page="", status=200):
        self._data, self.status_code, self.text = data, status, str(data)
        self.headers = {"X-Next-Page": next_page}

    def json(self):
        return self._data


class FakeSession:
    def __init__(self, pages):
        self.pages, self.calls = pages, []

    def get(self, url, params=None, timeout=None):
        self.calls.append((url, dict(params)))
        return self.pages[params["page"]]


def client(pages):
    c = GitLabClient("https://gitlab.example", "group/p", "t")
    c.session = FakeSession(pages)
    return c


def test_get_all_follows_pagination():
    c = client({1: FakeResp([{"iid": 1}], "2"), 2: FakeResp([{"iid": 2}])})
    assert [i["iid"] for i in c.list_issues(["backlog"])] == [1, 2]
    url, params = c.session.calls[0]
    assert url == "https://gitlab.example/api/v4/projects/group%2Fp/issues"
    assert params == {"labels": "backlog", "state": "opened", "per_page": 100, "page": 1}
    assert c.session.calls[1][1]["page"] == 2


def test_notes_drop_system_notes():
    c = client({1: FakeResp([{"id": 1, "system": True}, {"id": 2, "system": False}])})
    assert [n["id"] for n in c.notes(12)] == [2]
    assert c.session.calls[0][0].endswith("/issues/12/notes")


def test_label_events_and_related_mrs_paths():
    c = client({1: FakeResp([])})
    c.label_events(5)
    c.related_merge_requests(5)
    assert c.session.calls[0][0].endswith("/issues/5/resource_label_events")
    assert c.session.calls[1][0].endswith("/issues/5/related_merge_requests")


def test_error_status_raises():
    c = client({1: FakeResp({"message": "403 Forbidden"}, status=403)})
    with pytest.raises(GitLabError, match="403"):
        c.list_issues(["backlog"])
```

- [ ] **Step 2: Jalankan test, pastikan gagal**

Run: `.venv/bin/python -m pytest -q tests/test_gitlab_client.py`
Expected: FAIL, `AttributeError: 'GitLabClient' object has no attribute 'list_issues'`

- [ ] **Step 3: Implementasi**

Tambahkan di `GitLabClient`, setelah method `_req`:
```python
    def _get_all(self, path: str, params: dict[str, Any] | None = None) -> list[dict]:
        """GET semua halaman (per_page=100, ikuti header X-Next-Page)."""
        params = {**(params or {}), "per_page": 100, "page": 1}
        out: list[dict] = []
        while True:
            r = self.session.get(f"{self.base}{path}", params=params, timeout=self.timeout)
            if r.status_code >= 400:
                raise GitLabError(f"GET {path} -> {r.status_code}: {r.text[:300]}")
            out.extend(r.json() or [])
            nxt = r.headers.get("X-Next-Page")
            if not nxt:
                return out
            params["page"] = int(nxt)

    def list_issues(self, labels: list[str], state: str = "opened") -> list[dict]:
        return self._get_all("/issues", {"labels": ",".join(labels), "state": state})

    def label_events(self, iid: int) -> list[dict]:
        return self._get_all(f"/issues/{iid}/resource_label_events")

    def notes(self, iid: int) -> list[dict]:
        """Komentar manusia saja; system note (perubahan label, assign, ...) dibuang."""
        return [n for n in self._get_all(f"/issues/{iid}/notes") if not n.get("system")]

    def related_merge_requests(self, iid: int) -> list[dict]:
        return self._get_all(f"/issues/{iid}/related_merge_requests")
```

- [ ] **Step 4: Jalankan test, pastikan lulus**

Run: `.venv/bin/python -m pytest -q tests/test_gitlab_client.py`
Expected: 4 passed

- [ ] **Step 5: Commit**

```bash
git add issue_log_agent/gitlab_client.py tests/test_gitlab_client.py
git commit -m "Add paginated read-only GitLab calls for activity monitoring"
```

---

### Task 4: `activity.py` — model, `compute`, render tab

**Files:**
- Create: `issue_log_agent/activity.py`
- Create: `tests/activity_fakes.py`
- Test: `tests/test_activity.py`

**Interfaces:**
- Consumes: `business_days_between` (Task 1), `cfg.activity` (Task 2), `status_from_issue(cfg, issue) -> str` dari `sync.py`.
- Produces:
  - Konstanta: `UNASSIGNED = "(belum di-assign)"`, `FLAG_IDLE`, `FLAG_LONG`, `FLAG_REWORK`, `FLAG_NO_ASSIGNEE`, `FLAGS: list[str]` (urutan: IDLE, LAMA, REWORK, NO ASSIGNEE), `ACTIVITY_HEADERS: list[str]` (12 kolom, `Flag` di indeks 10 / kolom K, `URL` di indeks 11 / kolom L).
  - `parse_ts(value: str) -> datetime`
  - `IssueActivity` dengan `from_gitlab(issue, events, notes, mrs)`
  - `ActivityRow`, `DeveloperSummary`, `ActivityReport(rows, summary, skipped: dict[int, str])` dengan `.summary_line() -> str`
  - `compute(issues: list[IssueActivity], cfg: Config, now: datetime) -> ActivityReport`
  - `summary_headers(cfg) -> list[str]` (status columns mulai indeks 2)
  - `activity_values(report, cfg) -> list[list]`, `summary_values(report, cfg) -> list[list]`, `table_lines(report) -> Iterator[str]`

- [ ] **Step 1: Buat fake bersama**

`tests/activity_fakes.py`:
```python
"""Data GitLab palsu untuk test activity/dashboard. NOW = Jumat 16 Okt 2026 16:00 WIB."""

from datetime import datetime, timezone

from issue_log_agent.gitlab_client import GitLabError

NOW = datetime(2026, 10, 16, 9, 0, tzinfo=timezone.utc)
URL = "https://gitlab.example/p/-/issues/{}"


def ev(when, action, label):
    return {"created_at": when, "action": action, "label": {"name": label} if label else None}


def issue(iid, title, labels, assignees, updated_at="2026-10-01T02:00:00Z"):
    return {
        "iid": iid, "title": title, "web_url": URL.format(iid), "labels": ["backlog", *labels],
        "assignees": [{"name": n} for n in assignees], "updated_at": updated_at,
    }


ISSUES = [
    issue(12, "[FEATURE][FSD]-Upload dokumen", ["feature", "status::On-Progress"], ["Budi"]),
    issue(13, "[BUG][SIT]-Error HTTP 500", ["bug", "status::Re-work"], ["Budi", "Sari"]),
    issue(14, '=HYPERLINK("http://evil","x")', ["bug", "status::On-Progress"], []),
    issue(15, "Belum dikerjakan", ["feature"], ["Sari"]),
    issue(16, "Gagal diambil", ["bug", "status::Testing"], ["Sari"]),
]

EVENTS = {
    12: [
        ev("2026-10-08T01:00:00Z", "add", None),  # label yang sudah dihapus
        ev("2026-10-08T02:00:00Z", "add", "status::On-Progress"),
    ],
    13: [
        ev("2026-10-01T02:00:00Z", "add", "status::On-Progress"),
        ev("2026-10-05T02:00:00Z", "remove", "status::On-Progress"),
        ev("2026-10-05T02:00:00Z", "add", "status::Re-work"),
        ev("2026-10-07T02:00:00Z", "remove", "status::Re-work"),
        ev("2026-10-07T02:00:00Z", "add", "status::Testing"),
        ev("2026-10-13T02:00:00Z", "remove", "status::Testing"),
        ev("2026-10-13T02:00:00Z", "add", "status::Re-work"),
    ],
    14: [ev("2026-10-16T08:00:00+07:00", "add", "status::On-Progress")],
}
NOTES = {12: [{"system": False, "created_at": "2026-10-15T02:00:00Z", "updated_at": "2026-10-15T02:00:00Z"}]}
MRS = {13: [{"updated_at": "2026-10-12T02:00:00Z"}]}


class FakeGitLab:
    def __init__(self, fail_list=False):
        self.fail_list = fail_list
        self.detail_calls = []
        self.list_labels = None

    def list_issues(self, labels, state="opened"):
        if self.fail_list:
            raise GitLabError("GET /issues -> 503: Service Unavailable")
        self.list_labels = labels
        return ISSUES

    def label_events(self, iid):
        self.detail_calls.append(iid)
        if iid == 16:
            raise GitLabError("GET /issues/16/resource_label_events -> 500")
        return EVENTS.get(iid, [])

    def notes(self, iid):
        return NOTES.get(iid, [])

    def related_merge_requests(self, iid):
        return MRS.get(iid, [])


class FakeTab:
    _ids = iter(range(1000, 2000))

    def __init__(self, name):
        self.id = next(FakeTab._ids)
        self.name = name
        self.values = []  # isi terakhir dari update(..., "A1")
        self.cells = {}  # range -> values dari batch_update / update parsial
        self.cleared = 0
        self.size = None

    def clear(self):
        self.cleared += 1
        self.values = []

    def resize(self, rows=None, cols=None):
        self.size = (rows, cols)

    def update(self, values, range_name="A1", **kw):
        if range_name == "A1":
            self.values = values
        else:
            self.cells[range_name] = values

    def batch_update(self, data, **kw):
        for d in data:
            self.cells[d["range"]] = d["values"]


class FakeBook:
    def __init__(self, existing=()):
        self.tabs = {n: FakeTab(n) for n in existing}
        self.requests = []
        self.deleted = []

    def worksheet(self, name):
        return self.tabs[name]

    def ensure_worksheet(self, name, rows=100, cols=26, index=None):
        if name in self.tabs:
            return self.tabs[name], False
        self.tabs[name] = FakeTab(name)
        return self.tabs[name], True

    def delete_worksheet(self, name):
        if self.tabs.pop(name, None):
            self.deleted.append(name)

    def batch_update(self, body):
        self.requests.extend(body["requests"])
```

- [ ] **Step 2: Tulis test yang gagal**

`tests/test_activity.py`:
```python
from datetime import datetime, timezone
from pathlib import Path

import pytest

from activity_fakes import NOW, FakeGitLab, ISSUES, EVENTS, NOTES, MRS
from issue_log_agent.activity import (
    ACTIVITY_HEADERS, UNASSIGNED, IssueActivity, activity_values, compute, summary_headers, summary_values,
)
from issue_log_agent.config import load_config

ROOT = Path(__file__).resolve().parents[1]
UTC = timezone.utc


@pytest.fixture
def cfg():
    return load_config(ROOT / "config.example.yaml", require_token=False)


def active_issues():
    by_iid = {i["iid"]: i for i in ISSUES}
    return [
        IssueActivity.from_gitlab(by_iid[iid], EVENTS.get(iid, []), NOTES.get(iid, []), MRS.get(iid, []))
        for iid in (12, 13, 14)
    ]


def rows_by_key(report):
    return {(r.developer, r.iid): r for r in report.rows}


def test_rows_per_developer_sorted(cfg):
    report = compute(active_issues(), cfg, NOW)
    assert [(r.developer, r.iid) for r in report.rows] == [
        ("Budi", 12), ("Budi", 13), ("Sari", 13), (UNASSIGNED, 14),
    ]


def test_compute_durations_and_flags(cfg):
    rows = rows_by_key(compute(active_issues(), cfg, NOW))

    r = rows[("Budi", 12)]
    assert (r.status, r.type) == ("On-Progress", "FEATURE")
    assert r.since == datetime(2026, 10, 8, 2, tzinfo=UTC)
    assert (r.days_in_status, r.idle_days, r.rework) == (6.3, 1.3, 0)
    assert r.flags == ["LAMA"]

    r = rows[("Sari", 13)]
    assert (r.status, r.type) == ("Re-work", "BUG")
    # label Re-work dipasang ulang 13 Okt: durasi dihitung dari pemasangan terakhir
    assert r.since == datetime(2026, 10, 13, 2, tzinfo=UTC)
    assert (r.days_in_status, r.idle_days, r.rework) == (3.3, 3.3, 2)
    assert r.flags == ["IDLE", "LAMA", "REWORK"]

    r = rows[(UNASSIGNED, 14)]
    assert (r.days_in_status, r.idle_days) == (0.3, 0.3)
    assert r.flags == ["NO ASSIGNEE"]


def test_flag_thresholds_are_inclusive(cfg):
    cfg.activity.idle_days = 1.3
    cfg.activity.max_days["On-Progress"] = 6.4
    r = rows_by_key(compute(active_issues(), cfg, NOW))[("Budi", 12)]
    assert r.flags == ["IDLE"]


def test_two_status_labels_uses_config_order(cfg):
    raw = dict(ISSUES[0], labels=["backlog", "status::Testing", "status::On-Progress"])
    report = compute([IssueActivity.from_gitlab(raw, [], [], [])], cfg, NOW)
    assert report.rows[0].status == "On-Progress"  # On-Progress ada lebih dulu di statuses
    assert report.rows[0].since == datetime(2026, 10, 1, 2, tzinfo=UTC)  # fallback updated_at


def test_inactive_status_is_ignored(cfg):
    raw = next(i for i in ISSUES if i["iid"] == 15)
    assert compute([IssueActivity.from_gitlab(raw, [], [], [])], cfg, NOW).rows == []


def test_summary_per_developer(cfg):
    report = compute(active_issues(), cfg, NOW)
    assert summary_headers(cfg) == [
        "Developer", "Total Aktif", "On-Progress", "Testing", "Re-work", "Ready to Deploy",
        "Jumlah Flag", "Issue Terlama (hari)",
    ]
    assert summary_values(report, cfg)[1:] == [
        ["Budi", 2, 1, 0, 1, 0, 2, 6.3],
        ["Sari", 1, 0, 0, 1, 0, 1, 3.3],
        [UNASSIGNED, 1, 1, 0, 0, 0, 1, 0.3],
    ]
    assert report.summary_line() == "issues=3 developers=2 flagged=3 skipped=0"


def test_values_escape_formulas(cfg):
    values = activity_values(compute(active_issues(), cfg, NOW), cfg)
    assert values[0] == ACTIVITY_HEADERS
    budi12 = values[1]
    assert budi12[:6] == [
        "Budi", '=HYPERLINK("https://gitlab.example/p/-/issues/12","#12")', "[FEATURE][FSD]-Upload dokumen",
        "FEATURE", "On-Progress", "2026-10-08 09:00",
    ]
    assert budi12[6:] == [6.3, "2026-10-15 09:00", 1.3, 0, "LAMA", "https://gitlab.example/p/-/issues/12"]
    # judul dari GitLab yang diawali "=" tidak boleh dieksekusi sebagai formula
    assert values[4][2] == "'=HYPERLINK(\"http://evil\",\"x\")"
```

- [ ] **Step 3: Jalankan test, pastikan gagal**

Run: `.venv/bin/python -m pytest -q tests/test_activity.py`
Expected: FAIL, `ModuleNotFoundError: No module named 'issue_log_agent.activity'`

- [ ] **Step 4: Implementasi**

`issue_log_agent/activity.py`:
```python
"""Developer Activity: siapa mengerjakan issue apa, sejak kapan, dan issue mana yang macet.

    GitLab (issue open berlabel backlog dengan status aktif)
      -> collect : riwayat label, komentar, MR terkait
      -> compute : Hari di Status, Hari Idle, Re-work, Flag (logika murni)
      -> tab "Developer Activity" + "Developer Summary", diringkas di tab "Dashboard"
"""

from __future__ import annotations

from collections import defaultdict
from collections.abc import Iterator
from dataclasses import dataclass, field
from datetime import datetime
from zoneinfo import ZoneInfo

from .config import Config
from .sync import status_from_issue
from .workdays import business_days_between

UNASSIGNED = "(belum di-assign)"
FLAG_IDLE, FLAG_LONG, FLAG_REWORK, FLAG_NO_ASSIGNEE = "IDLE", "LAMA", "REWORK", "NO ASSIGNEE"
FLAGS = [FLAG_IDLE, FLAG_LONG, FLAG_REWORK, FLAG_NO_ASSIGNEE]

ACTIVITY_HEADERS = [
    "Developer", "Issue", "Title", "Type", "Status", "Since", "Hari di Status",
    "Last Activity", "Hari Idle", "Re-work", "Flag", "URL",
]


def parse_ts(value: str) -> datetime:
    return datetime.fromisoformat(value.replace("Z", "+00:00"))


@dataclass
class IssueActivity:
    iid: int
    title: str
    url: str
    labels: list[str]
    assignees: list[str]
    updated_at: datetime
    label_events: list[dict] = field(default_factory=list)
    note_times: list[datetime] = field(default_factory=list)
    mr_times: list[datetime] = field(default_factory=list)

    @classmethod
    def from_gitlab(cls, issue: dict, events: list[dict], notes: list[dict], mrs: list[dict]) -> IssueActivity:
        return cls(
            iid=issue["iid"],
            title=issue.get("title") or "",
            url=issue.get("web_url") or "",
            labels=list(issue.get("labels") or []),
            assignees=[a.get("name") or a.get("username") or "" for a in issue.get("assignees") or []],
            updated_at=parse_ts(issue["updated_at"]),
            label_events=list(events),
            note_times=[parse_ts(n.get("updated_at") or n["created_at"]) for n in notes],
            mr_times=[parse_ts(m["updated_at"]) for m in mrs if m.get("updated_at")],
        )


@dataclass
class ActivityRow:
    developer: str
    iid: int
    title: str
    type: str
    status: str
    since: datetime
    days_in_status: float
    last_activity: datetime
    idle_days: float
    rework: int
    flags: list[str]
    url: str


@dataclass
class DeveloperSummary:
    developer: str
    total: int
    by_status: dict[str, int]
    flagged: int
    longest_days: float


@dataclass
class ActivityReport:
    rows: list[ActivityRow]
    summary: list[DeveloperSummary]
    skipped: dict[int, str] = field(default_factory=dict)  # iid -> alasan

    def summary_line(self) -> str:
        issues = len({r.iid for r in self.rows})
        devs = sum(1 for s in self.summary if s.developer != UNASSIGNED)
        flagged = len({r.iid for r in self.rows if r.flags})
        return f"issues={issues} developers={devs} flagged={flagged} skipped={len(self.skipped)}"


def _label(event: dict) -> str:
    # label yang sudah dihapus dari project muncul sebagai "label": null
    return (event.get("label") or {}).get("name", "")


def compute(issues: list[IssueActivity], cfg: Config, now: datetime) -> ActivityReport:
    a = cfg.activity
    active = {s.lower() for s in a.active_statuses}
    type_of = {label: t.name for t in cfg.types for label in t.labels}
    rework_rule = cfg.statuses.get(a.rework_status)
    rework_labels = set(rework_rule.labels) if rework_rule else set()

    def days(start: datetime) -> float:
        return business_days_between(start, now, cfg.timezone, a.holidays)

    rows: list[ActivityRow] = []
    for issue in issues:
        status = status_from_issue(cfg, {"state": "opened", "labels": issue.labels})
        if status.lower() not in active:
            continue
        status_labels = set(cfg.statuses[status].labels)
        adds = [e for e in issue.label_events if e.get("action") == "add"]
        since = max(
            (parse_ts(e["created_at"]) for e in adds if _label(e) in status_labels),
            default=issue.updated_at,
        )
        last = max([since, *issue.note_times, *issue.mr_times, *(parse_ts(e["created_at"]) for e in issue.label_events)])
        rework = sum(1 for e in adds if _label(e) in rework_labels)
        in_status, idle = days(since), days(last)

        flags: list[str] = []
        if idle >= a.idle_days:
            flags.append(FLAG_IDLE)
        limit = a.max_days.get(status)
        if limit is not None and in_status >= limit:
            flags.append(FLAG_LONG)
        if rework >= a.rework_limit:
            flags.append(FLAG_REWORK)
        if not issue.assignees:
            flags.append(FLAG_NO_ASSIGNEE)

        itype = next((type_of[l] for l in issue.labels if l in type_of), "")
        for dev in issue.assignees or [UNASSIGNED]:
            rows.append(ActivityRow(
                developer=dev, iid=issue.iid, title=issue.title, type=itype, status=status,
                since=since, days_in_status=in_status, last_activity=last, idle_days=idle,
                rework=rework, flags=list(flags), url=issue.url,
            ))

    rows.sort(key=lambda r: (r.developer == UNASSIGNED, r.developer.lower(), -r.days_in_status, r.iid))
    return ActivityReport(rows=rows, summary=_summarize(rows, a.active_statuses))


def _summarize(rows: list[ActivityRow], statuses: list[str]) -> list[DeveloperSummary]:
    by_dev: dict[str, list[ActivityRow]] = defaultdict(list)
    for r in rows:
        by_dev[r.developer].append(r)
    out = [
        DeveloperSummary(
            developer=dev,
            total=len(rs),
            by_status={s: sum(1 for r in rs if r.status == s) for s in statuses},
            flagged=sum(1 for r in rs if r.flags),
            longest_days=max(r.days_in_status for r in rs),
        )
        for dev, rs in by_dev.items()
    ]
    out.sort(key=lambda s: (-s.total, s.developer == UNASSIGNED, s.developer.lower()))
    return out


def _cell(text: str) -> str:
    """Teks dari GitLab ditampilkan apa adanya, tidak dijalankan sebagai formula Sheets."""
    return "'" + text if text[:1] in ("=", "+", "-", "@") else text


def _fmt(dt: datetime, tz: str) -> str:
    return dt.astimezone(ZoneInfo(tz)).strftime("%Y-%m-%d %H:%M")


def activity_values(report: ActivityReport, cfg: Config) -> list[list]:
    out: list[list] = [list(ACTIVITY_HEADERS)]
    for r in report.rows:
        link = r.url.replace('"', '""')
        out.append([
            _cell(r.developer), f'=HYPERLINK("{link}","#{r.iid}")', _cell(r.title), r.type, r.status,
            _fmt(r.since, cfg.timezone), r.days_in_status, _fmt(r.last_activity, cfg.timezone),
            r.idle_days, r.rework, ", ".join(r.flags), _cell(r.url),
        ])
    return out


def summary_headers(cfg: Config) -> list[str]:
    return ["Developer", "Total Aktif", *cfg.activity.active_statuses, "Jumlah Flag", "Issue Terlama (hari)"]


def summary_values(report: ActivityReport, cfg: Config) -> list[list]:
    out: list[list] = [summary_headers(cfg)]
    for s in report.summary:
        out.append([
            _cell(s.developer), s.total, *(s.by_status[st] for st in cfg.activity.active_statuses),
            s.flagged, s.longest_days,
        ])
    return out


def table_lines(report: ActivityReport) -> Iterator[str]:
    yield f"{'Developer':<20} {'Issue':<7} {'Status':<16} {'Hari':>5} {'Idle':>5}  Flag"
    for r in report.rows:
        yield (f"{r.developer[:20]:<20} #{r.iid:<6} {r.status:<16} {r.days_in_status:>5} "
               f"{r.idle_days:>5}  {', '.join(r.flags)}")
```

- [ ] **Step 5: Jalankan test, pastikan lulus**

Run: `.venv/bin/python -m pytest -q tests/test_activity.py`
Expected: 7 passed

- [ ] **Step 6: Commit**

```bash
git add issue_log_agent/activity.py tests/activity_fakes.py tests/test_activity.py
git commit -m "Compute developer activity rows, flags and summary from GitLab data"
```

---

### Task 5: Dashboard (`dashboard.py`) + Sheets helper

**Files:**
- Create: `issue_log_agent/dashboard.py`
- Modify: `issue_log_agent/sheet.py` (method baru di `GoogleSpreadsheet`)
- Test: `tests/test_dashboard.py`

**Interfaces:**
- Consumes: `ACTIVITY_HEADERS`, `FLAGS`, `UNASSIGNED`, `summary_headers` (Task 4), `cfg.activity` (Task 2).
- Produces:
  - `dashboard.ensure(book, cfg, rebuild: bool = False) -> bool`. Return `True` kalau Dashboard baru dibuat. Tab data dan Dashboard dibuat kalau belum ada. Conditional formatting di tab Activity hanya ditambahkan saat tab itu baru dibuat.
  - `dashboard.stamp(book, cfg, now: datetime) -> None`, untuk menulis sel `B2`.
  - Kontrak `book`: `ensure_worksheet(name, rows, cols, index=None) -> (ws, created)`, `delete_worksheet(name)`, `batch_update(body)`, `worksheet(name)`. Kontrak `ws`: `.id`, `clear()`, `resize(rows, cols)`, `update(values, range_name, **kw)`, `batch_update(data, **kw)`.
  - `GoogleSpreadsheet.ensure_worksheet / delete_worksheet / batch_update`.

- [ ] **Step 1: Tulis test yang gagal**

`tests/test_dashboard.py`:
```python
from pathlib import Path

import pytest

from activity_fakes import NOW, FakeBook
from issue_log_agent import dashboard
from issue_log_agent.config import load_config

ROOT = Path(__file__).resolve().parents[1]


@pytest.fixture
def cfg():
    return load_config(ROOT / "config.example.yaml", require_token=False)


def kinds(requests):
    return [next(iter(r)) for r in requests]


def cf_sheets(requests):
    return [r["addConditionalFormatRule"]["rule"]["ranges"][0]["sheetId"]
            for r in requests if "addConditionalFormatRule" in r]


def test_creates_tabs_layout_chart_and_rules(cfg):
    book = FakeBook(["FEATURE"])
    assert dashboard.ensure(book, cfg) is True
    act, summ, dash = (book.tabs[n] for n in ("Developer Activity", "Developer Summary", "Dashboard"))

    assert dash.cells["A4"][0] == ["Issue Aktif", "Developer Aktif", "IDLE", "LAMA", "REWORK", "NO ASSIGNEE"]
    assert dash.cells["A4"][1][2] == "=COUNTIF('Developer Activity'!K2:K,\"*IDLE*\")"
    assert "QUERY('Developer Activity'!A1:L" in dash.cells["A24"][0][0]
    assert "where K <> ''" in dash.cells["A24"][0][0]
    assert "QUERY('Developer Activity'!A1:L" in dash.cells["J24"][0][0]

    chart = next(r["addChart"]["chart"] for r in book.requests if "addChart" in r)
    basic = chart["spec"]["basicChart"]
    assert (basic["chartType"], basic["stackedType"]) == ("BAR", "STACKED")
    assert basic["domains"][0]["domain"]["sourceRange"]["sources"][0]["sheetId"] == summ.id
    assert [s["series"]["sourceRange"]["sources"][0]["startColumnIndex"] for s in basic["series"]] == [2, 3, 4, 5]
    assert chart["position"]["overlayPosition"]["anchorCell"]["sheetId"] == dash.id

    # 4 aturan warna untuk setiap tabel di Dashboard (2 tabel) + kolom Flag di tab Activity
    assert sorted(cf_sheets(book.requests)) == sorted([dash.id] * 8 + [act.id] * 4)


def test_existing_dashboard_is_left_alone(cfg):
    book = FakeBook()
    dashboard.ensure(book, cfg)
    book.requests.clear()
    assert dashboard.ensure(book, cfg) is False
    assert book.requests == []


def test_recreate_dashboard_without_duplicate_activity_rules(cfg):
    book = FakeBook()
    dashboard.ensure(book, cfg)
    act = book.tabs["Developer Activity"]
    book.delete_worksheet("Dashboard")  # dihapus manual oleh user
    book.requests.clear()
    assert dashboard.ensure(book, cfg) is True
    assert act.id not in cf_sheets(book.requests)
    assert "addChart" in kinds(book.requests)


def test_rebuild_recreates_all_tabs(cfg):
    book = FakeBook()
    dashboard.ensure(book, cfg)
    old_ids = {n: t.id for n, t in book.tabs.items()}
    book.requests.clear()
    assert dashboard.ensure(book, cfg, rebuild=True) is True
    assert sorted(book.deleted) == ["Dashboard", "Developer Activity", "Developer Summary"]
    assert all(book.tabs[n].id != i for n, i in old_ids.items())
    assert cf_sheets(book.requests).count(book.tabs["Developer Activity"].id) == 4


def test_sheet_names_are_quoted(cfg):
    cfg.activity.activity_tab = "Dev's Activity"
    book = FakeBook()
    dashboard.ensure(book, cfg)
    assert "'Dev''s Activity'!K2:K" in book.tabs["Dashboard"].cells["A4"][1][2]


def test_stamp_writes_local_time(cfg):
    book = FakeBook()
    dashboard.ensure(book, cfg)
    dashboard.stamp(book, cfg, NOW)
    assert book.tabs["Dashboard"].cells["B2"] == [["2026-10-16 16:00"]]
```

- [ ] **Step 2: Jalankan test, pastikan gagal**

Run: `.venv/bin/python -m pytest -q tests/test_dashboard.py`
Expected: FAIL, `ImportError: cannot import name 'dashboard'`

- [ ] **Step 3: Implementasi `dashboard.py`**

`issue_log_agent/dashboard.py`:
```python
"""Tab "Dashboard" di Google Sheets: KPI, grafik beban per developer, dan tabel issue.

Dashboard dibuat sekali saja (atau ulang dengan --rebuild-dashboard). Isinya formula yang
membaca tab "Developer Activity" / "Developer Summary", jadi setiap run cukup menulis ulang
tab data dan sel "Diperbarui".
"""

from __future__ import annotations

from datetime import datetime
from zoneinfo import ZoneInfo

from .activity import ACTIVITY_HEADERS, FLAGS, UNASSIGNED, summary_headers
from .config import Config

FLAG_COLORS = {
    "IDLE": (0.96, 0.78, 0.78),
    "LAMA": (0.99, 0.85, 0.68),
    "REWORK": (1.0, 0.95, 0.70),
    "NO ASSIGNEE": (0.85, 0.85, 0.85),
}
FLAG_COL = ACTIVITY_HEADERS.index("Flag")  # K
CHART_ANCHOR_ROW = 6  # 0-based: chart mulai di baris 7
TABLE_ROW = 24  # 1-based: header tabel QUERY; judul tabel di baris 23
ATTENTION_COL, ATTENTION_FLAG_COL = "A", 6  # select A,B,C,E,G,I,K,L -> Flag di kolom ke-7 (G)
ALL_COL, ALL_FLAG_COL = "J", 16  # mulai J; select A,B,C,E,G,I,J,K -> Flag di kolom ke-8 (Q)


def _q(name: str) -> str:
    return "'" + name.replace("'", "''") + "'"


def _layout(cfg: Config) -> list[dict]:
    a = cfg.activity
    act, summ = _q(a.activity_tab), _q(a.summary_tab)
    kpis = [
        ("Issue Aktif", f"=IFERROR(COUNTUNIQUE({act}!L2:L),0)"),
        ("Developer Aktif", f'=COUNTIFS({summ}!A2:A,"<>",{summ}!A2:A,"<>{UNASSIGNED}")'),
        *[(flag, f'=COUNTIF({act}!K2:K,"*{flag}*")') for flag in FLAGS],
    ]
    attention = (f'=IFERROR(QUERY({act}!A1:L,"select A,B,C,E,G,I,K,L where K <> \'\' order by I desc",1),'
                 f'"Tidak ada issue yang perlu perhatian")')
    everything = (f'=IFERROR(QUERY({act}!A1:L,"select A,B,C,E,G,I,J,K order by A, G desc",1),'
                  f'"Belum ada issue aktif")')
    return [
        {"range": "A1", "values": [["Developer Activity Dashboard"]]},
        {"range": "A2", "values": [["Diperbarui:"]]},
        {"range": "A4", "values": [[k for k, _ in kpis], [f for _, f in kpis]]},
        {"range": f"{ATTENTION_COL}{TABLE_ROW - 1}", "values": [["Perlu Perhatian"]]},
        {"range": f"{ATTENTION_COL}{TABLE_ROW}", "values": [[attention]]},
        {"range": f"{ALL_COL}{TABLE_ROW - 1}", "values": [["Semua Issue Aktif"]]},
        {"range": f"{ALL_COL}{TABLE_ROW}", "values": [[everything]]},
    ]


def _text_format(sheet_id: int, row: int, col0: int, col1: int, size: int) -> dict:
    return {"repeatCell": {
        "range": {"sheetId": sheet_id, "startRowIndex": row, "endRowIndex": row + 1,
                  "startColumnIndex": col0, "endColumnIndex": col1},
        "cell": {"userEnteredFormat": {"textFormat": {"bold": True, "fontSize": size}}},
        "fields": "userEnteredFormat.textFormat",
    }}


def _formats(dash_id: int) -> list[dict]:
    return [
        _text_format(dash_id, 0, 0, 1, 16),  # judul
        _text_format(dash_id, 3, 0, 6, 10),  # label KPI
        _text_format(dash_id, 4, 0, 6, 18),  # angka KPI
        _text_format(dash_id, TABLE_ROW - 2, 0, 20, 12),  # judul tabel
    ]


def _flag_rules(sheet_id: int, col: int, start_row: int) -> list[dict]:
    rules = []
    for i, (flag, (r, g, b)) in enumerate(FLAG_COLORS.items()):
        rules.append({"addConditionalFormatRule": {"index": i, "rule": {
            "ranges": [{"sheetId": sheet_id, "startRowIndex": start_row,
                        "startColumnIndex": col, "endColumnIndex": col + 1}],
            "booleanRule": {
                "condition": {"type": "TEXT_CONTAINS", "values": [{"userEnteredValue": flag}]},
                "format": {"backgroundColor": {"red": r, "green": g, "blue": b}},
            },
        }}})
    return rules


def _chart(cfg: Config, dash_id: int, summary_id: int) -> dict:
    def col(c: int) -> dict:
        return {"sourceRange": {"sources": [{"sheetId": summary_id, "startColumnIndex": c, "endColumnIndex": c + 1}]}}

    status_cols = range(2, 2 + len(cfg.activity.active_statuses))
    return {"addChart": {"chart": {
        "spec": {
            "title": "Beban per Developer",
            "basicChart": {
                "chartType": "BAR",
                "stackedType": "STACKED",
                "legendPosition": "BOTTOM_LEGEND",
                "headerCount": 1,
                "axis": [{"position": "BOTTOM_AXIS", "title": "Issue aktif"}],
                "domains": [{"domain": col(0)}],
                "series": [{"series": col(c), "targetAxis": "BOTTOM_AXIS"} for c in status_cols],
            },
        },
        "position": {"overlayPosition": {
            "anchorCell": {"sheetId": dash_id, "rowIndex": CHART_ANCHOR_ROW, "columnIndex": 0},
            "widthPixels": 720, "heightPixels": 320,
        }},
    }}}


def ensure(book, cfg: Config, rebuild: bool = False) -> bool:
    """Siapkan tab data + Dashboard. Return True jika Dashboard baru dibuat."""
    a = cfg.activity
    if rebuild:
        for name in (a.dashboard_tab, a.activity_tab, a.summary_tab):
            book.delete_worksheet(name)
    act, act_new = book.ensure_worksheet(a.activity_tab, rows=200, cols=len(ACTIVITY_HEADERS))
    summ, _ = book.ensure_worksheet(a.summary_tab, rows=100, cols=len(summary_headers(cfg)))
    dash, dash_new = book.ensure_worksheet(a.dashboard_tab, rows=200, cols=26, index=0)

    requests: list[dict] = []
    if act_new:  # aturan warna tab Activity hanya sekali, supaya tidak dobel
        requests += _flag_rules(act.id, FLAG_COL, 1)
    if dash_new:
        dash.batch_update(_layout(cfg), value_input_option="USER_ENTERED")
        requests += [
            *_formats(dash.id),
            _chart(cfg, dash.id, summ.id),
            *_flag_rules(dash.id, ATTENTION_FLAG_COL, TABLE_ROW),
            *_flag_rules(dash.id, ALL_FLAG_COL, TABLE_ROW),
        ]
    if requests:
        book.batch_update({"requests": requests})
    return dash_new


def stamp(book, cfg: Config, now: datetime) -> None:
    ts = now.astimezone(ZoneInfo(cfg.timezone)).strftime("%Y-%m-%d %H:%M")
    book.worksheet(cfg.activity.dashboard_tab).update([[ts]], "B2", value_input_option="USER_ENTERED")
```

- [ ] **Step 4: Tambahkan helper di `GoogleSpreadsheet` (`sheet.py`)**

Tambahkan setelah method `worksheet` di `class GoogleSpreadsheet`:
```python
    def ensure_worksheet(self, name: str, rows: int = 100, cols: int = 26, index: int | None = None):
        """Return (worksheet, created)."""
        import gspread

        try:
            return self._book.worksheet(name), False
        except gspread.WorksheetNotFound:
            return self._book.add_worksheet(title=name, rows=rows, cols=cols, index=index), True

    def delete_worksheet(self, name: str) -> None:
        import gspread

        try:
            self._book.del_worksheet(self._book.worksheet(name))
        except gspread.WorksheetNotFound:
            pass

    def batch_update(self, body: dict) -> object:
        """Spreadsheet-level batchUpdate (chart, conditional formatting, format sel)."""
        return self._book.batch_update(body)
```

- [ ] **Step 5: Jalankan test, pastikan lulus**

Run: `.venv/bin/python -m pytest -q tests/test_dashboard.py`
Expected: 6 passed

- [ ] **Step 6: Commit**

```bash
git add issue_log_agent/dashboard.py issue_log_agent/sheet.py tests/test_dashboard.py
git commit -m "Build Google Sheets dashboard tab for developer activity"
```

---

### Task 6: `collect` + `ActivityMonitor`

**Files:**
- Modify: `issue_log_agent/activity.py` (tambahkan di akhir file, plus import)
- Test: `tests/test_activity.py` (tambah di akhir file)

**Interfaces:**
- Consumes: method GitLab dari Task 3, `dashboard.ensure/stamp` (Task 5), `compute` dan render (Task 4).
- Produces:
  - `collect(gitlab, cfg) -> tuple[list[IssueActivity], dict[int, str]]`. Error dari `list_issues` diteruskan ke pemanggil. Error per issue dimasukkan ke dict `skipped`.
  - `write_tab(ws, values: list[list]) -> None`
  - `ActivityMonitor(cfg, gitlab, *, dry_run=False, rebuild_dashboard=False, now=None).run(book) -> ActivityReport`

- [ ] **Step 1: Tulis test yang gagal**

Tambahkan di akhir `tests/test_activity.py`:
```python
import logging

from activity_fakes import FakeBook
from issue_log_agent.activity import ActivityMonitor, collect
from issue_log_agent.gitlab_client import GitLabError


def test_collect_skips_inactive_and_failed(cfg):
    gl = FakeGitLab()
    issues, skipped = collect(gl, cfg)
    assert [i.iid for i in issues] == [12, 13, 14]
    assert gl.list_labels == ["backlog"]
    assert 15 not in gl.detail_calls  # status Open: detail tidak diambil
    assert list(skipped) == [16] and "500" in skipped[16]


def test_monitor_writes_tabs_and_stamp(cfg):
    book = FakeBook(["FEATURE"])
    report = ActivityMonitor(cfg, FakeGitLab(), now=NOW).run(book)
    act, summ = book.tabs["Developer Activity"], book.tabs["Developer Summary"]
    assert act.values[0] == ACTIVITY_HEADERS and len(act.values) == 5
    assert act.cleared == 1 and act.size == (100, 12)
    assert summ.values[1][0] == "Budi"
    assert book.tabs["Dashboard"].cells["B2"] == [["2026-10-16 16:00"]]
    assert list(report.skipped) == [16]


def test_monitor_list_failure_leaves_tabs_untouched(cfg):
    book = FakeBook(["Developer Activity"])
    book.tabs["Developer Activity"].values = [["data lama"]]
    with pytest.raises(GitLabError):
        ActivityMonitor(cfg, FakeGitLab(fail_list=True), now=NOW).run(book)
    assert book.tabs["Developer Activity"].values == [["data lama"]]
    assert book.tabs["Developer Activity"].cleared == 0
    assert "Dashboard" not in book.tabs and book.requests == []


def test_dry_run_only_logs(cfg, caplog):
    book = FakeBook()
    with caplog.at_level(logging.INFO):
        ActivityMonitor(cfg, FakeGitLab(), dry_run=True, now=NOW).run(book)
    assert book.tabs == {}
    assert any("Budi" in m and "LAMA" in m for m in caplog.messages)
```

- [ ] **Step 2: Jalankan test, pastikan gagal**

Run: `.venv/bin/python -m pytest -q tests/test_activity.py`
Expected: FAIL, `ImportError: cannot import name 'ActivityMonitor'`

- [ ] **Step 3: Implementasi**

Di `activity.py`, tambahkan import berikut ke blok import:
```python
import logging

import requests

from .gitlab_client import GitLabError
```
dan setelah blok import:
```python
log = logging.getLogger(__name__)
```

Tambahkan di akhir file:
```python
def collect(gitlab, cfg: Config) -> tuple[list[IssueActivity], dict[int, str]]:
    """Ambil issue aktif + detailnya. Gagal list_issues -> exception; gagal per issue -> skipped."""
    active = {s.lower() for s in cfg.activity.active_statuses}
    issues: list[IssueActivity] = []
    skipped: dict[int, str] = {}
    for raw in gitlab.list_issues(cfg.default_labels):
        if status_from_issue(cfg, raw).lower() not in active:
            continue
        iid = raw["iid"]
        try:
            issues.append(IssueActivity.from_gitlab(
                raw, gitlab.label_events(iid), gitlab.notes(iid), gitlab.related_merge_requests(iid)
            ))
        except (GitLabError, requests.RequestException, KeyError, ValueError) as e:
            log.warning("Developer Activity: issue #%s dilewati: %s", iid, e)
            skipped[iid] = str(e)
    return issues, skipped


def write_tab(ws, values: list[list]) -> None:
    """Tulis ulang seluruh tab supaya tidak ada baris lama yang tersisa."""
    ws.clear()
    ws.resize(rows=max(len(values) + 50, 100), cols=len(values[0]))
    ws.update(values, "A1", value_input_option="USER_ENTERED")


class ActivityMonitor:
    def __init__(self, cfg: Config, gitlab, *, dry_run: bool = False, rebuild_dashboard: bool = False,
                 now: datetime | None = None):
        self.cfg = cfg
        self.gitlab = gitlab
        self.dry_run = dry_run
        self.rebuild_dashboard = rebuild_dashboard
        self.now = now or datetime.now(ZoneInfo(cfg.timezone))

    def run(self, book) -> ActivityReport:
        # import di sini: dashboard.py memakai konstanta dari modul ini
        from . import dashboard

        issues, skipped = collect(self.gitlab, self.cfg)  # gagal di sini -> tab tidak disentuh
        report = compute(issues, self.cfg, self.now)
        report.skipped.update(skipped)
        if self.dry_run:
            for line in table_lines(report):
                log.info(line)
            return report

        a = self.cfg.activity
        dashboard.ensure(book, self.cfg, rebuild=self.rebuild_dashboard)
        write_tab(book.worksheet(a.activity_tab), activity_values(report, self.cfg))
        write_tab(book.worksheet(a.summary_tab), summary_values(report, self.cfg))
        dashboard.stamp(book, self.cfg, self.now)
        return report
```

- [ ] **Step 4: Jalankan semua test**

Run: `.venv/bin/python -m pytest -q tests`
Expected: semua PASS

- [ ] **Step 5: Commit**

```bash
git add issue_log_agent/activity.py tests/test_activity.py
git commit -m "Collect GitLab activity and write Developer Activity tabs"
```

---

### Task 7: CLI wiring + README

**Files:**
- Modify: `issue_log_agent/__main__.py`
- Modify: `README.md` (bagian baru sebelum `## Anti-duplikat`, plus baris di tabel `## Struktur`)
- Test: `tests/test_sync.py` (tambah di akhir file)

**Interfaces:**
- Consumes: `ActivityMonitor` (Task 6), `GitLabError`.
- Produces: `run_activity(cfg, gitlab, book, *, dry_run, rebuild) -> bool` di `__main__.py`. Return `True` artinya gagal (exit code 1). Flag CLI baru: `--no-activity`, `--activity-only`, `--rebuild-dashboard`.

- [ ] **Step 1: Tulis test yang gagal**

Tambahkan di akhir `tests/test_sync.py`:
```python
import logging

from activity_fakes import FakeBook, FakeGitLab as FakeActivityGitLab
from issue_log_agent.__main__ import run_activity


def test_run_activity_without_gitlab_is_skipped(cfg, caplog):
    book = FakeBook()
    with caplog.at_level(logging.INFO):
        assert run_activity(cfg, None, book, dry_run=True, rebuild=False) is False
    assert book.tabs == {}
    assert any("dilewati" in m for m in caplog.messages)


def test_run_activity_gitlab_down_reports_failure(cfg):
    book = FakeBook()
    assert run_activity(cfg, FakeActivityGitLab(fail_list=True), book, dry_run=False, rebuild=False) is True
    assert book.tabs == {}


def test_run_activity_writes_dashboard(cfg):
    book = FakeBook()
    assert run_activity(cfg, FakeActivityGitLab(), book, dry_run=False, rebuild=False) is False
    assert "Dashboard" in book.tabs
```

- [ ] **Step 2: Jalankan test, pastikan gagal**

Run: `.venv/bin/python -m pytest -q tests/test_sync.py -k run_activity`
Expected: FAIL, `ImportError: cannot import name 'run_activity'`

- [ ] **Step 3: Implementasi di `__main__.py`**

Ubah docstring baris 1 menjadi:
```python
"""CLI entry point: python -m issue_log_agent --config config.yaml [--dry-run] [--activity-only]"""
```

Tambahkan import:
```python
import requests

from .activity import ActivityMonitor
from .gitlab_client import GitLabClient, GitLabError
```
(ganti baris `from .gitlab_client import GitLabClient` yang lama.)

Tambahkan sebelum `def main`:
```python
def run_activity(cfg, gitlab, book, *, dry_run: bool, rebuild: bool) -> bool:
    """Perbarui Developer Activity + Dashboard. Return True jika gagal."""
    if gitlab is None:
        logging.info("Developer Activity dilewati: GitLab token belum di-set")
        return False
    try:
        report = ActivityMonitor(cfg, gitlab, dry_run=dry_run, rebuild_dashboard=rebuild).run(book)
    except (GitLabError, requests.RequestException) as e:
        logging.error("Developer Activity gagal, tab tidak diubah: %s", e)
        return True
    logging.info("Developer Activity: %s", report.summary_line())
    return False


def _gitlab(cfg) -> GitLabClient:
    return GitLabClient(cfg.gitlab.url, cfg.gitlab.project, cfg.gitlab.token, verify_ssl=cfg.gitlab.verify_ssl)
```

Tambahkan argumen setelah `--watch`:
```python
    p.add_argument("--no-activity", action="store_true", help="Lewati update Developer Activity + Dashboard")
    p.add_argument("--activity-only", action="store_true", help="Hanya update Developer Activity + Dashboard")
    p.add_argument("--rebuild-dashboard", action="store_true",
                   help="Hapus lalu buat ulang tab Dashboard, Developer Activity, dan Developer Summary")
```

Ganti pembuatan `gitlab = None if args.dry_run else GitLabClient(...)` menjadi:
```python
    gitlab = None if args.dry_run else _gitlab(cfg)
    # Activity hanya membaca GitLab, jadi tetap jalan saat --dry-run bila token tersedia.
    reader = gitlab or (_gitlab(cfg) if cfg.gitlab.token else None)
    if args.activity_only and args.no_activity:
        logging.error("--activity-only dan --no-activity tidak bisa dipakai bersamaan")
        return 2
```

Ganti isi loop `while True:` menjadi:
```python
    while True:
        book = CsvSpreadsheet(args.csv_dir) if args.csv_dir else GoogleSpreadsheet(cfg)
        failed = False
        if not args.activity_only:
            report = Syncer(
                cfg, gitlab,
                assistant=assistant,
                dry_run=args.dry_run,
                status_sync=args.status_sync,
                only_rows=set(args.row) if args.row else None,
            ).run(book, types)
            logging.info("Selesai: %s", report.summary())
            failed = bool(report.failed)
        if cfg.activity.enabled and not args.no_activity:
            failed = run_activity(cfg, reader, book, dry_run=args.dry_run, rebuild=args.rebuild_dashboard) or failed
        if not args.watch:
            return 1 if failed else 0
        args.rebuild_dashboard = False  # rebuild hanya di iterasi pertama
        time.sleep(args.watch)
```

- [ ] **Step 4: Jalankan semua test**

Run: `.venv/bin/python -m pytest -q tests`
Expected: semua PASS

- [ ] **Step 5: Smoke test CLI offline**

Run: `env -u GITLAB_TOKEN .venv/bin/python -m issue_log_agent --config config.example.yaml --csv-dir examples -v; echo "exit=$?"`
Expected: log sync seperti biasa, diikuti `Developer Activity dilewati: GitLab token belum di-set`, lalu `exit=0`.

Run: `.venv/bin/python -m issue_log_agent --help | grep -E "activity|rebuild"`
Expected: muncul 3 flag baru.

- [ ] **Step 6: Update README**

Sisipkan sebelum `## Anti-duplikat`:
````markdown
## Developer Activity Dashboard

Setiap run (setelah sync backlog), agent membaca GitLab lalu memperbarui tiga tab di spreadsheet
yang sama:

| Tab | Isi |
|---|---|
| **Dashboard** | KPI (Issue Aktif, Developer Aktif, IDLE, LAMA, REWORK, NO ASSIGNEE), grafik beban per developer, tabel *Perlu Perhatian* dan *Semua Issue Aktif* dengan warna flag |
| **Developer Activity** | satu baris per developer × issue: status, sejak kapan, hari di status, aktivitas terakhir, hari idle, jumlah Re-work, flag |
| **Developer Summary** | satu baris per developer: jumlah issue per status, jumlah flag, issue terlama |

Data diambil dari issue open berlabel `backlog` dengan status aktif (`activity.active_statuses`):
assignee, riwayat label `status::*`, komentar, dan MR terkait. Durasi dihitung dalam hari kerja
(Senin–Jumat, tanggal merah di `activity.holidays` tidak dihitung).

| Flag | Arti (threshold di `config.yaml` → `activity`) |
|---|---|
| `IDLE` | tidak ada komentar, update MR, atau perubahan label selama ≥ `idle_days` hari kerja |
| `LAMA` | sudah ≥ `max_days.<status>` hari kerja di status tersebut |
| `REWORK` | sudah masuk Re-work ≥ `rework_limit` kali |
| `NO ASSIGNEE` | status aktif tetapi belum ada assignee |

Tab Dashboard dibuat sekali, berisi formula yang membaca tab data. Jangan edit tab Developer
Activity / Developer Summary karena keduanya ditulis ulang setiap run.

```bash
python -m issue_log_agent --activity-only         # hanya refresh dashboard
python -m issue_log_agent --no-activity           # hanya sync backlog
python -m issue_log_agent --rebuild-dashboard     # buat ulang Dashboard (mis. setelah ubah active_statuses)
python -m issue_log_agent --dry-run --activity-only   # cetak tabel aktivitas ke log, tanpa menulis sheet
```

Kalau GitLab tidak bisa diakses, tab tidak diubah (data terakhir tetap ada) dan exit code = 1.
Dashboard ini dimaksudkan untuk melihat alur kerja dan hambatan saat standup, bukan untuk menilai individu.
````

Di tabel `## Struktur`, tambahkan baris:
```markdown
| `issue_log_agent/activity.py` | Developer Activity: ambil data GitLab, hitung durasi & flag, tulis tab |
| `issue_log_agent/dashboard.py` | Tab Dashboard: KPI, chart, conditional formatting |
| `issue_log_agent/workdays.py` | Hitung hari kerja |
```

- [ ] **Step 7: Commit**

```bash
git add issue_log_agent/__main__.py README.md tests/test_sync.py
git commit -m "Run developer activity dashboard after backlog sync"
```

---

## Verifikasi akhir (setelah semua task)

- [ ] `.venv/bin/python -m pytest -q tests` → semua PASS
- [ ] Smoke test offline dari Task 7 Step 5 → `exit=0`
- [ ] (Manual, butuh kredensial) `python -m issue_log_agent --activity-only` di spreadsheet uji. Periksa: tab Dashboard muncul di posisi pertama, KPI terisi, grafik tampil, warna flag sesuai, dan run kedua tidak menggandakan chart maupun aturan warna.
