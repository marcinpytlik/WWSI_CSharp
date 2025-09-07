# Semestr 5 – Ćwiczenia z punktacją i podziałem tygodniowym

> Podział na **10 tygodni** (po ~4h). Każdy tydzień ma 3 zadania: **Łatwe (2 pkt)**, **Średnie (4 pkt)**, **Trudne (6 pkt)**. Razem: **12 pkt/tydzień**, **120 pkt/semestr**.
## Tydzień 1
- **Łatwe (2 pkt):** Minimal API: endpoint `/health`.
- **Średnie (4 pkt):** Endpoint `/hello/{name}`.
- **Trudne (6 pkt):** Model `User`: Id, Username, Email.

**Kryteria oceny:** poprawność, styl/konwencje, testy (jeśli dotyczy), obsługa błędów.

---
## Tydzień 2
- **Łatwe (2 pkt):** EF Core InMemory: konfiguracja DbContext.
- **Średnie (4 pkt):** POST /users: dodaj użytkownika.
- **Trudne (6 pkt):** GET /users: lista użytkowników.

**Kryteria oceny:** poprawność, styl/konwencje, testy (jeśli dotyczy), obsługa błędów.

---
## Tydzień 3
- **Łatwe (2 pkt):** GET /users/{id}: szczegóły.
- **Średnie (4 pkt):** PUT /users/{id}: aktualizacja.
- **Trudne (6 pkt):** DELETE /users/{id}: usuwanie.

**Kryteria oceny:** poprawność, styl/konwencje, testy (jeśli dotyczy), obsługa błędów.

---
## Tydzień 4
- **Łatwe (2 pkt):** EF Core: pierwsza migracja.
- **Średnie (4 pkt):** Relacja 1‑w‑wiele `User`→`Task`.
- **Trudne (6 pkt):** POST /tasks: dodaj zadanie do usera.

**Kryteria oceny:** poprawność, styl/konwencje, testy (jeśli dotyczy), obsługa błędów.

---
## Tydzień 5
- **Łatwe (2 pkt):** GET /users/{id}/tasks: lista zadań.
- **Średnie (4 pkt):** Rejestracja: hashowanie hasła.
- **Trudne (6 pkt):** Logowanie: wydaj JWT.

**Kryteria oceny:** poprawność, styl/konwencje, testy (jeśli dotyczy), obsługa błędów.

---
## Tydzień 6
- **Łatwe (2 pkt):** Middleware: autoryzacja JWT.
- **Średnie (4 pkt):** Autoryzacja: chroń `/tasks`.
- **Trudne (6 pkt):** Logowanie błędów do konsoli.

**Kryteria oceny:** poprawność, styl/konwencje, testy (jeśli dotyczy), obsługa błędów.

---
## Tydzień 7
- **Łatwe (2 pkt):** Serilog: zapis do pliku.
- **Średnie (4 pkt):** Walidacja e‑maila (DataAnnotations).
- **Trudne (6 pkt):** DTO `UserDto` (bez hasła).

**Kryteria oceny:** poprawność, styl/konwencje, testy (jeśli dotyczy), obsługa błędów.

---
## Tydzień 8
- **Łatwe (2 pkt):** AutoMapper: mapowanie `User→UserDto`.
- **Średnie (4 pkt):** Raport `/reports/new-users?from&to`.
- **Trudne (6 pkt):** Test integracyjny `/health`.

**Kryteria oceny:** poprawność, styl/konwencje, testy (jeśli dotyczy), obsługa błędów.

---
## Tydzień 9
- **Łatwe (2 pkt):** Test integracyjny rejestracji.
- **Średnie (4 pkt):** Test integracyjny logowania + zasób chroniony.
- **Trudne (6 pkt):** Seeding użytkowników w DbContext.

**Kryteria oceny:** poprawność, styl/konwencje, testy (jeśli dotyczy), obsługa błędów.

---
## Tydzień 10
- **Łatwe (2 pkt):** Swagger (Swashbuckle) – dokumentacja.
- **Średnie (4 pkt):** Versioning API – konfiguracja.
- **Trudne (6 pkt):** Testy wersjonowania API.

**Kryteria oceny:** poprawność, styl/konwencje, testy (jeśli dotyczy), obsługa błędów.

---

### Skala ocen (sugerowana)
- 0–59 pkt – niedostateczny
- 60–74 pkt – dostateczny
- 75–89 pkt – dobry
- 90–104 pkt – bardzo dobry
- 105–120 pkt – celujący
