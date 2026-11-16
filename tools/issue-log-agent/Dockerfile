FROM python:3.12-slim

ENV PYTHONDONTWRITEBYTECODE=1 \
    PYTHONUNBUFFERED=1 \
    PIP_NO_CACHE_DIR=1 \
    PIP_DISABLE_PIP_VERSION_CHECK=1

WORKDIR /app

# tzdata: image slim tidak punya database zona waktu (dibutuhkan untuk timezone: Asia/Jakarta)
COPY requirements.txt .
RUN pip install -r requirements.txt tzdata

COPY issue_log_agent/ issue_log_agent/

RUN useradd --system --uid 10001 agent
USER agent

# config.yaml dan key JSON Google di-mount saat runtime, tidak ikut di-image
ENTRYPOINT ["python", "-m", "issue_log_agent", "--config", "/app/config.yaml"]
CMD ["--watch", "300"]
