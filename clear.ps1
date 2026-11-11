
[CmdletBinding()]
param(
    [string]$RepositoryPath = "C:\Users\blad\Documents\GitHub\WWSI_CSharp",

    [switch]$Execute
)

$ErrorActionPreference = "Stop"

# Sprawdzenie katalogu repozytorium
$RepositoryPath = (Resolve-Path -LiteralPath $RepositoryPath).Path

if (-not (Test-Path -LiteralPath (
    Join-Path $RepositoryPath ".git"
))) {
    throw "Podana ścieżka nie jest katalogiem głównym repozytorium Git."
}

Write-Host "`nRepozytorium: $RepositoryPath" -ForegroundColor Cyan

# Znajdź wszystkie katalogi bin i obj
# Nie przeszukuj wewnętrznej struktury .git
$directories = @(
    Get-ChildItem -LiteralPath $RepositoryPath `
        -Directory -Recurse -Force |
        Where-Object {
            $_.Name -in @("bin", "obj") -and
            -not $_.FullName.StartsWith(
                (Join-Path $RepositoryPath ".git") +
                [IO.Path]::DirectorySeparatorChar,
                [StringComparison]::OrdinalIgnoreCase
            )
        } |
        Sort-Object FullName -Unique
)

if ($directories.Count -eq 0) {
    Write-Host "Nie znaleziono katalogów bin ani obj." `
        -ForegroundColor Green
    return
}

Write-Host "`nZnalezione katalogi: $($directories.Count)" `
    -ForegroundColor Yellow

$directories | ForEach-Object {
    Write-Host $_.FullName
}

if (-not $Execute) {
    Write-Host "`nTRYB PODGLĄDU — nic nie usunięto." `
        -ForegroundColor Cyan
    Write-Host "Aby usunąć katalogi, uruchom skrypt z -Execute."
    return
}

# Usuwanie znalezionych katalogów
$removed = 0
$failed = 0

foreach ($directory in $directories) {
    try {
        Remove-Item -LiteralPath $directory.FullName `
            -Recurse -Force -ErrorAction Stop

        $removed++
        Write-Host "[OK] $($directory.FullName)" `
            -ForegroundColor Green
    }
    catch {
        $failed++
        Write-Warning "Nie udało się usunąć: $($directory.FullName) - $_"
    }
}

Write-Host "`nPODSUMOWANIE" -ForegroundColor Cyan
Write-Host "Usunięte katalogi: $removed"
Write-Host "Błędy: $failed"
