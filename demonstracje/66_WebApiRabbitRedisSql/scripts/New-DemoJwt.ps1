param(
    [string]$Subject = "student01",
    [string]$EnvFile = (Join-Path $PSScriptRoot "..\.env")
)
$ErrorActionPreference = "Stop"
$settings = @{}
foreach ($line in [System.IO.File]::ReadAllLines((Resolve-Path $EnvFile))) {
    if ($line -match '^\s*([^#=\s]+)=(.*)$') { $settings[$Matches[1]] = $Matches[2].Trim() }
}
$key = $settings["JWT_SIGNING_KEY"]
if (-not $key -or [Text.Encoding]::UTF8.GetByteCount($key) -lt 32) { throw "JWT_SIGNING_KEY missing or too short" }
function To-Base64Url([byte[]]$bytes) {
    return [Convert]::ToBase64String($bytes).TrimEnd('=').Replace('+','-').Replace('/','_')
}
$now = [DateTimeOffset]::UtcNow.ToUnixTimeSeconds()
$header = To-Base64Url ([Text.Encoding]::UTF8.GetBytes('{"alg":"HS256","typ":"JWT"}'))
$payloadJson = @{
    sub = $Subject
    iss = $settings["JWT_ISSUER"]
    aud = $settings["JWT_AUDIENCE"]
    iat = $now
    exp = $now + 3600
} | ConvertTo-Json -Compress
$payload = To-Base64Url ([Text.Encoding]::UTF8.GetBytes($payloadJson))
$unsigned = "$header.$payload"
$hmac = [Security.Cryptography.HMACSHA256]::new([Text.Encoding]::UTF8.GetBytes($key))
try { $signature = To-Base64Url ($hmac.ComputeHash([Text.Encoding]::UTF8.GetBytes($unsigned))) }
finally { $hmac.Dispose() }
"$unsigned.$signature"
