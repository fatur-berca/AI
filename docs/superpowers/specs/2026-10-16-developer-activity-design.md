# Developer Activity Dashboard — Design

Tanggal: 2026-10-16
Lokasi kode: `tools/issue-log-agent/`

## Tujuan

Lead bisa melihat setiap developer sedang mengerjakan issue apa, sudah berapa lama, dan issue mana
yang macet, langsung dari Google Sheet backlog yang sama. Dashboard ini dipakai untuk daily standup.
Fokusnya alur kerja dan hambatan, bukan penilaian individu.

Kriteria sukses:

- Satu run agent memperbarui data aktivitas dan dashboard tanpa langkah manual.
- Dashboard menampilkan KPI, beban per developer (grafik), dan daftar issue aktif dengan warna flag.
- Issue macet (IDLE / LAMA / REWORK / NO ASSIGNEE) terlihat tanpa perlu membuka GitLab.

Di luar scope (tahap berikutnya): cycle time per tipe, sync Redmine, notifikasi chat, dashboard HTML.

## Keputusan

| Topik | Keputusan |
|---|---|
| Output | Google Sheets: tab data + tab `Dashboard` di spreadsheet backlog yang sama |
| Sumber data | GitLab: assignee, label `status::*`, riwayat label, komentar, MR terkait |
| Jadwal | Ikut run agent backlog (setelah sync). Flag `--no-activity` dan `--activity-only` |
| Satuan durasi | Hari kerja (Senin–Jumat, `timezone` config, daftar `holidays` opsional) |

## Arsitektur

```
__main__.py ── Syncer (sudah ada) ──► ActivityMonitor
                                        │
              gitlab_client.py (read) ◄─┤  collect → compute → render
                                        │
              sheet.py (tab I/O)      ◄─┘  tulis "Developer Activity", "Developer Summary",
                                            buat/perbarui "Dashboard"
```

Modul baru:

- `issue_log_agent/activity.py`
  - `collect(gitlab, cfg) -> list[IssueActivity]`: ambil data mentah dari GitLab.
  - `compute(issues, cfg, now) -> ActivityReport`: logika murni (durasi, idle, rework, flag). `now` diinjeksi agar mudah di-test.
  - `ActivityMonitor.run(book)`: collect → compute → tulis tab.
- `issue_log_agent/workdays.py`: `business_days_between(start, end, tz, holidays) -> float`.
- `issue_log_agent/dashboard.py`: membangun tab `Dashboard` (layout, formula, chart, conditional formatting) lewat Sheets API `batchUpdate`.

Perubahan modul yang ada:

- `gitlab_client.py`: method read-only baru, semuanya dengan pagination (`per_page=100`, header `X-Next-Page`):
  - `list_issues(labels, state="opened")`
  - `label_events(iid)` → `GET /issues/:iid/resource_label_events`
  - `notes(iid)` → `GET /issues/:iid/notes?sort=desc` (dengan filter `system == false`)
  - `related_merge_requests(iid)` → `GET /issues/:iid/related_merge_requests`
- `sheet.py`: `SpreadsheetSource` mendapat `worksheet_or_create(name, rows, cols)` dan `batch_update(body)` (level spreadsheet, untuk chart dan formatting). `GoogleSpreadsheet` mengimplementasikannya lewat gspread. `CsvSpreadsheet` tidak mendukung keduanya, sehingga aktivitas dilewati pada `--csv-dir`.
- `config.py` dan `config.example.yaml`: blok `activity` baru (lihat bawah).
- `__main__.py`: menjalankan `ActivityMonitor` setelah `Syncer`, termasuk di setiap iterasi `--watch`.

## Data per issue

Issue yang diambil adalah issue open dengan label `backlog` (`labels.default`) yang statusnya termasuk
`activity.active_statuses` (default: On-Progress, Testing, Re-work, Ready to Deploy).

| Field | Cara hitung |
|---|---|
| Developer | setiap `assignees[].name`; satu issue dengan 2 assignee menghasilkan 2 baris |
| Status | label `status::*` yang cocok dengan `statuses` di config |
| Since | `created_at` event `add` terakhir untuk label status saat ini; fallback `issue.updated_at` |
| Hari di Status | `business_days_between(since, now)`, dibulatkan 1 desimal |
| Last Activity | max dari: komentar non-system terbaru, `updated_at` MR terkait, event label terbaru |
| Hari Idle | `business_days_between(last_activity, now)` |
| Re-work | jumlah event `add` untuk label `status::Re-work` |
| Type | label tipe (`feature`/`bug`/`security`/`performance`) |

