[CmdletBinding()]
param(
    [string]$BaseUrl = "http://localhost:8080",
    [string]$ManagementUrl = "http://127.0.0.1:15672",
    [int]$TimeoutSeconds = 120
)
$ErrorActionPreference = "Stop"
$previousChaos = [Environment]::GetEnvironmentVariable("CHAOS_ENABLE_DLQ_PROBE", "Process")
$workerReconfigured = $false
$failure = $null
Push-Location (Join-Path $PSScriptRoot "..")

function Read-LabSettings {
    $settings = @{}
    foreach ($line in [IO.File]::ReadAllLines((Join-Path $PSScriptRoot "..\.env"))) {
        if ($line -match '^\s*([^#=\s]+)=(.*)$') {
            $settings[$Matches[1]] = $Matches[2].Trim()
        }
    }
    return $settings
}

try {
    docker compose config --quiet
    if ($LASTEXITCODE -ne 0) { throw "Docker Compose configuration invalid" }

    $settings = Read-LabSettings
    if (-not $settings["RABBIT_USER"] -or -not $settings["RABBIT_PASSWORD"]) {
        throw "RABBIT_USER or RABBIT_PASSWORD missing from .env"
    }
    $credentials = "$($settings['RABBIT_USER']):$($settings['RABBIT_PASSWORD'])"
    $basic = [Convert]::ToBase64String([Text.Encoding]::UTF8.GetBytes($credentials))
    $rabbitHeaders = @{ Authorization = "Basic $basic" }
    $dlqUri = "$ManagementUrl/api/queues/%2F/products.dead"
    $publishUri = "$ManagementUrl/api/exchanges/%2F/amq.default/publish"

    # Enable the persistent fault only in this local Worker and restore it in finally.
    $env:CHAOS_ENABLE_DLQ_PROBE = "true"
    $workerReconfigured = $true
    docker compose up -d --no-deps --force-recreate worker
    if ($LASTEXITCODE -ne 0) { throw "Worker recreate failed" }

    $since = [DateTime]::UtcNow.AddSeconds(-10).ToString("yyyy-MM-ddTHH:mm:ssZ")
    $consumerReady = $false
    for ($i = 0; $i -lt 30; $i++) {
        $startup = docker compose logs --since $since worker 2>&1 | Out-String
        if ($startup -match "RabbitMQ consumer started") { $consumerReady = $true; break }
        Start-Sleep -Seconds 1
    }
    if (-not $consumerReady) { throw "RabbitMQ consumer not ready" }

    $baseline = Invoke-RestMethod $dlqUri -Headers $rabbitHeaders -TimeoutSec 10
    $before = [int]$baseline.messages
    Write-Host "DLQ baseline: $before messages"

    # Scenario A: malformed JSON -> Permanent -> immediate BasicNack -> DLQ.
    $marker = [guid]::NewGuid().ToString("N")
    $invalidPayload = "not-valid-json-$marker"
    $publishBody = @{
        properties = @{ delivery_mode = 2; message_id = $marker }
        routing_key = "products.create"
        payload = $invalidPayload
        payload_encoding = "string"
    } | ConvertTo-Json -Depth 5
    $publish = Invoke-RestMethod $publishUri -Method Post -Headers $rabbitHeaders -Body $publishBody -ContentType "application/json" -TimeoutSec 10
    if (-not $publish.routed) { throw "Malformed message not routed into products.create" }

    $foundPermanent = $false
    for ($i = 0; $i -lt 30; $i++) {
        $queue = Invoke-RestMethod $dlqUri -Headers $rabbitHeaders -TimeoutSec 10
        if ([int]$queue.messages -ge $before + 1) { $foundPermanent = $true; break }
        Start-Sleep -Seconds 1
    }
    if (-not $foundPermanent) { throw "Malformed JSON was not dead-lettered" }
    Write-Host "PASS: malformed JSON -> immediate DLQ" -ForegroundColor Green

    # Scenario B: valid POST, but every Worker delivery fails before the SQL transaction.
    $token = & (Join-Path $PSScriptRoot "New-DemoJwt.ps1")
    $headers = @{ Authorization = "Bearer $token"; "Idempotency-Key" = [guid]::NewGuid().ToString() }
    $body = @{ name = ("DeadLetterProbe-" + [guid]::NewGuid().ToString("N")); price = 23.45 } | ConvertTo-Json
    $accepted = Invoke-RestMethod "$BaseUrl/products" -Method Post -Headers $headers -Body $body -ContentType "application/json"
    if (-not $accepted.operationId) { throw "POST did not return operationId" }
    $operationId = [string]$accepted.operationId
    Write-Host "Accepted operation $operationId"

    $failed = $false
    $deadline = [DateTime]::UtcNow.AddSeconds($TimeoutSeconds)
    do {
        $status = Invoke-RestMethod "$BaseUrl/operations/$operationId" -Headers $headers
        Write-Host "Operation $operationId -> $($status.status) / $($status.errorCode)"
        if ($status.status -eq "Completed") { throw "Persistent failure unexpectedly completed" }
        if ($status.status -eq "Failed") {
            if ($status.errorCode -ne "RETRIES_EXHAUSTED") {
                throw "Expected RETRIES_EXHAUSTED, received $($status.errorCode)"
            }
            $failed = $true
            break
        }
        Start-Sleep -Seconds 2
    } while ([DateTime]::UtcNow -lt $deadline)
    if (-not $failed) { throw "Persistent failure did not reach Failed" }

    $inDlq = $false
    for ($i = 0; $i -lt 30; $i++) {
        $queue = Invoke-RestMethod $dlqUri -Headers $rabbitHeaders -TimeoutSec 10
        if ([int]$queue.messages -ge $before + 2) { $inDlq = $true; break }
        Start-Sleep -Seconds 1
    }
    if (-not $inDlq) { throw "Expected at least two new DLQ messages" }

    $logs = docker compose logs --since $since worker 2>&1 | Out-String
    for ($attempt = 1; $attempt -le 4; $attempt++) {
        $expected = "DLQ probe: transient failure for $operationId delivery $attempt"
        if ($logs -notmatch [regex]::Escape($expected)) {
            throw "Missing Worker evidence: $expected"
        }
    }
    for ($attempt = 1; $attempt -le 3; $attempt++) {
        $expected = "Scheduled retry $operationId attempt $attempt via products.retry"
        if ($logs -notmatch [regex]::Escape($expected)) {
            throw "Missing Worker retry evidence: $expected"
        }
    }
    Write-Host "PASS: 3 retries -> Failed/RETRIES_EXHAUSTED -> DLQ" -ForegroundColor Green
    Write-Host "DLQ final: $($queue.messages) (baseline: $before; expected delta: +2)"
}
catch {
    $failure = $_
    Write-Warning "DLQ experiment failed: $($_.Exception.Message)"
    docker compose logs --tail=100 worker
}
finally {
    if ($null -eq $previousChaos) {
        Remove-Item Env:CHAOS_ENABLE_DLQ_PROBE -ErrorAction SilentlyContinue
    } else {
        $env:CHAOS_ENABLE_DLQ_PROBE = $previousChaos
    }
    if ($workerReconfigured) {
        docker compose up -d --no-deps --force-recreate worker
        if ($LASTEXITCODE -ne 0) {
            Write-Warning "CRITICAL: Worker configuration restore failed; check docker compose ps"
        } else {
            Write-Host "Worker configuration restored" -ForegroundColor Green
        }
    }
    Pop-Location
}
if ($failure) { throw $failure }
