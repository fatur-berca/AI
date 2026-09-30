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

## Template Issue Backlog v2

Pakai `templates/Template_Backlog_v2.xlsx`. File ini adalah revisi dari `Template_Backlog_v1.xlsx`:
style header dan warna tab tetap sama, hanya kolomnya yang ditambah. Upload ke Google Drive lalu buka
dengan Google Sheets, atau salin header-nya ke spreadsheet yang sudah dipakai.

| Tab | Kolom yang ditambahkan |
|---|---|
| Semua | **Title** dan **Source** (dropdown per tipe) di depan, dropdown **Issue Board Status**, kolom agent abu-abu: GitLab Issue, GitLab URL, Sync Status, Sync Message, Synced At |
| BUG | **Description** (slide 8) |
| PERFORMANCE | **Current Condition**, **Expected Condition** (slide 12); **Impact** tetap ada |

- Kolom wajib dan cara pengisiannya dijelaskan lewat note di header.
- **Title** diisi reporter, tanpa prefix. Kalau kosong dan `claude.enabled: true`, Claude membuatkan judul
  sebagai fallback. Kalau Claude tidak aktif, baris ditandai INVALID.
- Kolom abu-abu diisi agent, jangan diubah manual.

Kalau config berubah (misalnya kolom atau section baru), buat ulang v2 dari v1. Data yang sudah ada
dipindahkan berdasarkan nama header:

```bash
pip install openpyxl
python scripts/make_template.py --from templates/Template_Backlog_v1.xlsx --out templates/Template_Backlog_v2.xlsx
```

## Mode akses sheet: `service_account` atau `public`

| | `service_account` (default) | `public` |
|---|---|---|
| Credential Google | Key JSON service account | **Tidak perlu** |
| Syarat sheet | Di-share ke email service account sebagai Editor | General access = *Anyone with the link* |
| Link issue, Sync Status, alasan INVALID ditulis ke sheet | Ya | **Tidak**, hanya di log |
| Title/Source hasil Claude ditulis ke sheet | Ya | Tidak |
| Status GitLab → sheet (`pull`) | Ya | Tidak (`push` sheet → GitLab tetap bisa) |
| Anti-duplikat | Kolom GitLab Issue + marker | Marker di GitLab saja |

Google tidak mengizinkan penulisan ke sheet tanpa credential, walaupun sheet dibuka untuk umum.
Jadi mode `public` hanya membaca. Aktifkan dengan `sheet.access: public` di `config.yaml`.

Di mode `public`, setiap run membaca semua baris lagi. Agent mengecek ke GitLab lewat dua marker
tersembunyi: hash isi baris dan hash judul. Baris yang sudah pernah dikirim dilewati (`existing` di
log), dan pengecekan ini dilakukan sebelum Claude dipanggil.
**Batasannya:** kalau judul **dan** isi sebuah baris sama-sama diubah setelah terkirim, baris itu
dianggap baru dan dibuatkan issue lagi.

> ⚠️ Sheet publik bisa dibaca siapa pun yang punya link, termasuk temuan di tab **SECURITY**
> (detail celah keamanan dari pentest). Untuk data seperti itu, pakai `service_account` dan tutup
> akses publiknya.

## Menjalankan di lokal

Butuh **Python 3.10+** dan **git**.

**1. Ambil kode & install dependency**

```bash
git clone https://github.com/fatur-berca/AI.git
cd AI
git checkout claude/spreadsheet-gitlab-issue-agent-w9xevp   # sebelum PR di-merge
cd tools/issue-log-agent
python -m venv .venv
```

| | Windows (PowerShell) | macOS / Linux |
|---|---|---|
| Aktifkan venv | `.venv\Scripts\Activate.ps1` | `source .venv/bin/activate` |
| Install | `pip install -r requirements.txt` | `pip install -r requirements.txt` |

Kalau PowerShell menolak menjalankan script, jalankan sekali `Set-ExecutionPolicy -Scope CurrentUser RemoteSigned`.

**2. Coba offline dulu** (tanpa kredensial apa pun):

```bash
python -m issue_log_agent --config config.example.yaml --csv-dir examples -v
```

**3. Kredensial**
- **Google:** tidak perlu kalau memakai `sheet.access: public` (lihat bagian mode akses di atas). Untuk `service_account`: di Google Cloud Console aktifkan *Google Sheets API*, buat Service Account, lalu buat key
  JSON dan simpan di luar folder repo (mis. `C:\secrets\issue-agent.json`). Share spreadsheet
  ke email service account (`...@...iam.gserviceaccount.com`) sebagai **Editor**.
- **GitLab:** buat Personal/Project Access Token dengan scope `api`. Akun token minimal
  berperan *Reporter* di project.
- **Claude (opsional):** API key dari console.anthropic.com, hanya jika `claude.enabled: true`.

**4. Config**: `cp config.example.yaml config.yaml` (Windows: `copy`), lalu isi:
- `sheet.spreadsheet_id`: ID dari URL spreadsheet (`/d/<ID>/edit`).
- `sheet.credentials_file`: path ke key JSON. Boleh ditulis langsung, tanpa env var.
- `gitlab.url` dan `gitlab.project` (mis. `group/nama-project`).

`config.yaml` sudah di-`.gitignore`, jadi tidak ikut ter-commit.

**5. Set token & jalankan**

| | Windows (PowerShell) | macOS / Linux |
|---|---|---|
| Token GitLab | `$env:GITLAB_TOKEN="glpat-xxxx"` | `export GITLAB_TOKEN=glpat-xxxx` |
| Key Claude (opsional) | `$env:ANTHROPIC_API_KEY="sk-ant-..."` | `export ANTHROPIC_API_KEY=sk-ant-...` |

```bash
python -m issue_log_agent --dry-run     # cek dulu: baca sheet asli, tidak menulis apa pun
python -m issue_log_agent               # sync sungguhan
python -m issue_log_agent --watch 300   # jalan terus, cek tiap 5 menit
```

Env var di atas hanya berlaku di terminal yang sedang dibuka. Untuk menjalankan agent secara
terjadwal, pakai **Task Scheduler** (Windows) atau **cron** (macOS/Linux) yang memanggil
`python -m issue_log_agent` dari folder ini, dengan env var di-set di task tersebut.

**Masalah umum**

| Gejala | Penyebab |
|---|---|
| `SpreadsheetNotFound` / `403` dari Google | Spreadsheet belum di-share ke email service account, atau Sheets API belum aktif |
| `WorksheetNotFound` | Nama tab tidak sama dengan `types.<TIPE>.worksheet` |
| `kolom tidak ditemukan` di log | Header sheet berbeda dengan config. Pakai Template v2 |
| `401` dari GitLab | Token salah atau kedaluwarsa |
| `404` dari GitLab | `gitlab.project` salah, atau token tidak punya akses ke project |
| `SSL: CERTIFICATE_VERIFY_FAILED` | GitLab self-hosted dengan sertifikat internal. Set `REQUESTS_CA_BUNDLE` ke CA kantor (atau `verify_ssl: false` untuk uji coba saja) |

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
| `scripts/make_template.py` | Revisi Template v1 → v2 sesuai config |
| `templates/` | Template Issue Backlog v1 (asli) dan v2 (hasil revisi) |
| `examples/*.csv` | Contoh isi per tab (diambil dari contoh di slide) |
