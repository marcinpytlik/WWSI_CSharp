# Zadanie 02 – Pipeline CSV → JSON

**Termin:** tydzień 5

## Treść

Wczytaj CSV (header), zmapuj do rekordów, zapisz JSON (`System.Text.Json`).
Pusty plik i zły wiersz obsłużone.

## Kryteria

- [ ] `StreamReader` (nie `ReadAllText` na „dużym” pliku — choć sample może być mały)
- [ ] round-trip test: JSON → obiekty zgodne z CSV
