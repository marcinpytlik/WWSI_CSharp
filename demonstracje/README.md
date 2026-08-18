# Demonstracje — 12 mini-projektów

Gotowe, małe aplikacje do pokazania na zajęciach (5–15 min każde).
Każda ma `src` + testy xUnit. Nie zastępują laboratoriów — to **żywe przykłady**
ścieżki semestr 3 → 5.

```bash
dotnet test WWSI_CSharp_NET10.sln --filter "FullyQualifiedName~Demo"
# albo jeden projekt:
dotnet run --project demonstracje/01_KalkulatorCli/src -- add 2 3
```

| # | Projekt | Semestr | Pokazuje |
|---|---|---|---|
| 01 | [Kalkulator CLI](01_KalkulatorCli/) | 3 | metody, wyjątki, kody wyjścia, testy |
| 02 | [Konto bankowe](02_KontoBankowe/) | 3 | hermetyzacja, niezmienniki |
| 03 | [LINQ zamówienia](03_LinqZamowienia/) | 4 | Where/GroupBy/Sum |
| 04 | [CSV → JSON](04_CsvDoJson/) | 4 | IO, `System.Text.Json` |
| 05 | [HTTP cytaty](05_HttpCytaty/) | 4 | `HttpClient`, async, fake handler |
| 06 | [ToDo warstwy](06_TodoWarstwy/) | 4 | serwis + repo JSON |
| 07 | [Notes API](07_NotesApi/) | 5 | Minimal API, CRUD in-memory |
| 08 | [Biblioteka EF](08_BibliotekaEf/) | 5 | EF Core + SQLite |
| 09 | [JWT mini](09_JwtMini/) | 5 | login + chroniony `/me` |
| 10 | [Wycena (strategia)](10_WycenaStrategia/) | 4–5 | Strategy + Factory |
| 11 | [Kolejka Rabbit + SQL Server](11_KolejkaRabbitSql/) | 5 | RabbitMQ, worker, EF Core SQL Server |
| 12 | [EF Core SQL: Code/DB First](12_EfCoreSqlLeastPrivilege/) | 5 | SQL Server, migracje vs scaffold, dwa konta (least privilege) |

Kolejność na zajęciach: **01–02** (tydzień 4–5 sem. 3), **03–06** (sem. 4), **07–09** (sem. 5), **10** jako most do GoF/GRASP, **11–12** na osobne zajęcia z Dockerem (kolejka / EF Core + uprawnienia SQL).
