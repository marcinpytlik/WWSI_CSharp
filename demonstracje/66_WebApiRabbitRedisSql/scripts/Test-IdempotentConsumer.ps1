[CmdletBinding()]
param(
    [string]$BaseUrl = "http://localhost:8080",
    [string]$ManagementUrl = "http://127.0.0.1:15672",
    [int]$TimeoutSeconds = 90
)
$ErrorActionPreference = "Stop"
Push-Location (Join-Path $PSScriptRoot "..")
try {
    docker compose config --quiet
    if ($LASTEXITCODE -ne 0) { throw "Invalid Docker Compose configuration" }

    # SQL validation uses the SA password already present INSIDE the SQL lab container.
    # Never print secrets or pass passwords through the host PowerShell command line.
    function Invoke-LabSql {
        param([Parameter(Mandatory)][string]$Query)
        $output = @(
            docker compose exec -T sql /bin/bash -c 'exec /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -d WwsiQueueCache -b -h -1 -y 0 -w 8192 -Q "$1"' -- $Query
        )
        if ($LASTEXITCODE -ne 0) { throw "SQL command failed" }
        return ($output -join [Environment]::NewLine).Trim()
    }

    function Get-SqlCounts {
        param([Parameter(Mandatory)][guid]$OperationId, [Parameter(Mandatory)][guid]$ProductId)
        $query = "SET NOCOUNT ON; SELECT CONCAT((SELECT COUNT_BIG(*) FROM dbo.Products WHERE Id = '$ProductId'),'|',(SELECT COUNT_BIG(*) FROM dbo.ProcessedCommands WHERE Id = '$OperationId'),'|',(SELECT COUNT_BIG(*) FROM dbo.OperationStatuses WHERE Id = '$OperationId' AND Status = 'Completed'));"
        $result = Invoke-LabSql $query
        if ($result -notmatch '(?m)^(\d+)\|(\d+)\|(\d+)$') {
            throw "Unexpected SQL counts output: $result"
        }
        return @{ Products = [int]$Matches[1]; ProcessedCommands = [int]$Matches[2]; CompletedStatuses = [int]$Matches[3] }
    }

    function Wait-CompletedLog {
        param([string]$OperationId, [string]$Since, [int]$Minimum)
        for ($i = 0; $i -lt 30; $i++) {
            $logs = docker compose logs --since $Since worker 2>&1 | Out-String
            $pattern = [regex]::Escape("Completed $OperationId")
            $hits = [regex]::Matches($logs, $pattern).Count
            if ($hits -ge $Minimum) { return $hits }
            Start-Sleep -Seconds 1
        }
        throw "Missing Worker ACK/completion evidence: expected $Minimum logs for operation $OperationId"
    }

    $settings = @{}
    foreach ($line in [IO.File]::ReadAllLines((Join-Path $PSScriptRoot "..\.env"))) {
        if ($line -match '^\s*([^#=\s]+)=(.*)$') { $settings[$Matches[1]] = $Matches[2].Trim() }
    }
    if (-not $settings["RABBIT_USER"] -or -not $settings["RABBIT_PASSWORD"]) {
        throw "Missing RabbitMQ credentials in .env"
    }
    $credentials = "$($settings['RABBIT_USER']):$($settings['RABBIT_PASSWORD'])"
    $rabbitHeaders = @{
        Authorization = "Basic $([Convert]::ToBase64String([Text.Encoding]::UTF8.GetBytes($credentials)))"
    }
    $publishUri = "$ManagementUrl/api/exchanges/%2F/amq.default/publish"

    $since = [DateTime]::UtcNow.AddSeconds(-5).ToString("yyyy-MM-ddTHH:mm:ssZ")
    $token = & (Join-Path $PSScriptRoot "New-DemoJwt.ps1")
    $headers = @{
        Authorization = "Bearer $token"
        "Idempotency-Key" = [guid]::NewGuid().ToString()
    }
    $body = @{ name = ("IdempotentConsumer-" + [guid]::NewGuid().ToString("N")); price = 58.70 } | ConvertTo-Json
    $accepted = Invoke-RestMethod "$BaseUrl/products" -Method Post -Headers $headers -Body $body -ContentType "application/json"
    if (-not $accepted.operationId -or -not $accepted.productId) { throw "POST did not return identifiers" }
    $operationId = [guid]$accepted.operationId
    $productId = [guid]$accepted.productId
    Write-Host "Created operation $operationId; product $productId"

    $completed = $false
    $deadline = [DateTime]::UtcNow.AddSeconds($TimeoutSeconds)
    do {
        $status = Invoke-RestMethod "$BaseUrl/operations/$operationId" -Headers $headers
        if ($status.status -eq "Failed") { throw "Original operation failed" }
        if ($status.status -eq "Completed") { $completed = $true; break }
        Start-Sleep -Seconds 1
    } while ([DateTime]::UtcNow -lt $deadline)
    if (-not $completed) { throw "Original operation did not complete" }

    $before = Get-SqlCounts -OperationId $operationId -ProductId $productId
    if ($before.Products -ne 1 -or $before.ProcessedCommands -ne 1 -or $before.CompletedStatuses -ne 1) {
        throw "Unexpected baseline SQL rows: $($before | ConvertTo-Json -Compress)"
    }
    Write-Host "SQL baseline: Products=1, ProcessedCommands=1, CompletedStatus=1"
    [void](Wait-CompletedLog -OperationId ([string]$operationId) -Since $since -Minimum 1)

    # Read the *actual durable command* from SQL Outbox, not a reconstructed JSON.
    $payloadQuery = "SET NOCOUNT ON; SELECT Payload FROM dbo.OutboxMessages WHERE OperationId = '$operationId';"
    $payload = Invoke-LabSql $payloadQuery
    if (-not $payload.StartsWith("{")) { throw "Outbox payload missing or not JSON" }
    $command = $payload | ConvertFrom-Json
    if ([guid]$command.OperationId -ne $operationId -or [guid]$command.ProductId -ne $productId) {
        throw "Outbox payload identifiers differ from HTTP response"
    }

    # Republish same message twice AFTER SQL COMMIT; manual RabbitMQ publisher.
    for ($delivery = 2; $delivery -le 3; $delivery++) {
        $message = @{
            properties = @{ delivery_mode = 2; message_id = [string]$operationId }
            routing_key = "products.create"
            payload = $payload
            payload_encoding = "string"
        } | ConvertTo-Json -Depth 5
        $publication = Invoke-RestMethod $publishUri -Method Post -Headers $rabbitHeaders -Body $message -ContentType "application/json" -TimeoutSec 15
        if (-not $publication.routed) { throw "Duplicate message was not routed to products.create" }
        [void](Wait-CompletedLog -OperationId ([string]$operationId) -Since $since -Minimum $delivery)

        $current = Get-SqlCounts -OperationId $operationId -ProductId $productId
        Write-Host "Delivery $delivery -> Products=$($current.Products), ProcessedCommands=$($current.ProcessedCommands), Completed=$($current.CompletedStatuses)"
        if ($current.Products -ne 1 -or $current.ProcessedCommands -ne 1 -or $current.CompletedStatuses -ne 1) {
            throw "Idempotent consumer invariant broken after delivery $delivery"
        }
    }

    $finalStatus = Invoke-RestMethod "$BaseUrl/operations/$operationId" -Headers $headers
    if ($finalStatus.status -ne "Completed" -or $finalStatus.productId -ne $productId) {
        throw "Operation status changed after duplicate deliveries"
    }
    Write-Host "PASS: 3 deliveries -> exactly 1 Product, 1 ProcessedCommand, 1 Completed operation" -ForegroundColor Green
}
finally {
    Pop-Location
}
