"""Optional Claude step: propose a title (and source) when the reporter left them empty.

The template has no mandatory Title column, so the agent can draft one from the body.
Suggestions are written back to the sheet so the reporter can revise them (slide 15:
"possibility manual revision if have an error").
"""

from __future__ import annotations

import logging

from pydantic import BaseModel, Field

from .config import ClaudeConfig, IssueType
from .sheet import Row

log = logging.getLogger(__name__)

_SYSTEM = """Kamu membantu tim Rapid Technical menulis judul issue GitLab sesuai Development Guideline.

Format judul lengkap: [PREFIX 1][PREFIX 2]-<judul>. Prefix ditambahkan oleh sistem; kamu hanya menulis bagian <judul>.
Contoh judul yang baik:
- [FEATURE][FSD]-Penambahan column salary pada list debitur multiguna
- [BUG][SIT]-Error HTTP 500 saat submit pengajuan dengan data valid
- [SECURITY][EXT]-Document API belum melakukan authorization check
- [PERFORMANCE][INT]-Response time API Submit Pengajuan tinggi saat 50 concurrent user

Aturan <judul>: Bahasa Indonesia (istilah teknis boleh bahasa Inggris), ringkas dan spesifik, maksimal {max_len} karakter,
sebut fitur/modul yang terdampak, tanpa titik di akhir, tanpa prefix [..].

Untuk source (PREFIX 2), pilih salah satu dari daftar yang diberikan hanya jika isi issue menyebutnya dengan jelas;
jika tidak yakin, kembalikan string kosong."""


class TitleSuggestion(BaseModel):
    title: str = Field(description="Judul tanpa prefix")
    source: str = Field(description="Salah satu nilai source yang diizinkan, atau string kosong jika tidak yakin")


class ClaudeAssistant:
    def __init__(self, cfg: ClaudeConfig, max_title_len: int = 100):
        import anthropic

        self._anthropic = anthropic
        self.cfg = cfg
        self.max_title_len = max_title_len
        self.client = anthropic.Anthropic()  # ANTHROPIC_API_KEY or an `ant auth login` profile

    def suggest(self, row: Row, itype: IssueType, need_source: bool) -> TitleSuggestion | None:
        body = "\n\n".join(f"## {s.heading}\n{row.get(s.column)}" for s in itype.sections if row.get(s.column))
        prompt = (
            f"Tipe issue (PREFIX 1): {itype.name}\n"
            f"Source yang diizinkan (PREFIX 2): {', '.join(itype.sources)}\n"
            f"Source perlu ditebak: {'ya' if need_source else 'tidak'}\n\n"
            f"Isi issue:\n{body}"
        )
        try:
            response = self._call(prompt)
        except self._anthropic.APIError as e:
            # A Claude outage must not stop the sync; the row just stays without a title.
            log.warning("Claude gagal dipanggil: %s", e)
            return None
        if response.stop_reason == "refusal" or response.parsed_output is None:
            log.warning("Claude tidak memberi saran judul (stop_reason=%s)", response.stop_reason)
            return None
        s = response.parsed_output
        s.title = s.title.strip().rstrip(".")
        s.source = s.source.strip().upper()
        if s.source not in itype.sources:
            s.source = ""
        return s

    def _call(self, prompt: str):
        return self.client.beta.messages.parse(
            model=self.cfg.model,
            max_tokens=16000,
            system=_SYSTEM.format(max_len=self.max_title_len),
            messages=[{"role": "user", "content": prompt}],
            output_format=TitleSuggestion,
            output_config={"effort": self.cfg.effort},
            betas=["server-side-fallback-2026-07-01"],
            fallbacks="default",
        )
