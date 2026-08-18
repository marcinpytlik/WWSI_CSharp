# Lab03 Walidacja — szczegółowe laboratorium

**Czas:** 120 min · ASP.NET Core 10 · nadal in-memory

## Cel

Odrzucać złe DTO kodem 400 i ciałem ProblemDetails / własnym `{ error, fields }`.

## Zadania

1. `CreateNoteDto(string Title, string? Body)` — `[Required]`, `[MinLength(3)]`, `[MaxLength(120)]` na Title.
2. Filtr walidacji albo `TypedResults.ValidationProblem`.
3. `POST /api/v1/notes` → 201 + `Location` przy sukcesie.
4. Nie waliduj tylko w handlerze dwudziestoma `if`.

## Testy

- pusty title → 400
- title 3+ znaki → 201

## Kryteria

- [ ] 400 nie zwraca 500
- [ ] DTO ≠ encja dziedzinowa (nawet jeśli na razie to prawie to samo)
