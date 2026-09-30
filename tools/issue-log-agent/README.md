# Issue Log Agent: Google Spreadsheet → GitLab Issue Board

Skrip otomasi (Python) yang membaca Issue Log dari Google Spreadsheet, memvalidasinya
sesuai standar penulisan, membuat/meng-update issue di GitLab, lalu menulis balik
hasilnya (nomor & link issue, status sync) ke spreadsheet.

## Alur

```
Google Sheet ──► baca baris ──► filter (Status = Open/New/Ready, belum punya issue)
                                   │
                                   ▼
                          validasi standar penulisan
                         (wajib isi, nilai yg diizinkan,
                              panjang judul)
                          │                     │
                     tidak valid              valid
                          │                     ▼
                          │        format judul, deskripsi, label,
                          │        assignee, due date (dari template)
                          │                     ▼
                          │        cek duplikat (marker di deskripsi)
                          │                     ▼
                          │           create / update issue GitLab
                          ▼                     ▼
          tulis balik: Sync Status, Sync Message, GitLab Issue, GitLab URL, Synced At
```

## Setup

1. **Python 3.10+**, lalu `pip install -r requirements.txt`.
2. **Google Service Account**
   - Google Cloud Console → buat project → aktifkan *Google Sheets API*.
   - Buat Service Account → buat key JSON → simpan (jangan di-commit).
   - Share spreadsheet ke email service account dengan akses **Editor**.
   - `export GOOGLE_APPLICATION_CREDENTIALS=/path/ke/key.json`
3. **GitLab token**: buat Personal/Project Access Token dengan scope `api`.
   `export GITLAB_TOKEN=glpat-xxxx`
4. `cp config.example.yaml config.yaml`, lalu sesuaikan:
   - `sheet.worksheet`: nama tab sheet.
   - `gitlab.url` / `gitlab.project`: instance & path project.
   - `columns`: header kolom sheet untuk tiap field.
   - `writeback`: tambahkan kolom-kolom ini di sheet (GitLab Issue, GitLab URL, Sync Status, Sync Message, Synced At).
   - `format` & `rules`: standar penulisan (judul, deskripsi, label, validasi).
   - `assignees`: nama PIC → username GitLab.

> Header yang berupa `No`, `Yes`, `On`, `Off` harus diberi tanda kutip di YAML (`key: "No"`).

## Pemakaian

```bash
# Preview offline dari file CSV (hasil File → Download → CSV), tanpa kredensial
python -m issue_log_agent --config config.yaml --csv examples/sample_issue_log.csv

# Preview dari sheet asli, tanpa membuat issue
python -m issue_log_agent --config config.yaml --dry-run

# Jalankan sync
python -m issue_log_agent --config config.yaml

# Hanya baris tertentu
python -m issue_log_agent --config config.yaml --row 12 --row 15

# Update issue yang sudah ada (judul/deskripsi/label, close/reopen via state_map)
python -m issue_log_agent --config config.yaml --update-existing

# Jalan terus, polling tiap 5 menit
python -m issue_log_agent --config config.yaml --watch 300
```

Untuk penjadwalan, bisa pakai cron (`*/15 * * * * cd /path && python -m issue_log_agent`)
atau GitLab CI scheduled pipeline (simpan `GITLAB_TOKEN` & key JSON sebagai CI variable).

## Anti-duplikat

- Baris yang kolom **GitLab Issue**-nya sudah terisi akan di-skip (kecuali `--update-existing`).
- Setiap deskripsi issue diberi marker tersembunyi
  `<!-- issue-log:<spreadsheet>:<worksheet>:<No> -->`. Sebelum membuat issue, agent
  mencari marker ini di GitLab. Kalau skrip sempat berhenti setelah issue dibuat tapi sebelum
  sheet ter-update, issue tidak akan dibuat dua kali.

## Test

```bash
pip install pytest && python -m pytest -q tests
```

## Struktur

| File | Isi |
|---|---|
| `issue_log_agent/config.py` | Load & validasi `config.yaml` |
| `issue_log_agent/sheet.py` | Baca/tulis Google Sheet (gspread) + sumber CSV offline |
| `issue_log_agent/formatter.py` | Validasi & format issue sesuai standar penulisan |
| `issue_log_agent/gitlab_client.py` | Client REST API GitLab v4 |
| `issue_log_agent/sync.py` | Orkestrasi alur sync |
| `issue_log_agent/__main__.py` | CLI |
