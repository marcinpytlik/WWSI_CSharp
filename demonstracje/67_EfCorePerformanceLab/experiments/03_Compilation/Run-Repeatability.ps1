<#
.SYNOPSIS
Experiment 03 phase B: repeated, counterbalanced compilation-mode comparisons.
.DESCRIPTION
Four rounds form a balanced Latin square: each mode occupies each position once.
Uses existing Run-Experiment.ps1 (with Query Store) and preserves the SQL container and volume.
Exports round-level CSV and a descriptive rollup. It does not claim statistical significance.
#>
[CmdletBinding()]
param(
    [ValidateRange(1,4)] [int] $Rounds = 4,
    [ValidateRange(1,100)] [int] $Take = 100,
    [ValidateRange(1,100)] [int] $Repetitions = 10
)
$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
$root = $PSScriptRoot
$resultsDir = Join-Path $root 'results'
New-Item -ItemType Directory -Force -Path $resultsDir | Out-Null
$batchId = [guid]::NewGuid().ToString('N').Substring(0,8)

# Williams balanced Latin square: every mode appears once at every position
# and ordered carry-over effects are more evenly distributed than fixed order.
$sequences = @(
    @('jit','r2r','no-tiering','composite'),
    @('r2r','composite','jit','no-tiering'),
    @('composite','no-tiering','r2r','jit'),
    @('no-tiering','jit','composite','r2r')
)
function Convert-LabNumber([string] $Value) {
    if ([string]::IsNullOrWhiteSpace($Value)) { throw 'Missing numeric metric' }
    $n = 0.0
    $styles = [Globalization.NumberStyles]::Float
    if ([double]::TryParse($Value, $styles, [Globalization.CultureInfo]::CurrentCulture, [ref]$n)) { return $n }
    if ([double]::TryParse($Value, $styles, [Globalization.CultureInfo]::InvariantCulture, [ref]$n)) { return $n }
    throw "Cannot parse numeric metric: $Value"
}
function Get-Median([double[]] $Values) {
    if ($Values.Count -eq 0) { throw 'Missing values' }
    $s = @($Values | Sort-Object)
    $half = [int][math]::Floor($s.Count / 2)
    if ($s.Count % 2) { return [double]$s[$half] }
    return [double](($s[$half - 1] + $s[$half]) / 2)
}

