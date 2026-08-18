# Lab07 JWT — szczegółowe laboratorium

**Czas:** 120–150 min · ASP.NET Core 10 Authentication JwtBearer

## Cel

Zabezpieczyć endpointy tokenem. Sekret **nie** ląduje w git.

## Zadania

1. `POST /api/v1/auth/register` i `/login` → `{ accessToken, expiresUtc }`.
2. Hasło: `PasswordHasher<T>` albo analog — **nie** SHA256 „na piechotę” bez soli (framework hasher ma sól).
3. `AddAuthentication().AddJwtBearer(...)` z kluczem z konfiguracji `Jwt:Key` (min. 32 bajty).
4. `GET /api/v1/me` z `.RequireAuthorization()` zwraca email z claimów.
5. User-secrets: `dotnet user-secrets set "Jwt:Key" "..."` .

## Testy

- bez tokena `/me` → 401
- z tokenem → 200

## Kryteria

- [ ] brak hardcode sekretu w `appsettings.json` commitowanym do main (Development może mieć placeholder, produkcja nie)
- [ ] HTTPS opisany w README (na labie HTTP localhost OK)
