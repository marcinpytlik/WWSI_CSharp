[CmdletBinding()]
param([string]$BaseUrl = "http://localhost:8080", [int]$TimeoutSeconds = 90)
$ErrorActionPreference = "Stop"
$previousChaos = [Environment]::GetEnvironmentVariable("CHAOS_ENABLE_RETRY_PROBE", "Process")
$workerReconfigured = $false
$failure = $null
Push-Location (Join-Path $PSScriptRoot "..")
try {
    docker compose config --quiet
    if ($LASTEXITCODE -ne 0) { throw "Docker Compose configuration invalid" }
    # Only lab Worker receives fault injection; API and SQL remain untouched.
    $env:CHAOS_ENABLE_RETRY_PROBE = "true"
    docker compose up -d --no-deps --force-recreate worker
    if ($LASTEXITCODE -ne 0) { throw "Cannot recreate Worker with retry probe enabled" }
    $workerReconfigured = $true
    $since = [DateTime]::UtcNow.AddSeconds(-10).ToString("yyyy-MM-ddTHH:mm:ssZ")
    $consumerReady = $false
    for ($i = 1; $i -le 30; $i++) {
        $startupLogs = docker compose logs --since $since worker 2>&1 | Out-String
        if ($startupLogs -match "RabbitMQ consumer started") { $consumerReady = $true; break }
        Start-Sleep -Seconds 1
    }
    if (-not $consumerReady) { throw "Worker consumer did not start" }

    $token = & (Join-Path $PSScriptRoot "New-DemoJwt.ps1")
    $headers = @{ Authorization = "Bearer $token"; "Idempotency-Key" = [guid]::NewGuid().ToString() }
    $probeName = "RetryProbe-" + [guid]::NewGuid().ToString("N")
    $body = @{ name = $probeName; price = 17.77 } | ConvertTo-Json
    $accepted = Invoke-RestMethod "$BaseUrl/products" -Method Post -Headers $headers -Body $body -ContentType "application/json"
    if (-not $accepted.operationId) { throw "POST did not return operation ID" }
    $operationId = [string]$accepted.operationId
    Write-Host "Accepted operation $operationId (initial status $($accepted.status))"

    $completed = $false
    $deadline = [DateTime]::UtcNow.AddSeconds($TimeoutSeconds)
    do {
        $status = Invoke-RestMethod "$BaseUrl/operations/$operationId" -Headers $headers
        Write-Host "Operation $operationId -> $($status.status)"
        if ($status.status -eq "Failed") { throw "Operation reached Failed instead of recovering" }
        if ($status.status -eq "Completed") { $completed = $true; break }
        Start-Sleep -Seconds 1
    } while ([DateTime]::UtcNow -lt $deadline)
    if (-not $completed) { throw "Operation did not complete before deadline" }

    $logs = docker compose logs --since $since worker 2>&1 | Out-String
    if ($logs -notmatch [regex]::Escape("Retry probe: transient failure for $operationId")) {
        throw "No evidence that transient failure was injected; check Worker logs"
    }
    if ($logs -notmatch [regex]::Escape("Scheduled retry $operationId attempt 1 via products.retry")) {
        throw "No evidence of scheduled retry into products.retry"
    }
    if ($logs -notmatch [regex]::Escape("Completed $operationId")) {
        throw "No evidence of Worker completion after retry"
    }
    Write-Host "PASS: transient failure -> products.retry -> completed (same operation)" -ForegroundColor Green
}
catch {
    $failure = $_
    Write-Warning "Retry test failed: $($_.Exception.Message)"
    docker compose logs --tail=100 worker
}
finally {
    if ($null -eq $previousChaos) { Remove-Item Env:CHAOS_ENABLE_RETRY_PROBE -ErrorAction SilentlyContinue }
    else { $env:CHAOS_ENABLE_RETRY_PROBE = $previousChaos }
    if ($workerReconfigured) {
        docker compose up -d --no-deps --force-recreate worker
        if ($LASTEXITCODE -ne 0) { Write-Warning "CRITICAL: failed to restore Worker; check docker compose ps" }
        else { Write-Host "Worker configuration restored (normal mode)" -ForegroundColor Green }
    }
    Pop-Location
}
if ($failure) { throw $failure }
