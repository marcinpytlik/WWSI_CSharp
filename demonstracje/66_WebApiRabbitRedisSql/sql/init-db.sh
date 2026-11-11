#!/usr/bin/env bash
set -euo pipefail

# One-off provisioning process. Only this container receives the sa password.
# Lab-only sqlcmd -v variables: do not pass production secrets via process arguments.
exec /opt/mssql-tools18/bin/sqlcmd \
  -S sql -U sa -P "$SQL_SA_PASSWORD" -C -b -l 30 -t 120 \
  -i /scripts/01-bootstrap.sql \
  -v ApiPassword="$SQL_API_PASSWORD" WorkerPassword="$SQL_WORKER_PASSWORD"
