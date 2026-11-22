<#
.SYNOPSIS
Experiment 03: compare .NET compilation modes with the SAME Demo 67 SQL Server.
.NOTES
Requires PowerShell 7, Docker Compose, SQL container initialized, Query Store ALL for full mode.
Stops/replaces ONLY the demo API; does not reset SQL or delete any volumes.
#>
[CmdletBinding()]
param(
    [ValidateSet('jit','r2r','composite','no-tiering')]
    [string[]] $Modes = @('jit','r2r','composite','no-tiering'),
    [ValidateRange(1,100)] [int] $Take = 100,
    [ValidateRange(1,100)] [int] $Repetitions = 10,
    [ValidateRange(10,180)] [int] $ReadyTimeoutSeconds = 120,
    [switch] $SkipQueryStore
)
$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$root = (Resolve-Path (Join-Path $PSScriptRoot '../..')).Path
$composeOverride = Join-Path $root 'docker-compose.compilation.yml'
$resultsDir = Join-Path $PSScriptRoot 'results'
New-Item -ItemType Directory -Path $resultsDir -Force | Out-Null
$experimentId = [guid]::NewGuid().ToString('N').Substring(0,8)
$baseUrl = 'http://localhost:5087'

function Invoke-Compose([string[]] $Arguments) {
    & docker compose --project-directory $root -f (Join-Path $root 'docker-compose.yml') -f $composeOverride @Arguments
    if ($LASTEXITCODE -ne 0) { throw "docker compose failed: $($Arguments -join ' ')" }
}
function Get-LatestBenchmark([string] $Prefix, [string[]] $BeforeNames) {
    $files = @(Get-ChildItem -LiteralPath (Join-Path $root 'scripts') -Filter "$Prefix*.csv" -File |
        Where-Object { $_.Name -notin $BeforeNames } |
        Sort-Object LastWriteTimeUtc -Descending)
    if ($files.Count -lt 1) { throw "No new $Prefix CSV produced by benchmark" }
    return $files[0]
}
function Get-Median([double[]] $Values) {
    if ($Values.Count -eq 0) { throw 'No timings' }
    $sorted = @($Values | Sort-Object)
    $half = [int][math]::Floor($sorted.Count / 2)
    if ($sorted.Count % 2) { return [double]$sorted[$half] }
    return [double](($sorted[$half - 1] + $sorted[$half]) / 2)
}

$dockerInfo = & docker version --format '{{.Server.Version}}' 2>&1
if ($LASTEXITCODE -ne 0) { throw "Docker not available: $dockerInfo" }
$serviceStatus = @(docker compose --project-directory $root ps --services --status running)
if ($LASTEXITCODE -ne 0 -or 'sql' -notin $serviceStatus) {
    throw 'SQL service is not running. Start the original Demo 67 SQL container first.'
}

