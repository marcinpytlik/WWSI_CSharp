param(
    [string]$BaseUrl = "http://localhost:8080",
    [int]$TimeoutSeconds = 90
)
$ErrorActionPreference = "Stop"
$token = & (Join-Path $PSScriptRoot "New-DemoJwt.ps1")
$headers = @{ Authorization = "Bearer $token"; "Idempotency-Key" = [guid]::NewGuid().ToString() }
$body = @{ name = "WWSI production-path smoke"; price = 129.99 } | ConvertTo-Json
$live = Invoke-RestMethod "$BaseUrl/health/live"
if ($live.status -ne "alive") { throw "Liveness check failed" }
$accepted = Invoke-RestMethod "$BaseUrl/products" -Method Post -Headers $headers -Body $body -ContentType "application/json"
if (-not $accepted.operationId) { throw "No operationId returned" }
$duplicate = Invoke-RestMethod "$BaseUrl/products" -Method Post -Headers $headers -Body $body -ContentType "application/json"
if ($accepted.operationId -ne $duplicate.operationId) { throw "Idempotency failed" }
$otherToken = & (Join-Path $PSScriptRoot "New-DemoJwt.ps1") -Subject "student02"
$otherHeaders = @{ Authorization = "Bearer $otherToken" }

$deadline = [DateTime]::UtcNow.AddSeconds($TimeoutSeconds)
do {
    Start-Sleep -Seconds 1
    $status = Invoke-RestMethod "$BaseUrl/operations/$($accepted.operationId)" -Headers $headers
    if ($status.status -eq "Failed") { throw "Operation failed" }
} while ($status.status -ne "Completed" -and [DateTime]::UtcNow -lt $deadline)
if ($status.status -ne "Completed") { throw "Operation never completed" }
$url = "$BaseUrl/products/$($accepted.productId)"
$r1 = Invoke-WebRequest $url -Headers $headers
$r2 = Invoke-WebRequest $url -Headers $headers
if ($r1.Headers["X-Cache"] -notcontains "MISS") { throw "Expected cache miss" }
if ($r2.Headers["X-Cache"] -notcontains "HIT") { throw "Expected cache hit" }
$forbidden = Invoke-WebRequest $url -Headers $otherHeaders -SkipHttpErrorCheck
if ($forbidden.StatusCode -ne 404) { throw "Cross-principal product access was not denied" }
Write-Host "PASS: accepted -> idempotent -> completed -> MISS -> HIT -> tenant isolation" -ForegroundColor Green


$bad = Invoke-WebRequest "$BaseUrl/products" -Method Post -Headers $headers -Body (@{ name = ""; price = -1 } | ConvertTo-Json) -ContentType "application/json" -SkipHttpErrorCheck
if ($bad.StatusCode -ne 400) { throw "Expected HTTP 400 validation error" }
$badProblem = $bad.Content | ConvertFrom-Json
if ($badProblem.code -ne "VALIDATION_ERROR" -or -not $badProblem.traceId) { throw "Validation ProblemDetails contract failed" }
$conflict = Invoke-WebRequest "$BaseUrl/products" -Method Post -Headers $headers -Body (@{ name = "DIFFERENT"; price = 1.00 } | ConvertTo-Json) -ContentType "application/json" -SkipHttpErrorCheck
if ($conflict.StatusCode -ne 409) { throw "Expected HTTP 409 idempotency conflict" }
$unknown = Invoke-WebRequest "$BaseUrl/operations/$([guid]::NewGuid())" -Headers $headers -SkipHttpErrorCheck
if ($unknown.StatusCode -ne 404) { throw "Expected HTTP 404 missing operation" }
$unauthorized = Invoke-WebRequest "$BaseUrl/operations/$($accepted.operationId)" -SkipHttpErrorCheck
if ($unauthorized.StatusCode -ne 401) { throw "Expected HTTP 401 unauthenticated" }
Write-Host "PASS: error contracts 400 / 409 / 404 / 401" -ForegroundColor Green
