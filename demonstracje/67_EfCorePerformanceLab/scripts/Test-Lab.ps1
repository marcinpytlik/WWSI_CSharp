param([string] $BaseUrl = "http://localhost:5087", [ValidateRange(1,100)] [int] $Take = 100)
$ErrorActionPreference = "Stop"
$cases = @(
    @{ Name = "nplusone"; ExpectedCommands = $Take + 1 },
    @{ Name = "include"; ExpectedCommands = 1 },
    @{ Name = "projection"; ExpectedCommands = 1 },
    @{ Name = "split"; ExpectedCommands = 2 }
)
$all = @{}
foreach ($case in $cases) {
    $name = $case.Name
    $response = Invoke-RestMethod "$BaseUrl/api/orders/${name}?take=$Take"
    if ($response.orders -ne $Take) { throw "$name : expected $Take orders, got $($response.orders)" }
    if ($response.sqlCommands -ne $case.ExpectedCommands) {
        throw "$name : expected $($case.ExpectedCommands) SQL commands, got $($response.sqlCommands)"
    }
    $all[$name] = $response
    Write-Host "PASS $name : $($response.orders) orders, $($response.sqlCommands) SQL commands"
}
$baseline = $all["nplusone"].data | ConvertTo-Json -Depth 20 -Compress
foreach ($name in "include","projection","split") {
    $candidate = $all[$name].data | ConvertTo-Json -Depth 20 -Compress
    if ($candidate -cne $baseline) { throw "$name returned different orders/items." }
    Write-Host "PASS $name : payload matches nplusone"
}
Write-Host "PASS all assertions (seed: 100 orders × 10 items; take=$Take)."
