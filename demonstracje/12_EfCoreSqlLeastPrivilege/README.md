# Demo 12 — EF Core + SQL Server: Code First, Database First, minimalne uprawnienia

Dwa podejścia do schematu i **dwa konta SQL** (zasada least privilege):

| Konto | Po co | Czego nie wolno |
|---|---|---|
| `demo12_deploy` | wdrażanie: migracje EF albo skrypt SQL (DDL) | `sa` / `sysadmin` |
| `demo12_app` | aplikacja: SELECT/INSERT/UPDATE/DELETE | CREATE/ALTER/DROP tabel |

`sa` jest tylko w `docker-compose` do **bootstrapu** (loginy + puste bazy). Aplikacja i `dotnet ef` nigdy nie używają `sa`.

Testy jednostkowe **nie** wymagają Dockera.

## Wymagania na sali

- Docker Desktop / Docker Engine
- .NET 10 SDK
- (opcjonalnie) `dotnet tool install -g dotnet-ef` — gdy pokazujesz `migrations add` / `dbcontext scaffold`

## Start infrastruktury

```bash
cd demonstracje/12_EfCoreSqlLeastPrivilege
docker compose up -d
```

Kontener `init` czeka na SQL Server, tworzy loginy/bazy (jako `sa`) i wdraża schemat Database First **jako `demo12_deploy`**.

- SQL Server: `localhost,1433`
- `sa` / `Demo12_StrongPass!` — tylko bootstrap
- `demo12_deploy` / `Demo12_Deploy_Pass!`
- `demo12_app` / `Demo12_App_Pass!`

Jeśli port 1433 jest zajęty (np. demo 11), zatrzymaj tamten compose albo uruchom skrypty `sql/` na istniejącej instancji.

To hasła **wyłącznie do lokalnego docker-compose**.

## Code First (źródłem prawdy jest C#)

1. Encje + `OnModelCreating` → migracja w repozytorium.
2. Wdrożenie: konto **deploy** woła `Database.Migrate()`.
3. Runtime: konto **app** tylko CRUD.

```bash
dotnet run --project demonstracje/12_EfCoreSqlLeastPrivilege/src/CodeFirst -- deploy
dotnet run --project demonstracje/12_EfCoreSqlLeastPrivilege/src/CodeFirst -- app SKU-42 "Notes" 12.50
dotnet run --project demonstracje/12_EfCoreSqlLeastPrivilege/src/CodeFirst -- app --try-migrate
```

Nowa migracja (konto deploy, design-time factory):

```bash
dotnet ef migrations add Nazwa --project demonstracje/12_EfCoreSqlLeastPrivilege/src/CodeFirst
```

## Database First (źródłem prawdy jest SQL)

1. DBA/deploy: `sql/01_dbfirst_schema.sql` (compose robi to automatycznie).
2. Model C# odpowiada tabeli (na sali: `dotnet ef dbcontext scaffold` z `--no-onconfiguring`).
3. Aplikacja **nie** woła `Migrate()` / `EnsureCreated()`.

```bash
dotnet run --project demonstracje/12_EfCoreSqlLeastPrivilege/src/DatabaseFirst -- app SKU-7 "Długopis" 3.20
dotnet run --project demonstracje/12_EfCoreSqlLeastPrivilege/src/DatabaseFirst -- app --try-ddl
```

Scaffold (po `docker compose up`, connection string **deploy** albo tylko-odczyt; nie `sa`):

```bash
dotnet ef dbcontext scaffold \
  "Server=localhost,1433;Database=Demo12_DbFirst;User Id=demo12_deploy;Password=Demo12_Deploy_Pass!;TrustServerCertificate=True;" \
  Microsoft.EntityFrameworkCore.SqlServer \
  --project demonstracje/12_EfCoreSqlLeastPrivilege/src/DatabaseFirst \
  --context CatalogDbContext --output-dir Models --force --no-onconfiguring
```

`--no-onconfiguring` zostawia connection string w konfiguracji — aplikacja podaje konto `demo12_app`.

## Co pokazać studentom

1. **Least privilege:** dwa connection stringi w `appsettings.json`; runtime nigdy nie jest `sa` ani `db_owner`.
2. Code First: zmiana w C# → migracja → `deploy` → `app`.
3. Database First: zmiana w SQL → scaffold → `app` bez `Migrate()`.
4. `--try-migrate` / `--try-ddl`: konto aplikacji dostaje błąd na `CREATE TABLE`.
5. Opcjonalnie `sql/02_tighten_app_grants.sql`: DML tylko na `dbo.Products`, bez `__EFMigrationsHistory`.

## Testy bez Dockera

```bash
dotnet test demonstracje/12_EfCoreSqlLeastPrivilege/tests/Demo12_EfCoreSqlLeastPrivilege.Tests.csproj
```
