#!/usr/bin/env bash
set -euo pipefail
exec /opt/mssql-tools18/bin/sqlcmd -S sql -U sa -P "$SQL_SA_PASSWORD" -C -b -l 30 \
  -i /scripts/bootstrap.sql -v AppPassword="$SQL_HANGFIRE_PASSWORD"
