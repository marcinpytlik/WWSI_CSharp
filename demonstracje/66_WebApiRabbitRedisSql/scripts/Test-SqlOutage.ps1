[CmdletBinding()]
param([string]$BaseUrl = "http://localhost:8080", [int]$ClientTimeoutSeconds = 30)
$ErrorActionPreference = "Stop"
Push-Location (Join-Path $PSScriptRoot "..")
$stopped = $false
$failed = $null
try {
    docker compose config --quiet
    if ($LASTEXITCODE -ne 0) { throw "Invalid Docker Compose configuration" }
    $token = & (Join-Path $PSScriptRoot "New-DemoJwt.ps1")
    $headers = @{ Authorization = "Bearer $token"; "Idempotency-Key" = [guid]::NewGuid().ToString() }
    $body = @{ name = "SQL outage timeout test"; price = 49.90 } | ConvertTo-Json
    docker compose stop sql
    if ($LASTEXITCODE -ne 0) { throw "Failed to stop SQL" }
    $stopped = $true
    $live = Invoke-WebRequest "$BaseUrl/health/live" -TimeoutSec 10
    if ($live.StatusCode -ne 200) { throw "Expected live 200" }
    Write-Host "PASS: liveness 200" -ForegroundColor Green
    $sw = [Diagnostics.Stopwatch]::StartNew()
    $ready = Invoke-WebRequest "$BaseUrl/health/ready" -SkipHttpErrorCheck -TimeoutSec $ClientTimeoutSeconds
    $sw.Stop()
    Write-Host "Readiness: HTTP $($ready.StatusCode) in $([math]::Round($sw.Elapsed.TotalSeconds,2)) s"
    if ($ready.StatusCode -ne 503) { throw "Expected readiness 503" }
    $sw.Restart()
    $post = Invoke-WebRequest "$BaseUrl/products" -Method Post -Headers $headers -Body $body -ContentType "application/json" -SkipHttpErrorCheck -TimeoutSec $ClientTimeoutSeconds
    $sw.Stop()
    Write-Host "POST: HTTP $($post.StatusCode) in $([math]::Round($sw.Elapsed.TotalSeconds,2)) s"
    if ($post.StatusCode -ne 503) { throw "Expected POST 503; if 202 inspect idempotency key and data before any retry" }
    Write-Host "PASS: SQL outage yields 503 for readiness and POST" -ForegroundColor Green
}
catch { $failed = $_; Write-Warning $_.Exception.Message }
finally {
    if ($stopped) {
        docker compose start sql
        if ($LASTEXITCODE -ne 0) { Write-Warning "SQL DID NOT RESTART: run docker compose start sql manually" }
        else {
            $healthy = $false
            for ($i = 0; $i -lt 60; $i++) {
                $id = docker compose ps -q sql
                if ($id) {
                    $state = docker inspect --format "{{.State.Health.Status}}" $id 2>$null
                    if ($state -eq "healthy") { $healthy = $true; break }
                }
                Start-Sleep -Seconds 2
            }
            if ($healthy) { Write-Host "PASS: SQL healthy again" -ForegroundColor Green }
            else { Write-Warning "SQL has not become healthy; check docker compose ps and SQL logs" }
        }
    }
    Pop-Location
}
if ($failed) { throw $failed }