$rows = [System.Collections.Generic.List[object]]::new()
Write-Host "Experiment $experimentId | Docker Engine $dockerInfo | SQL container remains running"
try {
    foreach ($mode in $Modes) {
        Write-Host ""
        Write-Host "=== Compilation mode: $mode ==="
        $env:LAB_COMPILATION_MODE = if ($mode -eq 'no-tiering') { 'jit' } else { $mode }
        $env:LAB_TIERED_COMPILATION = if ($mode -eq 'no-tiering') { '0' } else { '1' }

        # Build outside the readiness timer. The same source, dependencies and publish model are used.
        Invoke-Compose @('build','api')
        $startup = [Diagnostics.Stopwatch]::StartNew()
        Invoke-Compose @('up','-d','--no-deps','--force-recreate','--no-build','api')
        $ready = $false
        while ($startup.Elapsed.TotalSeconds -lt $ReadyTimeoutSeconds) {
            try {
                $r = Invoke-RestMethod -Uri "$baseUrl/health" -TimeoutSec 3
                if ($r.status -eq 'ready') { $ready = $true; break }
            } catch { Start-Sleep -Milliseconds 250 }
        }
        $startup.Stop()
        if (-not $ready) { throw "API not ready after $ReadyTimeoutSeconds seconds for $mode. Check docker compose logs api." }
        $containerReadyMs = [math]::Round($startup.Elapsed.TotalMilliseconds,2)

        # Integrity check: prevents measuring strategies with different SQL command counts or data.
        & (Join-Path $root 'scripts/Test-Lab.ps1') -BaseUrl $baseUrl -Take $Take
        if (-not $?) { throw "Smoke test failed for $mode" }

        $scriptDir = Join-Path $root 'scripts'
        $previous = @{
            comparison = @(Get-ChildItem $scriptDir -Filter 'comparison-*.csv' -File | ForEach-Object Name)
            detail = @(Get-ChildItem $scriptDir -Filter 'benchmark-all-*.csv' -File | ForEach-Object Name)
        }
        if (-not $SkipQueryStore) {
            & (Join-Path $scriptDir 'Run-All-Benchmark.ps1') -BaseUrl $baseUrl -Take $Take -Repetitions $Repetitions
            if (-not $?) { throw "Full benchmark failed for $mode" }
            $summary = Get-LatestBenchmark 'comparison-' $previous.comparison
            $raw = Get-LatestBenchmark 'benchmark-all-' $previous.detail
            Copy-Item -LiteralPath $summary.FullName -Destination (Join-Path $resultsDir "$experimentId-$mode-comparison.csv")
            Copy-Item -LiteralPath $raw.FullName -Destination (Join-Path $resultsDir "$experimentId-$mode-raw.csv")
            $data = @(Import-Csv -LiteralPath $summary.FullName)
            foreach ($item in $data) {
                $rows.Add([pscustomobject]@{
                    ExperimentId = $experimentId
                    Mode = $mode
                    Take = $Take
                    Repetitions = $Repetitions
                    ContainerReadyMs = $containerReadyMs
                    Strategy = $item.Strategy
                    MedianEfMs = $item.MedianEfMs
                    SqlCommandsPerRequest = $item.SqlCommandsPerRequest
                    SqlCpuMsPerRequest = $item.SqlCpuMsPerRequest
                    SqlDurationMsPerRequest = $item.SqlDurationMsPerRequest
                    SqlLogicalReadsPerRequest = $item.SqlLogicalReadsPerRequest
                    Measurement = 'QueryStore'
                })
            }
        } else {
            & (Join-Path $scriptDir 'Run-Benchmark.ps1') -BaseUrl $baseUrl -Take $Take -Repetitions $Repetitions -Strategy all
            if (-not $?) { throw "Application benchmark failed for $mode" }
            $raw = Join-Path $scriptDir 'benchmark-results.csv'
            Copy-Item -LiteralPath $raw -Destination (Join-Path $resultsDir "$experimentId-$mode-raw.csv")
            $data = @(Import-Csv -LiteralPath $raw)
            foreach ($strategy in @('nplusone','include','projection','split')) {
                $subset = @($data | Where-Object Strategy -eq $strategy)
                if ($subset.Count -ne $Repetitions) { throw "Missing $strategy samples for $mode" }
                $times = @($subset | ForEach-Object { [double]::Parse($_.ElapsedMs, [Globalization.CultureInfo]::CurrentCulture) })
                $rows.Add([pscustomobject]@{
                    ExperimentId = $experimentId
                    Mode = $mode
                    Take = $Take
                    Repetitions = $Repetitions
                    ContainerReadyMs = $containerReadyMs
                    Strategy = $strategy
                    MedianEfMs = [math]::Round((Get-Median $times),2)
                    SqlCommandsPerRequest = ($subset | Select-Object -First 1).SqlCommands
                    SqlCpuMsPerRequest = $null
                    SqlDurationMsPerRequest = $null
                    SqlLogicalReadsPerRequest = $null
                    Measurement = 'ApplicationOnly'
                })
            }
        }
        Write-Host "PASS $mode | API ready after $containerReadyMs ms | $($rows.Count) total comparison rows"
    }
    $output = Join-Path $resultsDir "$experimentId-compilation-comparison.csv"
    $rows | Export-Csv -LiteralPath $output -NoTypeInformation -Encoding utf8
    $rows | Format-Table Mode,Strategy,MedianEfMs,SqlCpuMsPerRequest,SqlLogicalReadsPerRequest,ContainerReadyMs -AutoSize
    Write-Host "Saved: $output"
    Write-Host 'NOTE: ContainerReadyMs includes Docker Compose overhead; not pure CLR startup.'
    Write-Host 'NOTE: The last tested API image remains running. SQL data volume was not touched.'
} finally {
    Remove-Item Env:LAB_COMPILATION_MODE -ErrorAction SilentlyContinue
    Remove-Item Env:LAB_TIERED_COMPILATION -ErrorAction SilentlyContinue
}
