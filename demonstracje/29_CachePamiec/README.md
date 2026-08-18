# Demo 29 — cache w pamięci (Redis opcjonalnie)

`CachedCatalog` owija źródło danych w `IMemoryCache`. Testy nie potrzebują Dockera.

Na sali: ta sama `IProductSource` może iść do Redis (`IDistributedCache`) — interfejs się nie zmienia.
