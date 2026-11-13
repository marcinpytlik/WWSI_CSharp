[CmdletBinding()]
param([string]$BaseUrl = "http://localhost:8080", [int]$TimeoutSeconds = 90)
$ErrorActionPreference = "Stop"
$previous = [Environment]::GetEnvironmentVariable("CHAOS_ENABLE_POST_COMMIT_PAUSE", "Process")
$needsRestore = $false
$failure = $null
Push-Location (Join-Path $PSScriptRoot "..")

function Get-DbCounts {
    param([guid]$OperationId, [guid]$ProductId)
    $query = "SET NOCOUNT ON; SELECT CONCAT((SELECT COUNT_BIG(*) FROM dbo.Products WHERE Id='$ProductId'),'|',(SELECT COUNT_BIG(*) FROM dbo.ProcessedCommands WHERE Id='$OperationId'),'|',(SELECT COUNT_BIG(*) FROM dbo.OperationStatuses WHERE Id='$OperationId' AND Status='Completed'));"
    $output = @(
        docker compose exec -T sql /bin/bash -c 'exec /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -d WwsiQueueCache -b -h -1 -W -w 8192 -Q "$1"' -- $query
    )
    if ($LASTEXITCODE -ne 0) { throw "SQL verification failed" }
    $text = ($output -join [Environment]::NewLine).Trim()
    if ($text -notmatch '(?m)^(\d+)\|(\d+)\|(\d+)$') {
        throw "Unexpected SQL result: $text"
    }
    return @{ Products = [int]$Matches[1]; ProcessedCommands = [int]$Matches[2]; Completed = [int]$Matches[3] }
}

function Assert-ExactlyOnce {
    param([guid]$OperationId, [guid]$ProductId, [string]$Stage)
    $counts = Get-DbCounts -OperationId $OperationId -ProductId $ProductId
    Write-Host "$Stage : Products=$($counts.Products), ProcessedCommands=$($counts.ProcessedCommands), Completed=$($counts.Completed)"
    if ($counts.Products -ne 1 -or $counts.ProcessedCommands -ne 1 -or $counts.Completed -ne 1) {
        throw "SQL exactly-once invariant broken at $Stage"
    }
}

try {
    docker compose config --quiet
    if ($LASTEXITCODE -ne 0) { throw "Invalid Compose configuration" }

    $env:CHAOS_ENABLE_POST_COMMIT_PAUSE = "true"
    $needsRestore = $true
    docker compose up -d --no-deps --force-recreate worker
    if ($LASTEXITCODE -ne 0) { throw "Cannot enable Worker crash probe" }
    $since = [DateTime]::UtcNow.AddSeconds(-10).ToString("yyyy-MM-ddTHH:mm:ssZ")

    $started = $false
    for ($i = 0; $i -lt 30; $i++) {
        $logs = docker compose logs --since $since worker 2>&1 | Out-String
        if ($logs -match "RabbitMQ consumer started") { $started = $true; break }
        Start-Sleep -Seconds 1
    }
    if (-not $started) { throw "Worker consumer is not ready" }

    $token = & (Join-Path $PSScriptRoot "New-DemoJwt.ps1")
    $headers = @{ Authorization = "Bearer $token"; "Idempotency-Key" = [guid]::NewGuid().ToString() }
    $name = "CrashProbe-" + [guid]::NewGuid().ToString("N")
    $body = @{ name = $name; price = 31.25 } | ConvertTo-Json
    $accepted = Invoke-RestMethod "$BaseUrl/products" -Method Post -Headers $headers -Body $body -ContentType "application/json"
    $operationId = [guid]$accepted.operationId
    $productId = [guid]$accepted.productId
    if ($operationId -eq [guid]::Empty -or $productId -eq [guid]::Empty) { throw "Missing operation identifiers" }
    Write-Host "Created operation $operationId"

    # The pause log is produced AFTER db transaction CommitAsync returned.
    $pauseSeen = $false
    $deadline = [DateTime]::UtcNow.AddSeconds($TimeoutSeconds)
    do {
        $logs = docker compose logs --since $since worker 2>&1 | Out-String
        if ($logs.Contains("POST_COMMIT_PRE_ACK_PAUSE $operationId")) { $pauseSeen = $true; break }
        Start-Sleep -Milliseconds 500
    } while ([DateTime]::UtcNow -lt $deadline)
    if (-not $pauseSeen) { throw "No post-COMMIT/pre-ACK marker; refusing to kill Worker" }

    Assert-ExactlyOnce -OperationId $operationId -ProductId $productId -Stage "Before crash"
    $preStatus = Invoke-RestMethod "$BaseUrl/operations/$operationId" -Headers $headers
    if ($preStatus.status -ne "Completed") { throw "SQL transaction not Completed before crash" }
    Write-Host "PASS: SQL COMMIT persisted; RabbitMQ ACK deliberately paused" -ForegroundColor Green

    # Kill ONLY the Worker process; no graceful dispose / no BasicAck.
    docker compose kill -s SIGKILL worker
    if ($LASTEXITCODE -ne 0) { throw "Could not SIGKILL Worker" }
    Write-Host "Worker terminated using SIGKILL after SQL COMMIT" -ForegroundColor Yellow

    # Restore normal mode before starting the replacement consumer.
    $env:CHAOS_ENABLE_POST_COMMIT_PAUSE = "false"
    docker compose up -d --no-deps --force-recreate worker
    if ($LASTEXITCODE -ne 0) { throw "Could not restart Worker" }
    $needsRestore = $false

    $redelivered = $false
    $completed = $false
    $deadline = [DateTime]::UtcNow.AddSeconds($TimeoutSeconds)
    do {
        $logs = docker compose logs --since $since worker 2>&1 | Out-String
        if ($logs -match [regex]::Escape("Crash probe delivery $operationId redelivered=True")) { $redelivered = $true }
        if ($logs -match [regex]::Escape("Completed $operationId")) { $completed = $true }
        if ($redelivered -and $completed) { break }
        Start-Sleep -Seconds 1
    } while ([DateTime]::UtcNow -lt $deadline)
    if (-not $redelivered) { throw "RabbitMQ redelivery flag was not observed" }
    if (-not $completed) { throw "Worker did not ACK/complete redelivered command" }

    Assert-ExactlyOnce -OperationId $operationId -ProductId $productId -Stage "After redelivery"
    $after = Invoke-RestMethod "$BaseUrl/operations/$operationId" -Headers $headers
    if ($after.status -ne "Completed") { throw "Operation no longer Completed after redelivery" }

    Write-Host "PASS: COMMIT -> SIGKILL before ACK -> RabbitMQ redelivery -> exactly one SQL business effect" -ForegroundColor Green
}
catch {
    $failure = $_
    Write-Warning "Commit/ACK crash test failed: $($_.Exception.Message)"
    docker compose logs --tail=100 worker
}
finally {
    if ($null -eq $previous) {
        Remove-Item Env:CHAOS_ENABLE_POST_COMMIT_PAUSE -ErrorAction SilentlyContinue
    } else {
        $env:CHAOS_ENABLE_POST_COMMIT_PAUSE = $previous
    }
    if ($needsRestore) {
        docker compose up -d --no-deps --force-recreate worker
        if ($LASTEXITCODE -ne 0) { Write-Warning "CRITICAL: failed to restore Worker; inspect docker compose ps" }
        else { Write-Host "Worker configuration restored" -ForegroundColor Green }
    }
    Pop-Location
}
if ($failure) { throw $failure }
