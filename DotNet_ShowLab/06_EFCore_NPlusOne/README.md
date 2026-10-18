# EF Core N+1

Wykorzystaj istniejący projekt EF, omów lazy loading/explicit queries, policz polecenia SQL przy odczycie relacji.

## Uruchomienie (PowerShell z korzenia repo)
```powershell
# istniejące materiały: demonstracje/08_BibliotekaEf, demonstracje/21_RelacjeEf, demonstracje/58_AsNoTracking
Get-ChildItem demonstracje/21_RelacjeEf -Recurse -Filter *.csproj
```

## Przebieg
1. Zapytaj studentów, czego się spodziewają.
2. Uruchom pokaz i porównaj pomiary lub odpowiedzi.
3. Omów wynik oraz ograniczenia eksperymentu.

## Interpretacja
Wykorzystaj istniejący projekt EF, omów lazy loading/explicit queries, policz polecenia SQL przy odczycie relacji.

**Uwaga:** ta demonstracja jest instrukcją do już istniejącego projektu z katalogu `demonstracje/`, nie nową implementacją. Uruchomienie może wymagać dodatkowej konfiguracji.
