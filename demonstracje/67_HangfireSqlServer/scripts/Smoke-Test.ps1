[CmdletBinding()]
param([string]$BaseUrl="http://localhost:8081",[int]$TimeoutSeconds=120)
$ErrorActionPreference="Stop"
function Wait-Succeeded([string]$JobId) {
    $deadline=[DateTime]::UtcNow.AddSeconds($TimeoutSeconds)
    do {
        $state=Invoke-RestMethod "$BaseUrl/jobs/$JobId"
        Write-Host "$JobId -> $($state.latestState)"
        if ($state.latestState -eq "Succeeded") { return }
        if ($state.latestState -eq "Deleted") { throw "Job $JobId deleted" }
        Start-Sleep -Seconds 2
    } while ([DateTime]::UtcNow -lt $deadline)
    throw "Job $JobId not completed"
}
if ((Invoke-RestMethod "$BaseUrl/health/live").status -ne "alive") { throw "API unavailable" }
$fire=Invoke-RestMethod "$BaseUrl/jobs/fire" -Method Post -ContentType application/json -Body '{"workSeconds":0}'
Wait-Succeeded $fire.jobId
Write-Host "PASS: fire-and-forget" -ForegroundColor Green
$delayed=Invoke-RestMethod "$BaseUrl/jobs/delay" -Method Post -ContentType application/json -Body '{"workSeconds":0,"delaySeconds":8}'
Wait-Succeeded $delayed.jobId
Write-Host "PASS: delayed" -ForegroundColor Green
$chain=Invoke-RestMethod "$BaseUrl/jobs/continuation" -Method Post
Wait-Succeeded $chain.parentJobId
Wait-Succeeded $chain.childJobId
Write-Host "PASS: continuation" -ForegroundColor Green
