# Issue Backlog Agent: Google Sheet → GitLab Issue Board

Agent Python untuk **Alur Pelaporan Backlog** di *Development Guideline* (slide 15).
Agent membaca *Template Issue Backlog* (satu tab per tipe), memeriksa isinya terhadap standar
penulisan (slide 5–13), membuat issue di GitLab Issue Board, lalu menulis link dan status sync
kembali ke sheet.

## Alur (slide 15)

```
QA / BA / Internal / Lead ──► Template Issue Backlog (tab FEATURE, BUG, SECURITY, PERFORMANCE)
                                          │
                                          ▼
                                   Issue Backlog Agent
            ┌─────────────────────────────┼──────────────────────────────┐
     baris baru                    tidak sesuai standar          sudah punya issue
            │                             │                              │
  Title/Source kosong? →        Sync Status = INVALID           sinkron Issue Board Status
  tebak dari isi / Claude       + alasan di Sync Message        (default: GitLab → sheet)
            │                   → reporter revisi manual
            ▼
  validasi → buat issue GitLab → tulis balik GitLab Issue, URL, Sync Status
            │
            ▼
  Checking Issue Backlog (manual) → Developer
```

## Standar yang diterapkan

**Judul (slide 5):** `[PREFIX 1][PREFIX 2]-<judul>`, contohnya `[FEATURE][FSD]-Penambahan column salary pada list debitur multiguna`

| Tab (PREFIX 1) | Source (PREFIX 2) |
|---|---|
| FEATURE | FSD, BRD, INT |
| BUG | VIT, SIT, UAT |
| SECURITY | EXT, INT |
| PERFORMANCE | EXT, INT |

- PREFIX 1 diambil dari nama tab. PREFIX 2 diambil dari kolom **Source**.
- Kalau Source kosong, agent mencoba menebaknya dari isi issue, misalnya `FSD: ...` di Reference atau `Environment: SIT`. Jika tidak berhasil, Claude yang menebak (bila diaktifkan).
- Kalau kolom Title sudah berisi `[BUG][SIT]-...`, prefix-nya ikut dicek terhadap tab dan Source.

**Body (slide 6–13):** setiap kolom jadi section `## <Heading>`, dengan urutan mengikuti slide.

- Acceptance Criteria → checklist `- [ ]`
- Steps to Reproduce → daftar bernomor
- Scope, Requirement, Out of Scope, Reference, Environment, Evidence → bullet
- Section yang kosong tidak ditampilkan. Kolom wajib bisa diatur di `config.yaml` (`required: true`).

**Issue Board Status (slide 3):** Open, On-Progress, Testing, Re-work, Ready to Deploy, Closed.
Board GitLab berbasis label, jadi status dipetakan ke label `status::<nama>`. Status Closed menutup issue.
Buat list di Issue Board untuk masing-masing label tersebut.

**Label lain:** `backlog`, label tipe (`feature`/`bug`/`security`/`performance`), dan `source::<SOURCE>`.

## Perubahan yang dibutuhkan di Template v1

Agent butuh beberapa kolom tambahan. `templates/Template_Backlog_v2.xlsx` sudah berisi semuanya,
lengkap dengan dropdown Source dan Status. Tanda `*` di header = wajib; tanda ini diabaikan agent.

| Tab | Kolom yang ditambahkan |
|---|---|
| Semua | **Title**, **Source**, dan kolom agent: GitLab Issue, GitLab URL, Sync Status, Sync Message, Synced At |
| BUG | **Description** (ada di slide 8, belum ada di v1) |
| PERFORMANCE | **Current Condition**, **Expected Condition** (slide 12; v1 memakai kolom SECURITY) |

Template v2 bisa dibuat ulang dari config dengan `python scripts/make_template.py` (butuh `openpyxl`).

## Setup

1. **Python 3.10+**, lalu `pip install -r requirements.txt`.
2. **Google Service Account**
   - Buat project di Google Cloud Console, lalu aktifkan *Google Sheets API*.
   - Buat Service Account beserta key JSON. Jangan commit file key ini.
   - Share spreadsheet ke email service account dengan akses **Editor**.
   - `export GOOGLE_APPLICATION_CREDENTIALS=/path/ke/key.json`
3. **GitLab:** buat Personal/Project Access Token dengan scope `api`, lalu `export GITLAB_TOKEN=glpat-xxxx`.
4. **(Opsional) Claude:** isi `claude.enabled: true` di config dan `export ANTHROPIC_API_KEY=...`.
   Claude hanya dipakai untuk mengisi **Title/Source yang kosong**. Hasilnya ditulis balik ke sheet
   dan ditandai di Sync Message ("Title dibuat oleh Claude, mohon dicek").
5. `cp config.example.yaml config.yaml`, lalu isi `gitlab.url`, `gitlab.project`, dan sesuaikan bagian lain bila perlu.

## Pemakaian

```bash
# Preview offline dari export CSV per tab (examples/<TAB>.csv), tanpa kredensial
python -m issue_log_agent --config config.yaml --csv-dir examples -v

# Preview dari sheet asli tanpa membuat issue / menulis sheet
python -m issue_log_agent --config config.yaml --dry-run

# Jalankan sync (semua tab)
python -m issue_log_agent --config config.yaml

# Hanya tab/baris tertentu
python -m issue_log_agent --config config.yaml --type BUG --row 5

# Arah sinkron status untuk baris yang sudah punya issue
python -m issue_log_agent --status-sync pull   # GitLab → sheet (default)
python -m issue_log_agent --status-sync push   # sheet → GitLab (label status + close/reopen)
python -m issue_log_agent --status-sync none

# Jalan terus, polling tiap 5 menit
python -m issue_log_agent --config config.yaml --watch 300
```

Default `pull` dipilih karena setelah issue dibuat, developer memindahkan kartu di board GitLab.
Dengan `pull`, kolom Issue Board Status di sheet ikut ter-update, jadi reporter bisa memantau progres
tanpa membuka GitLab.

Untuk penjadwalan bisa memakai cron (`*/15 * * * * cd /path && python -m issue_log_agent`)
atau GitLab CI scheduled pipeline, dengan token dan key JSON disimpan sebagai CI variable.

## Anti-duplikat

- Baris yang kolom **GitLab Issue**-nya sudah terisi tidak dibuat ulang.
- Setiap issue diberi marker tersembunyi berisi hash isi baris
  (`<!-- issue-backlog:<spreadsheet>:<tab>:<hash> -->`). Sebelum membuat issue, agent mencari marker
  ini di GitLab. Jadi kalau skrip terhenti setelah issue dibuat tapi sebelum sheet ter-update,
  issue tidak akan dobel.

## Test

```bash
pip install pytest && python -m pytest -q tests
```

## Struktur

| File | Isi |
|---|---|
| `issue_log_agent/config.py` | Load & validasi `config.yaml` |
| `issue_log_agent/sheet.py` | Baca/tulis tab Google Sheet (gspread) + sumber CSV offline |
| `issue_log_agent/formatter.py` | Judul, body, label, validasi sesuai Development Guideline |
| `issue_log_agent/claude_assist.py` | (Opsional) saran Title/Source dari Claude |
| `issue_log_agent/gitlab_client.py` | Client REST API GitLab v4 |
| `issue_log_agent/sync.py` | Orkestrasi alur pelaporan backlog |
| `issue_log_agent/__main__.py` | CLI |
| `scripts/make_template.py` | Generator Template Issue Backlog v2 dari config |
| `examples/*.csv` | Contoh isi per tab (diambil dari contoh di slide) |
