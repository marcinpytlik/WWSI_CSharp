#!/usr/bin/env bash
set -uo pipefail
stamp="${1:?capture stamp required}"
fifo="/tmp/${stamp}.fifo"
outfile="/output/${stamp}.csv"
logfile="/output/${stamp}-capture.log"
exitfile="/output/${stamp}.exit"
rm -f "$fifo" "$exitfile"
mkfifo "$fifo"
# Hold both ends open so the input reader can run immediately, without
# blocking until the PowerShell runner sends the final q.
exec 3<>"$fifo"
# 'script' creates a PTY; dotnet-counters expects a real interactive console.
cat "$fifo" | script -q -e -c "/tools/dotnet-counters collect --process-id 1 --refresh-interval 1 --format csv --counters System.Runtime --output $outfile" /dev/null > "$logfile" 2>&1
rc=$?
printf '%s\n' "$rc" > "$exitfile"
rm -f "$fifo"
exit "$rc"
