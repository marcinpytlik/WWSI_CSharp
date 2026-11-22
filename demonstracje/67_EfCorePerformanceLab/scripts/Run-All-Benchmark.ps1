<#
.SYNOPSIS
Run all four EF Core Performance Lab strategies and compare SQL Server Query Store deltas.
.DESCRIPTION
Requires PowerShell 7, Docker Compose and an already-running demo 67.
Runs Test-Lab once for warmup, then for each strategy:
BEFORE snapshot -> isolated benchmark -> AFTER snapshot.
Reads SQL metrics from Query Store and prints combined application + SQL results.
Never resets Query Store or SQL database. Do not run concurrent workloads.
#>
[CmdletBinding()]
param(
    [string] $BaseUrl = 'http://localhost:5087',
    [ValidateRange(1, 100)] [int] $Take = 100,
    [ValidateRange(1, 100)] [int] $Repetitions = 10
)
$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
$root = Split-Path -Parent $PSScriptRoot
$repoScript = Join-Path $PSScriptRoot 'Run-Benchmark.ps1'
$snapshotTemplate = Get-Content (Join-Path $root 'sql/01_QueryStoreSnapshot.sql') -Raw
$compareTemplate = Get-Content (Join-Path $root 'sql/03_QueryStoreCompareAll.sql') -Raw
$token = [guid]::NewGuid().ToString('N').Substring(0, 8)
$strategies = @('nplusone', 'include', 'projection', 'split')
$allResults = [System.Collections.Generic.List[object]]::new()

function Invoke-LabSql([string] $Sql) {
    # SQLCMDPASSWORD is resolved inside the SQL container. Password is not sent as a CLI argument.
    $command = 'export SQLCMDPASSWORD="$MSSQL_SA_PASSWORD"; exec /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -C -d EfPerformanceLab -b -r1 -i /dev/stdin'
    $lines = @($Sql | docker compose --project-directory $root exec -T sql bash -c $command 2>&1)
    if ($LASTEXITCODE -ne 0) {
        throw "Query Store SQL failed (exit code $LASTEXITCODE): $($lines -join [Environment]::NewLine)"
    }
    return $lines
}

Write-Host "=== EF Core Performance Lab: controlled run $token ==="
Write-Host 'Prerequisite: SQL Server Query Store READ_WRITE, capture mode ALL.'
Write-Host 'Warmup + data/command assertions...'
& (Join-Path $PSScriptRoot 'Test-Lab.ps1') -BaseUrl $BaseUrl -Take $Take

foreach ($strategy in $strategies) {
    Write-Host ""
    Write-Host "=== $strategy : BEFORE / benchmark / AFTER ==="
    foreach ($phase in @('BEFORE','AFTER')) {
        if ($phase -eq 'AFTER') {
            & $repoScript -BaseUrl $BaseUrl -Take $Take -Repetitions $Repetitions -Strategy $strategy -SkipWarmup
            $path = Join-Path $PSScriptRoot 'benchmark-results.csv'
            $records = @(Import-Csv -LiteralPath $path)
            if ($records.Count -ne $Repetitions) { throw "Unexpected benchmark row count for $strategy" }
            foreach ($record in $records) { $allResults.Add($record) }
        }
        $label = "${phase}:${strategy}:$token"
        $statement = $snapshotTemplate.Replace("DECLARE @Label nvarchar(30) = N'BEFORE';", "DECLARE @Label nvarchar(30) = N'$label';")
        if ($statement -eq $snapshotTemplate) { throw 'Snapshot template label not found; check SQL file revision.' }
        $message = @(Invoke-LabSql $statement)
        Write-Host "Snapshot $label : $($message.Count) output lines"
    }
}
Write-Host ''
Write-Host '=== Comparing Query Store snapshots ==='
$comparison = $compareTemplate.Replace('__RUN_TOKEN__', $token)
$sqlOutput = @(Invoke-LabSql $comparison)
$server = @{}
foreach ($line in $sqlOutput) {
    $line = [string]$line
    $pos = $line.IndexOf('EFPLAB_RESULT|')
    if ($pos -lt 0) { continue }
    $fields = $line.Substring($pos).Trim().Split('|')
    if ($fields.Count -ne 6) { throw "Invalid SQL comparison row: $line" }
    $server[$fields[1]] = [pscustomobject]@{
        Commands = [int]::Parse($fields[2], [cultureinfo]::InvariantCulture)
        DurationMs = [double]::Parse($fields[3], [cultureinfo]::InvariantCulture)
        CpuMs = [double]::Parse($fields[4], [cultureinfo]::InvariantCulture)
        LogicalReads = [long]::Parse($fields[5], [cultureinfo]::InvariantCulture)
    }
}

$expected = @{
    nplusone = $Repetitions * ($Take + 1)
    include = $Repetitions
    projection = $Repetitions
    split = $Repetitions * 2
}
$summary = foreach ($strategy in $strategies) {
    if (-not $server.ContainsKey($strategy)) {
        throw "Missing Query Store data for $strategy. SQL output: $($sqlOutput -join '; ')"
    }
    $sql = $server[$strategy]
    if ($sql.Commands -ne $expected[$strategy]) {
        throw "Query Store mismatch for ${strategy}: expected $($expected[$strategy]) SQL commands, captured $($sql.Commands). Check isolation and Query Store flush."
    }
    $rows = @($allResults | Where-Object { $_.Strategy -eq $strategy })
    $times = @($rows | ForEach-Object { [double]::Parse($_.ElapsedMs, [cultureinfo]::CurrentCulture) } | Sort-Object)
    $half = [int][math]::Floor($times.Count / 2)
    $median = if ($times.Count % 2 -eq 1) { $times[$half] } else { ($times[$half - 1] + $times[$half]) / 2.0 }
    [pscustomobject]@{
        Strategy = $strategy
        MedianEfMs = [math]::Round($median, 2)
        SqlCommandsPerRequest = [int]($sql.Commands / $Repetitions)
        SqlCpuMsPerRequest = [math]::Round($sql.CpuMs / $Repetitions, 3)
        SqlDurationMsPerRequest = [math]::Round($sql.DurationMs / $Repetitions, 3)
        SqlLogicalReadsPerRequest = [math]::Round($sql.LogicalReads / $Repetitions, 1)
        Requests = $Repetitions
    }
}
Write-Host ''
Write-Host '=== COMPARISON: application median vs SQL Server per request ==='
$summary | Format-Table -AutoSize
$summaryPath = Join-Path $PSScriptRoot "comparison-$token.csv"
$detailPath = Join-Path $PSScriptRoot "benchmark-all-$token.csv"
$summary | Export-Csv -LiteralPath $summaryPath -Encoding utf8 -NoTypeInformation
$allResults | Export-Csv -LiteralPath $detailPath -Encoding utf8 -NoTypeInformation
Write-Host "Summary CSV: $summaryPath"
Write-Host "Raw samples: $detailPath"
Write-Host "Run token: $token"
