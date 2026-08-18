# Demo 49 — Directory.Build.props + global.json

Najbliższy `Directory.Build.props` (w tym folderze) **nadpisuje auto-import** z korzenia,
więc importujemy rodzica i doklejamy `DEMO49` + `AssemblyTitle`.

`global.json` w korzeniu wymusza SDK 10.x (`rollForward: latestFeature`).

## Co pokazać studentom

1. Bez importu rodzica zniknęłyby `net10.0` i CPM.
2. Stała `DEMO49` wchodzi do wszystkich projektów w tym folderze.
3. `global.json` — ten sam SDK na sali i na CI.
