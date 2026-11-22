#!/usr/bin/env bash
set -uo pipefail
stamp="${1:?capture stamp required}"
kind="${2:-counters}"
fifo="/tmp/${stamp}.fifo"
outfile="/output/${stamp}.csv"
logfile="/output/${stamp}-capture.log"
exitfile="/output/${stamp}.exit"
rm -f "$fifo" "$exitfile"
mkfifo "$fifo"
# Hold both ends open so the PTY input reader does not see EOF.
# The host sends q (counters) or Enter (trace) only AFTER the HTTP workload.
exec 3<>"$fifo"
case "$kind" in
  counters)
    command="/tools/dotnet-counters collect --process-id 1 --refresh-interval 1 --format csv --counters System.Runtime --output $outfile"
    ;;
  trace)
    outfile="/output/${stamp}.nettrace"
    command="/tools/dotnet-trace collect --process-id 1 --profile dotnet-common,dotnet-sampled-thread-time --output $outfile"
    ;;
  *)
    printf 'Unsupported capture kind: %s\n' "$kind" >&2
    exit 2
    ;;
esac
# 'script' creates a PTY, allowing a graceful interactive stop.
script -q -e -c "$command" /dev/null < "$fifo" > "$logfile" 2>&1
rc=$?
printf '%s\n' "$rc" > "$exitfile"
rm -f "$fifo"
exit "$rc"
