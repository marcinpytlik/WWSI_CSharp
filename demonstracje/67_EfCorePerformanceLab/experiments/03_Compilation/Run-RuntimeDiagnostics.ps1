<#
.SYNOPSIS
EF Core lab 67 experiment 03C: process-wide .NET CPU/allocations/GC and application times.
.DESCRIPTION
Uses the existing SQL container, replaces ONLY the demo API.
Each mode: compile, restart, smoke test, warmup, per-strategy 30 HTTP requests, process counter deltas.
No Query Store clearing, no database deletion, no GC.Collect.
GC/CPU/allocations are PROCESS-WIDE, not EF-query-only measurements.
#>
[CmdletBinding()]
param(
    [ValidateSet('jit','r2r','composite','no-tiering')]
    [string[]] $Modes = @('jit','r2r','composite','no-tiering'),
    [ValidateRange(1,100)] [int] $Take = 100,
    [ValidateRange(5,200)] [int] $Repetitions = 30,
    [ValidateRange(1,50)] [int] $WarmupPerStrategy = 5,
    [ValidateRange(10,180)] [int] $ReadyTimeoutSeconds = 120
)
$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
$root = (Resolve-Path (Join-Path $PSScriptRoot '../..')).Path
$compose = Join-Path $root 'docker-compose.yml'
$override = Join-Path $root 'docker-compose.compilation.yml'
$resultsDir = Join-Path $PSScriptRoot 'results'
New-Item -ItemType Directory -Path $resultsDir -Force | Out-Null
$token = [guid]::NewGuid().ToString('N').Substring(0,8)
$url = 'http://localhost:5087'
$strategies = @('nplusone','include','projection','split')
$summary = [System.Collections.Generic.List[object]]::new()
$raw = [System.Collections.Generic.List[object]]::new()

function Invoke-Compose([string[]] $Arguments) {
    & docker compose --project-directory $root -f $compose -f $override @Arguments
    if ($LASTEXITCODE -ne 0) { throw "Docker compose failed: $($Arguments -join ' ')" }
}
function Median([double[]] $Numbers) {
    $a = @($Numbers | Sort-Object)
    if ($a.Count -eq 0) { throw 'Cannot calculate median without samples' }
    $middle = [int][math]::Floor($a.Count / 2)
    if ($a.Count % 2 -eq 1) { return [double]$a[$middle] }
    return [double](($a[$middle - 1] + $a[$middle]) / 2)
}
function Get-Diagnostics {
    return Invoke-RestMethod -Uri "$url/diag/runtime" -TimeoutSec 10
}
function Call-Strategy([string] $Strategy) {
    $response = Invoke-RestMethod -Uri "$url/api/orders/${Strategy}?take=$Take" -TimeoutSec 60
    if ([int]$response.orders -ne $Take) { throw "Unexpected order count for $Strategy" }
    $expected = switch ($Strategy) { 'nplusone' {$Take + 1} 'split' {2} default {1} }
    if ([int]$response.sqlCommands -ne $expected) { throw "Unexpected SQL command count for $Strategy" }
    $items = 0
    foreach ($order in $response.data) { $items += @($order.items).Count }
    if ($items -ne (10 * $Take)) { throw "Unexpected items for $Strategy : $items" }
    return $response
}