$collected = [System.Collections.Generic.List[object]]::new()
$manifest = [System.Collections.Generic.List[object]]::new()
Write-Host "=== Experiment 03 repeatability batch $batchId ==="
Write-Host "Rounds: $Rounds, requests/strategy/mode/round: $Repetitions; Query Store enabled"
for ($i = 0; $i -lt $Rounds; $i++) {
    $round = $i + 1
    $sequence = [string[]]$sequences[$i]
    Write-Host ""
    Write-Host "=== ROUND $round/$Rounds : $($sequence -join ' -> ') ==="
    $before = @(Get-ChildItem -LiteralPath $resultsDir -Filter '*-compilation-comparison.csv' -File | ForEach-Object Name)
    $started = (Get-Date).ToUniversalTime()
    & (Join-Path $root 'Run-Experiment.ps1') -Modes $sequence -Take $Take -Repetitions $Repetitions
    if (-not $?) { throw "Round $round failed" }
    $new = @(Get-ChildItem -LiteralPath $resultsDir -Filter '*-compilation-comparison.csv' -File |
        Where-Object { $_.Name -notin $before })
    if ($new.Count -ne 1) { throw "Expected one newly generated comparison CSV for round $round, found $($new.Count)" }
    $inputCsv = $new[0]
    $items = @(Import-Csv -LiteralPath $inputCsv.FullName)
    if ($items.Count -ne 16) { throw "Expected 16 rows (4 modes x 4 strategies), received $($items.Count)" }
    foreach ($mode in $sequence) {
        foreach ($strategy in @('nplusone','include','projection','split')) {
            $match = @($items | Where-Object { $_.Mode -eq $mode -and $_.Strategy -eq $strategy })
            if ($match.Count -ne 1) { throw "Missing or duplicate row: $mode / $strategy" }
            $r = $match[0]
            if ($r.Measurement -ne 'QueryStore') { throw "Query Store not measured for $mode / $strategy" }
            $expectedCommands = switch ($strategy) {
                'nplusone' { $Take + 1 }
                'split' { 2 }
                default { 1 }
            }
            if ((Convert-LabNumber $r.SqlCommandsPerRequest) -ne $expectedCommands) {
                throw "Unexpected SQL command count for $mode / $strategy"
            }
            $collected.Add([pscustomobject]@{
                BatchId = $batchId
                Round = $round
                Position = [array]::IndexOf($sequence, $mode) + 1
                Mode = $mode
                Strategy = $strategy
                Take = $Take
                Repetitions = $Repetitions
                MedianEfMs = Convert-LabNumber $r.MedianEfMs
                SqlCpuMsPerRequest = Convert-LabNumber $r.SqlCpuMsPerRequest
                SqlDurationMsPerRequest = Convert-LabNumber $r.SqlDurationMsPerRequest
                SqlLogicalReadsPerRequest = Convert-LabNumber $r.SqlLogicalReadsPerRequest
                SqlCommandsPerRequest = Convert-LabNumber $r.SqlCommandsPerRequest
                ContainerReadyMs = Convert-LabNumber $r.ContainerReadyMs
                SourceCsv = $inputCsv.Name
            })
        }
    }
    $manifest.Add([pscustomobject]@{
        BatchId = $batchId; Round = $round; Order = $sequence -join ','
        UtcStart = $started.ToString('o'); SourceCsv = $inputCsv.Name
    })
    # Checkpoint: files remain available if a later round fails.
    $collected | Export-Csv -LiteralPath (Join-Path $resultsDir "$batchId-rounds.csv") -NoTypeInformation -Encoding utf8
    $manifest | Export-Csv -LiteralPath (Join-Path $resultsDir "$batchId-manifest.csv") -NoTypeInformation -Encoding utf8
}
$rollup = foreach ($group in ($collected | Group-Object Mode,Strategy)) {
    $rows = @($group.Group)
    $ef = [double[]]@($rows | ForEach-Object MedianEfMs)
    $cpu = [double[]]@($rows | ForEach-Object SqlCpuMsPerRequest)
    $reads = [double[]]@($rows | ForEach-Object SqlLogicalReadsPerRequest)
    [pscustomobject]@{
        BatchId = $batchId
        Mode = $rows[0].Mode
        Strategy = $rows[0].Strategy
        Rounds = $rows.Count
        MedianOfRoundMediansMs = [math]::Round((Get-Median $ef),3)
        MinRoundMedianMs = [math]::Round(($ef | Measure-Object -Minimum).Minimum,3)
        MaxRoundMedianMs = [math]::Round(($ef | Measure-Object -Maximum).Maximum,3)
        MedianSqlCpuPerRequestMs = [math]::Round((Get-Median $cpu),3)
        MinLogicalReadsPerRequest = ($reads | Measure-Object -Minimum).Minimum
        MaxLogicalReadsPerRequest = ($reads | Measure-Object -Maximum).Maximum
    }
}
$rollup = @($rollup | Sort-Object Strategy,Mode)
$rollupPath = Join-Path $resultsDir "$batchId-rollup.csv"
$rollup | Export-Csv -LiteralPath $rollupPath -NoTypeInformation -Encoding utf8
Write-Host ""
Write-Host "=== Descriptive comparison across $Rounds rounds (NOT a significance test) ==="
$rollup | Format-Table Mode,Strategy,MedianOfRoundMediansMs,MinRoundMedianMs,MaxRoundMedianMs,MedianSqlCpuPerRequestMs,MinLogicalReadsPerRequest,MaxLogicalReadsPerRequest -AutoSize
Write-Host "Round data: $resultsDir/$batchId-rounds.csv"
Write-Host "Order/manifest: $resultsDir/$batchId-manifest.csv"
Write-Host "Rollup: $rollupPath"
Write-Host 'The last compilation mode remains running; the SQL container and its data volume were preserved.'
