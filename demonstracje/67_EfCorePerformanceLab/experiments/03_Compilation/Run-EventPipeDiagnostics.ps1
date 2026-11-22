<#
.SYNOPSIS
Lab 67 / Experiment 03D: dotnet-counters and dotnet-trace using a separate SDK sidecar.
.DESCRIPTION
Captures two SEPARATE diagnostic sessions per compilation mode to avoid competing EventPipe
clients. Each session runs the same four EF Core strategies; output is counters CSV / .nettrace.
The SQL container and its data volume are preserved. Diagnostic collection changes runtime
overhead and the resulting times are NOT directly comparable with uninstrumented baselines.
#>
[CmdletBinding()]
param(
    [ValidateSet('jit','r2r','composite','no-tiering')]
    [string[]] $Modes = @('jit'),
    [ValidateRange(1,100)] [int] $Take = 100,
    [ValidateRange(5,100)] [int] $Repetitions = 20,
    [ValidateRange(30,3600)] [int] $CaptureSeconds = 300,
    [switch] $SkipTrace
)
$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
$root = (Resolve-Path (Join-Path $PSScriptRoot '../..')).Path
$resultsDir = Join-Path $PSScriptRoot 'results'
New-Item -ItemType Directory -Force -Path $resultsDir | Out-Null
$base = Join-Path $root 'docker-compose.yml'
$compilation = Join-Path $root 'docker-compose.compilation.yml'
$diagnostics = Join-Path $root 'docker-compose.diagnostics.yml'
$token = [guid]::NewGuid().ToString('N').Substring(0,8)
$files = @('-f', $base, '-f', $compilation, '-f', $diagnostics)
$manifest = [System.Collections.Generic.List[object]]::new()

