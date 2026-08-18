#!/usr/bin/env bash
set -euo pipefail

SA_PASSWORD="${MSSQL_SA_PASSWORD:?MSSQL_SA_PASSWORD is required}"
HOST="${MSSQL_HOST:-sqlserver}"

if [[ -x /opt/mssql-tools18/bin/sqlcmd ]]; then
  SQLCMD=/opt/mssql-tools18/bin/sqlcmd
elif [[ -x /opt/mssql-tools/bin/sqlcmd ]]; then
  SQLCMD=/opt/mssql-tools/bin/sqlcmd
else
  echo "sqlcmd not found in the SQL Server image." >&2
  exit 1
fi

for attempt in $(seq 1 40); do
  if "$SQLCMD" -S "$HOST" -U sa -P "$SA_PASSWORD" -C -Q "SELECT 1" >/dev/null 2>&1; then
    break
  fi
  echo "Waiting for SQL Server (${attempt}/40)…"
  sleep 3
done

"$SQLCMD" -S "$HOST" -U sa -P "$SA_PASSWORD" -C -i /sql/00_init.sql
"$SQLCMD" -S "$HOST" -U demo12_deploy -P "Demo12_Deploy_Pass!" -C -i /sql/01_dbfirst_schema.sql
echo "Demo12 init OK: logins, databases, Database First schema."
