#!/usr/bin/env bash
set -euo pipefail
SA_PASSWORD="${MSSQL_SA_PASSWORD:?}"
HOST="${MSSQL_HOST:-sqlserver}"
if [[ -x /opt/mssql-tools18/bin/sqlcmd ]]; then SQLCMD=/opt/mssql-tools18/bin/sqlcmd
elif [[ -x /opt/mssql-tools/bin/sqlcmd ]]; then SQLCMD=/opt/mssql-tools/bin/sqlcmd
else echo "sqlcmd missing" >&2; exit 1; fi
for attempt in $(seq 1 40); do
  "$SQLCMD" -S "$HOST" -U sa -P "$SA_PASSWORD" -C -Q "SELECT 1" >/dev/null 2>&1 && break
  sleep 3
done
"$SQLCMD" -S "$HOST" -U sa -P "$SA_PASSWORD" -C -i /sql/00_init.sql
echo "Demo31 init OK"
