param(
    [string] $BaseUrl = "http://localhost:5087",
    [ValidateRange(1, 500)] [int] $Take = 100,
    [ValidateRange(1, 100)] [int] $Repetitions = 5,
    [switch] $SkipWarmup
)
$ErrorActionPreference = "Stop"
$strategies = @("nplusone", "include", "projection", "split")
if (-not $SkipWarmup) {
    Write-Host "Warm-up for each variant..."
    foreach ($s in $strategies) {
        $null = Invoke-RestMethod "$BaseUrl/api/orders/${s}?take=$Take"
    }
}
$results = foreach ($run in 1..$Repetitions) {
    foreach ($s in $strategies) {
        $response = Invoke-RestMethod "$BaseUrl/api/orders/${s}?take=$Take"
        $itemTotal = @($response.data | ForEach-Object { $_.items.Count } | Measure-Object -Sum)[0].Sum
        [pscustomobject]@{
            Run = $run
            Strategy = $s
            Orders = $response.orders
            Items = $itemTotal
            SqlCommands = $response.sqlCommands
            ElapsedMs = [math]::Round([double]$response.elapsedMs, 2)
        }
    }
}
$results | Format-Table -AutoSize
$summary = $results | Group-Object Strategy | ForEach-Object {
    $times = @($_.Group | Sort-Object ElapsedMs | Select-Object -ExpandProperty ElapsedMs)
    $middle = [int][math]::Floor($times.Count / 2)
    $median = if ($times.Count % 2) { $times[$middle] } else { ($times[$middle-1] + $times[$middle])/2 }
    [pscustomobject]@{
        Strategy = $_.Name
        MedianMs = [math]::Round($median, 2)
        Commands = ($_.Group | Select-Object -First 1).SqlCommands
        Orders = ($_.Group | Select-Object -First 1).Orders
        Items = ($_.Group | Select-Object -First 1).Items
    }
}
Write-Host "=== Median of measured server-side EF operations ==="
$summary | Sort-Object Strategy | Format-Table -AutoSize
$path = Join-Path $PSScriptRoot "benchmark-results.csv"
$results | Export-Csv -Path $path -NoTypeInformation -Encoding utf8
Write-Host "CSV: $path"
if (($summary | Select-Object -ExpandProperty Orders -Unique).Count -ne 1 -or
    ($summary | Select-Object -ExpandProperty Items -Unique).Count -ne 1) {
    throw "Results differ in cardinality. Compare the response payloads before performance conclusions."
}
