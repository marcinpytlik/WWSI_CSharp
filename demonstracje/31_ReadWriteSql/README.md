# Demo 31 — dwa connection stringi: zapis vs odczyt

`CatalogWriter` używa konta `demo31_write`, `CatalogReader` — `demo31_read` (tylko SELECT).
Testy idą na SQLite (CI bez Dockera). Na sali: SQL Server jak demo 12.

```bash
cd demonstracje/31_ReadWriteSql
docker compose up -d
```