function Invoke-Compose([string[]] $Arguments) {
    & docker compose --project-directory $root @files @Arguments
    if ($LASTEXITCODE -ne 0) { throw "docker compose failed: $($Arguments -join ' ')" }
}
function Wait-ApiReady {
    # Windows Docker Desktop may temporarily accept connections but return HTTP 499
    # when the client closes a very short (2s) readiness request.
    $watch = [Diagnostics.Stopwatch]::StartNew()
    $attempt = 0
    $lastFailure = 'No response yet'
    while ($watch.Elapsed.TotalSeconds -lt 180) {
        $attempt++
        try {
            $health = Invoke-RestMethod -Uri 'http://127.0.0.1:5087/health' -TimeoutSec 10 -ErrorAction Stop
            if ($health.status -eq 'ready') {
                Write-Host ("API ready after {0:N1}s (attempt {1})" -f $watch.Elapsed.TotalSeconds,$attempt)
                return
            }
            $lastFailure = "Unexpected health status: $($health.status)"
        } catch {
            $lastFailure = $_.Exception.Message
        }
        if (($attempt % 5) -eq 0) {
            Write-Host ("Waiting for API: {0:N1}s, attempt {1}; last: {2}" -f $watch.Elapsed.TotalSeconds,$attempt,$lastFailure)
        }
        Start-Sleep -Milliseconds 700
    }
    Write-Warning "API readiness timed out. Last error: $lastFailure"
    & docker compose --project-directory $root @files ps -a
    & docker compose --project-directory $root @files logs --tail=60 api
    throw "API did not become ready after 180s. Last error: $lastFailure"
}
function Run-Workload([string] $Mode, [string] $Capture) {
    $output = Join-Path $resultsDir "$token-$Mode-$Capture-workload.csv"
    $strategies = @('nplusone','include','projection','split')
    $samples = [System.Collections.Generic.List[object]]::new()
    for ($iteration = 1; $iteration -le $Repetitions; $iteration++) {
        foreach ($strategy in $strategies) {
            $response = Invoke-RestMethod "http://localhost:5087/api/orders/${strategy}?take=$Take" -TimeoutSec 60
            $expect = switch ($strategy) {
                'nplusone' { $Take + 1 }
                'split' { 2 }
                default { 1 }
            }
            if ($response.orders -ne $Take -or $response.sqlCommands -ne $expect) {
                throw "Invalid workload response: $Mode/$Capture/$strategy"
            }
            $samples.Add([pscustomobject]@{
                Token=$token; Mode=$Mode; Capture=$Capture
                Iteration=$iteration; Strategy=$strategy
                ElapsedMs=[double]$response.elapsedMs; SqlCommands=[int]$response.sqlCommands
            })
        }
    }
    $samples | Export-Csv -LiteralPath $output -Encoding utf8 -NoTypeInformation
    Write-Host "Workload: $output"
    return $output
}
function Verify-Output([string] $Filename) {
    $path = Join-Path $resultsDir $Filename
    if (-not (Test-Path -LiteralPath $path)) {
        throw "Missing diagnostic artifact $path. Inspect results/*-capture.log and profiler logs."
    }
    $item = Get-Item -LiteralPath $path
    if ($item.Length -lt 30) {
        throw "Diagnostic artifact is empty: $path. Inspect matching capture.log."
    }
    return $item.Length
}
function Capture([string] $Mode, [ValidateSet('counters','trace')] [string] $Kind) {
    $stamp = "$token-$Mode-$Kind"
    $artifact = if ($Kind -eq 'counters') { "$stamp.csv" } else { "$stamp.nettrace" }
    $log = "$stamp-capture.log"
    $exitFile = "$stamp.exit"
    # Both collectors use the same PTY / FIFO control channel.
    # dotnet-trace receives Enter; dotnet-counters receives q.
    # CaptureSeconds remains a safety limit for the total workload duration.
    Write-Host "Starting interactive PTY $Kind session: $stamp"
    Invoke-Compose -Arguments @('exec','-d','profiler','/bin/bash','/tools/collect-counters.sh',$stamp,$Kind)
    $fifoReady = $false
    $fifo = "/tmp/$stamp.fifo"
    for ($n = 0; $n -lt 40; $n++) {
        & docker compose --project-directory $root @files exec -T profiler test -p $fifo 2>$null
        if ($LASTEXITCODE -eq 0) { $fifoReady = $true; break }
        Start-Sleep -Milliseconds 250
    }
    if (-not $fifoReady) { throw "Diagnostic PTY/FIFO did not initialize for $stamp. Inspect $log" }
    Start-Sleep -Seconds 4
    $workloadWatch = [Diagnostics.Stopwatch]::StartNew()
    $workloadCsv = Run-Workload $Mode $Kind
    $workloadWatch.Stop()
    if ($workloadWatch.Elapsed.TotalSeconds -gt $CaptureSeconds) {
        Write-Warning ("Workload took {0:N1}s, exceeding the advisory CaptureSeconds={1}s. Trace remains complete because capture stops after workload." -f $workloadWatch.Elapsed.TotalSeconds,$CaptureSeconds)
    }
    $inputCommand = if ($Kind -eq 'counters') { "printf q > /tmp/$stamp.fifo" } else { "printf '\\n' > /tmp/$stamp.fifo" }
    Invoke-Compose -Arguments @('exec','-T','profiler','bash','-c',$inputCommand)
    $exitPath = Join-Path $resultsDir $exitFile
    $wait = [Diagnostics.Stopwatch]::StartNew()
    $maxWait = 45
    while (-not (Test-Path -LiteralPath $exitPath) -and $wait.Elapsed.TotalSeconds -lt $maxWait) {
        Start-Sleep -Milliseconds 500
    }
    if (-not (Test-Path -LiteralPath $exitPath)) {
        $logPath = Join-Path $resultsDir $log
        $details = if (Test-Path -LiteralPath $logPath) {
            (Get-Content -LiteralPath $logPath -Tail 30) -join '; '
        } else { 'Capture log missing' }
        throw "Capture $stamp did not close cleanly. Log: $details"
    }
    $exitCode = (Get-Content -LiteralPath $exitPath -Raw).Trim()
    if ($exitCode -ne '0') { throw "Capture $stamp exit code=$exitCode. Inspect $log" }
    $size = Verify-Output $artifact
    $manifest.Add([pscustomobject]@{
        Token=$token; Mode=$Mode; Capture=$Kind
        CaptureSeconds=$CaptureSeconds; WorkloadRepetitions=$Repetitions
        WorkloadSeconds=[math]::Round($workloadWatch.Elapsed.TotalSeconds,2)
        Artifact=$artifact; ArtifactBytes=$size
        WorkloadCsv=(Split-Path $workloadCsv -Leaf)
        Log=$log; ToolExitCode=$exitCode; CollectedUtc=(Get-Date).ToUniversalTime().ToString('o')
    })
    $manifest | Export-Csv -LiteralPath (Join-Path $resultsDir "$token-manifest.csv") -NoTypeInformation -Encoding utf8
    Write-Host "PASS $Mode $Kind : $artifact ($size bytes)"
}
$running = @(docker compose --project-directory $root ps --services --status running)
if ($LASTEXITCODE -ne 0 -or 'sql' -notin $running) {
    throw 'SQL container is not running. Execute docker compose up -d sql first.'
}
try {
    $env:LAB_RUNTIME_DIAGNOSTICS = 'false'
    foreach ($mode in $Modes) {
        $env:LAB_COMPILATION_MODE = if ($mode -eq 'no-tiering') { 'jit' } else { $mode }
        $env:LAB_TIERED_COMPILATION = if ($mode -eq 'no-tiering') { '0' } else { '1' }
        Write-Host "=== $mode / compiling with diagnostic /tmp mount ==="
        Invoke-Compose -Arguments @('build','api','profiler')
        Invoke-Compose -Arguments @('up','-d','--no-deps','--force-recreate','--no-build','api')
        Wait-ApiReady
        # Sidecar shares PID namespace and /tmp with target process.
        Invoke-Compose -Arguments @('up','-d','--no-deps','--force-recreate','--no-build','profiler')
        $processList = @(& docker compose --project-directory $root @files exec -T profiler /tools/dotnet-counters ps 2>&1)
        if ($LASTEXITCODE -ne 0 -or ($processList -join ' ') -notmatch 'PerformanceLab') {
            throw "Diagnostic attach preflight failed. dotnet-counters ps: $($processList -join '; ')"
        }
        Write-Host "Attach preflight PASS: $($processList -join ' ')"
        & (Join-Path $root 'scripts/Test-Lab.ps1') -Take $Take
        if (-not $?) { throw "Smoke test failed for $mode" }
        # A little uninstrumented warmup before attaching.
        foreach ($strategy in @('nplusone','include','projection','split')) {
            $null = Invoke-RestMethod "http://localhost:5087/api/orders/${strategy}?take=$Take"
        }
        Capture $mode 'counters'
        if (-not $SkipTrace) { Capture $mode 'trace' }
    }
    Write-Host "DONE. Results: $resultsDir"
    Write-Host "Manifest: $token-manifest.csv"
    Write-Host "Use dotnet-trace convert or PerfView for .nettrace analysis; CSV counters are time-series."
} finally {
    Remove-Item Env:LAB_COMPILATION_MODE -ErrorAction SilentlyContinue
    Remove-Item Env:LAB_TIERED_COMPILATION -ErrorAction SilentlyContinue
    Remove-Item Env:LAB_RUNTIME_DIAGNOSTICS -ErrorAction SilentlyContinue
}
