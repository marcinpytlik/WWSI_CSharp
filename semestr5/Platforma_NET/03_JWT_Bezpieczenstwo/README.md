# 03 — JWT, autentykacja i autoryzacja

Demonstracje w repo:
- `demonstracje/09_JwtMini` — logowanie i chroniony endpoint.
- `demonstracje/33_JwtRole403` — różnica 401/403, rola użytkownika i administratora.

**Na sali:** wyślij żądanie bez tokena (oczekiwane 401 dla endpointu chronionego), z prawidłowym tokenem bez wymaganej roli (403), z wymaganym uprawnieniem (200). Zawsze sprawdź endpointy i polecenia w README konkretnego projektu.

**Ważne:** podpis, issuer/audience, czas ważności i transport HTTPS są elementami bezpieczeństwa; Base64URL koduje payload, ale go nie szyfruje. Do pokazu wyłącznie sztuczne dane i testowe sekrety.

**Pytania:** kto wydaje token? kto sprawdza podpis? dlaczego 403 nie oznacza, że użytkownik jest niezalogowany?