$serviceStatus = @(docker compose --project-directory $root ps --services --status running)
if ($LASTEXITCODE -ne 0 -or 'sql' -notin $serviceStatus) {
    throw 'SQL service must already be running; execute docker compose up -d sql.'
}
Write-Host "=== Runtime diagnostics experiment $token ==="
Write-Host 'CPU/GC/allocated bytes are whole-process DELTAS, not isolated EF query allocations.'
try {
    $env:LAB_RUNTIME_DIAGNOSTICS = 'true'
    foreach ($mode in $Modes) {
        Write-Host ""
        Write-Host "=== $mode ==="
        $env:LAB_COMPILATION_MODE = if ($mode -eq 'no-tiering') { 'jit' } else { $mode }
        $env:LAB_TIERED_COMPILATION = if ($mode -eq 'no-tiering') { '0' } else { '1' }
        Invoke-Compose -Arguments @('build','api')
        Invoke-Compose -Arguments @('up','-d','--no-deps','--force-recreate','--no-build','api')
        $ready = $false
        $clock = [Diagnostics.Stopwatch]::StartNew()
        while ($clock.Elapsed.TotalSeconds -lt $ReadyTimeoutSeconds) {
            try {
                $h = Invoke-RestMethod -Uri "$url/health" -TimeoutSec 3
                if ($h.status -eq 'ready') { $ready = $true; break }
            } catch { Start-Sleep -Milliseconds 250 }
        }
        if (-not $ready) { throw "API readiness timeout for $mode" }
        & (Join-Path $root 'scripts/Test-Lab.ps1') -BaseUrl $url -Take $Take
        if (-not $?) { throw "Smoke test failed for $mode" }

        # Warm up all strategies first to reduce JIT/connection initialization bias.
        foreach ($strategy in $strategies) {
            for ($k = 0; $k -lt $WarmupPerStrategy; $k++) {
                $null = Call-Strategy $strategy
            }
        }

        foreach ($strategy in $strategies) {
            # Snapshot endpoint itself incurs small allocations/CPU and is included in the delta.
            $before = Get-Diagnostics
            $times = [System.Collections.Generic.List[double]]::new()
            for ($i = 1; $i -le $Repetitions; $i++) {
                $response = Call-Strategy $strategy
                $elapsed = [double]$response.elapsedMs
                $times.Add($elapsed)
                $raw.Add([pscustomobject]@{
                    Token = $token
                    Mode = $mode
                    Strategy = $strategy
                    Run = $i
                    ElapsedMs = [math]::Round($elapsed,4)
                    SqlCommands = [int]$response.sqlCommands
                })
            }
            $after = Get-Diagnostics
            if ($before.processId -ne $after.processId) { throw "API process restarted during $mode / $strategy" }
            $alloc = [long]$after.allocatedTotalBytes - [long]$before.allocatedTotalBytes
            $cpu = [double]$after.cpuTotalMs - [double]$before.cpuTotalMs
            if ($alloc -lt 0 -or $cpu -lt 0) { throw "Nonmonotonic process counters for $mode / $strategy" }
            $summary.Add([pscustomobject]@{
                Token = $token
                Mode = $mode
                Strategy = $strategy
                Requests = $Repetitions
                ProcessId = $before.processId
                MedianEfMs = [math]::Round((Median ([double[]]$times.ToArray())),3)
                ProcessCpuMsPerRequest = [math]::Round($cpu / $Repetitions,3)
                ProcessAllocatedKBPerRequest = [math]::Round($alloc / (1024.0 * $Repetitions),2)
                Gen0CollectionsDelta = [int]$after.gen0Collections - [int]$before.gen0Collections
                Gen1CollectionsDelta = [int]$after.gen1Collections - [int]$before.gen1Collections
                Gen2CollectionsDelta = [int]$after.gen2Collections - [int]$before.gen2Collections
                HeapSizeBeforeMB = [math]::Round([double]$before.heapSizeBytes / 1MB,2)
                HeapSizeAfterMB = [math]::Round([double]$after.heapSizeBytes / 1MB,2)
                WorkingSetBeforeMB = [math]::Round([double]$before.workingSetBytes / 1MB,2)
                WorkingSetAfterMB = [math]::Round([double]$after.workingSetBytes / 1MB,2)
                Framework = [string]$after.framework
            })
            $record = $summary[$summary.Count - 1]
            Write-Host ("{0,-11} median={1:N2} ms; process CPU/req={2:N2} ms; allocated/req={3:N2} KB; GC0/1/2={4}/{5}/{6}" -f $strategy,$record.MedianEfMs,$record.ProcessCpuMsPerRequest,$record.ProcessAllocatedKBPerRequest,$record.Gen0CollectionsDelta,$record.Gen1CollectionsDelta,$record.Gen2CollectionsDelta)
        }
        # Persist after each variant to keep partial measurements if a later mode fails.
        $summary | Export-Csv -LiteralPath (Join-Path $resultsDir "$token-runtime-summary.csv") -NoTypeInformation -Encoding utf8
        $raw | Export-Csv -LiteralPath (Join-Path $resultsDir "$token-runtime-samples.csv") -NoTypeInformation -Encoding utf8
    }
    Write-Host ''
    Write-Host '=== COMPARISON: medians and PROCESS-wide resource deltas ==='
    $summary | Format-Table Mode,Strategy,MedianEfMs,ProcessCpuMsPerRequest,ProcessAllocatedKBPerRequest,Gen0CollectionsDelta,Gen1CollectionsDelta,Gen2CollectionsDelta -AutoSize
    Write-Host "Summary: $resultsDir/$token-runtime-summary.csv"
    Write-Host "Samples: $resultsDir/$token-runtime-samples.csv"
    Write-Host 'Diagnostic endpoint is enabled on the LAST tested API container. Restore default API to disable.'
} finally {
    Remove-Item Env:LAB_COMPILATION_MODE -ErrorAction SilentlyContinue
    Remove-Item Env:LAB_TIERED_COMPILATION -ErrorAction SilentlyContinue
    Remove-Item Env:LAB_RUNTIME_DIAGNOSTICS -ErrorAction SilentlyContinue
}
