# Tydzień 7 – JWT i autoryzacja

**Maksymalna liczba punktów: 100 pkt**

Ćwiczenia wykonuj w .NET 10. Tam, gdzie ma to sens, rozdziel logikę od wejścia/wyjścia i dbaj o czytelne nazwy.

## Ćwiczenie 1. Rejestracja użytkownika — 10 pkt

Dodaj endpoint rejestracji z bezpiecznym hashowaniem hasła; nigdy nie zapisuj hasła jawnie.

**Kryteria zaliczenia:**

- Hash.

- walidacja.

- brak hasła w response/logach.

## Ćwiczenie 2. Logowanie — 15 pkt

Dodaj endpoint login zwracający token JWT po poprawnym uwierzytelnieniu.

**Kryteria zaliczenia:**

- Weryfikacja hash.

- JWT.

- claims.

- błąd 401.

## Ćwiczenie 3. Authorize — 20 pkt

Zabezpiecz wybrane endpointy i pozostaw publiczny /health.

**Kryteria zaliczenia:**

- RequireAuthorization/[Authorize].

- 401.

- public health.

## Ćwiczenie 4. Role/claim — 25 pkt

Dodaj uprawnienia oparte na roli lub claim, np. tylko Admin może usuwać użytkowników.

**Kryteria zaliczenia:**

- Claim/role.

- policy.

- 403 dla braku uprawnień.

## Ćwiczenie 5. Bezpieczna konfiguracja JWT — 30 pkt

Przenieś klucz JWT poza repo, skonfiguruj issuer/audience/expiry i opisz rotację sekretu oraz różnicę 401 vs 403.

**Kryteria zaliczenia:**

- Secret poza Git.

- pełna walidacja tokena.

- sensowny expiry.

- dokumentacja bezpieczeństwa.

---

**Suma: 100 pkt**
