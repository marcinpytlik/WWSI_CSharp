[CmdletBinding()]
param(
    [guid]$OperationId = [guid]::Empty,
    [string]$BaseUrl = "http://localhost:8080",
    [int]$TimeoutSeconds = 90
)
$ErrorActionPreference = "Stop"
Push-Location (Join-Path $PSScriptRoot "..")
try {
    docker compose config --quiet
    if ($LASTEXITCODE -ne 0) { throw "Invalid Docker Compose configuration" }

    function Read-LabSql {
        param([string]$Sql)
        $lines = @(docker compose exec -T sql /bin/bash -c 'exec /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -d WwsiQueueCache -b -h -1 -W -w 8192 -Q "$1"' -- $Sql)
        if ($LASTEXITCODE -ne 0) { throw "SQL verification failed" }
        return ($lines -join [Environment]::NewLine).Trim()
    }
    function Get-Snapshot {
        param([guid]$Id)
        $query = "SET NOCOUNT ON; SELECT CONCAT((SELECT Status FROM dbo.OperationStatuses WHERE Id='$Id'),'|',COALESCE((SELECT ErrorCode FROM dbo.OperationStatuses WHERE Id='$Id'),'<null>'),'|',(SELECT COUNT_BIG(*) FROM dbo.Products WHERE Id=(SELECT ProductId FROM dbo.OperationStatuses WHERE Id='$Id')),'|',(SELECT COUNT_BIG(*) FROM dbo.ProcessedCommands WHERE Id='$Id'));"
        $result = Read-LabSql $query
        if ($result -notmatch '(?m)^(Pending|Failed|Completed)\|([^|]*)\|(\d+)\|(\d+)$') {
            throw "Operation was not found or invalid SQL snapshot: $result"
        }
        return @{ Status = $Matches[1]; ErrorCode = $Matches[2]; Products = [int]$Matches[3]; Processed = [int]$Matches[4] }
    }
    function Invoke-DlqCli {
        param([string[]]$CliArguments)
        docker compose --profile tools run --rm --no-deps dlq-replay @CliArguments
        if ($LASTEXITCODE -ne 0) { throw "DLQ operator tool failed (exit $LASTEXITCODE)" }
    }

    Write-Host "DLQ preview: manual ACK is disabled for inspection; no messages will be removed."
    if ($OperationId -eq [guid]::Empty) {
        Invoke-DlqCli -CliArguments @("preview")
        Write-Host "Choose a valid DeadLetterProbe operation GUID and rerun with -OperationId." -ForegroundColor Yellow
        return
    }

    $op = $OperationId.ToString()
    Invoke-DlqCli -CliArguments @("preview", $op)
    $before = Get-Snapshot -Id $OperationId
    Write-Host "SQL before: status=$($before.Status) error=$($before.ErrorCode) products=$($before.Products) processed=$($before.Processed)"
    if ($before.Status -ne "Failed" -or $before.ErrorCode -ne "RETRIES_EXHAUSTED" -or $before.Products -ne 0 -or $before.Processed -ne 0) {
        throw "Replay refused: expected Failed/RETRIES_EXHAUSTED, 0 Products, 0 ProcessedCommands"
    }

    $workerProbe = docker compose exec -T worker printenv Chaos__EnableDlqProbe
    if ($LASTEXITCODE -ne 0 -or ($workerProbe | Out-String).Trim() -ne "false") {
        throw "Replay refused: Worker Chaos__EnableDlqProbe is not confirmed disabled"
    }
    Write-Host "IMPORTANT: replay will move ONE selected lab message to products.create."
    Write-Host "Other DLQ messages (including malformed JSON) are left untouched."
    $typed = Read-Host "To authorize, type exactly: REPLAY $op"
    if ($typed -cne "REPLAY $op") {
        Write-Host "Cancelled. No replay performed."
        return
    }

    Invoke-DlqCli -CliArguments @("replay", $op, "--approve", $op)

    $deadline = [DateTime]::UtcNow.AddSeconds($TimeoutSeconds)
    $completed = $false
    do {
        $current = Get-Snapshot -Id $OperationId
        Write-Host "SQL after replay: status=$($current.Status) products=$($current.Products) processed=$($current.Processed)"
        if ($current.Status -eq "Completed") {
            if ($current.Products -ne 1 -or $current.Processed -ne 1) { throw "Exactly-once invariant failed" }
            $completed = $true
            break
        }
        Start-Sleep -Seconds 2
    } while ([DateTime]::UtcNow -lt $deadline)
    if (-not $completed) { throw "Replay not completed; inspect Worker logs and DLQ" }
    Write-Host "PASS: operator-approved DLQ replay -> Completed with one Product and one ProcessedCommand" -ForegroundColor Green
}
finally {
    Pop-Location
}