## Flag

Satu baris bisa punya beberapa flag, dipisah koma. Semua threshold ada di config:

```yaml
activity:
  enabled: true
  active_statuses: [On-Progress, Testing, Re-work, Ready to Deploy]
  idle_days: 2                  # IDLE  : tidak ada aktivitas >= N hari kerja
  max_days:                     # LAMA  : terlalu lama di status tertentu
    On-Progress: 5
    Testing: 3
    Re-work: 2
  rework_limit: 2               # REWORK: label Re-work ditambahkan >= N kali
  holidays: []                  # tanggal merah, format YYYY-MM-DD
  tabs:
    activity: Developer Activity
    summary: Developer Summary
    dashboard: Dashboard
```

- `NO ASSIGNEE`: issue berstatus aktif tetapi tanpa assignee. Developer ditulis `(belum di-assign)`.

## Tab data (ditulis ulang setiap run)

**Developer Activity**: satu baris per developer × issue, diurutkan per developer lalu Hari di Status menurun.

`Developer | Issue | Title | Type | Status | Since | Hari di Status | Last Activity | Hari Idle | Re-work | Flag | URL`

Kolom Issue berisi `=HYPERLINK(url, "#iid")`.

**Developer Summary**: satu baris per developer, diurutkan Total Aktif menurun.

`Developer | Total Aktif | On-Progress | Testing | Re-work | Ready to Deploy | Jumlah Flag | Issue Terlama (hari)`

Penulisan tab memakai `ws.clear()` lalu satu `ws.update()`, sehingga data lama tidak tersisa.
Waktu ditulis dalam `timezone` config dengan format `YYYY-MM-DD HH:MM`.

## Tab Dashboard

Dashboard dibuat sekali saat tab belum ada (atau dibangun ulang dengan flag `--rebuild-dashboard`).
Isinya formula yang membaca tab data, jadi setiap run hanya perlu menulis tab data dan sel
"Diperbarui". Layout:

1. **Header**: judul dan `Diperbarui: <timestamp>` (sel ini ditulis agent setiap run).
2. **Kartu KPI** (formula `COUNTA` / `COUNTIF` ke tab Developer Activity):
   Issue Aktif · Developer Aktif · IDLE · LAMA · REWORK · NO ASSIGNEE.
3. **Grafik beban per developer**: stacked bar dari tab Developer Summary (per status), dibuat dengan `addChart`.
4. **Tabel issue perlu perhatian**: `=QUERY('Developer Activity'!A:L, "select ... where K <> '' order by I desc")`,
   yaitu semua baris yang punya flag, diurutkan dari yang paling lama idle.
5. **Tabel semua issue aktif**: `=QUERY(...)` tanpa filter.
6. **Conditional formatting** di kolom Flag dan Hari di Status, di Dashboard maupun Developer Activity:
   IDLE merah muda, LAMA oranye, REWORK kuning, NO ASSIGNEE abu-abu.

Karena range formula dan chart menunjuk seluruh kolom, jumlah baris yang berubah tidak memerlukan rebuild.

## Error handling

- Gagal mengambil data satu issue: issue dilewati, `logging.warning`, lalu lanjut. Jumlah yang dilewati masuk ringkasan log.
- `list_issues` gagal (GitLab tidak bisa diakses): tab data dan dashboard tidak disentuh, error di-log, dan exit code menjadi 1.
- Gagal pada aktivitas tidak membatalkan hasil sync backlog yang sudah tertulis.
- `--dry-run`: kalau token tersedia, data diambil dan tabel dicetak ke log tanpa menulis ke sheet. Kalau token tidak ada, aktivitas dilewati dengan pesan info.

## Testing

`tests/test_activity.py`, `tests/test_workdays.py`, `tests/test_dashboard.py` memakai fake GitLab client dan fake spreadsheet:

- Hari kerja: melewati akhir pekan dan holiday, menghitung pecahan hari, menangani start setelah end.
- Since: diambil dari event `add` terakhir untuk label status saat ini, termasuk kasus label dihapus lalu ditambah lagi.
- Re-work dihitung dengan benar, dan setiap flag muncul tepat di threshold-nya.
- Multi-assignee menghasilkan beberapa baris, tanpa assignee menghasilkan `NO ASSIGNEE`.
- Isi dan urutan kedua tab data.
- Dashboard: request `batchUpdate` berisi chart dan aturan conditional formatting yang diharapkan, dibuat hanya jika tab belum ada.
- Kegagalan satu issue tidak menghentikan run, dan kegagalan `list_issues` tidak menimpa tab.
