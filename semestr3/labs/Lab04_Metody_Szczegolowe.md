# Lab04 Metody — szczegółowe laboratorium

**Czas:** 90 min · **Stack:** .NET 10, xUnit

## Cel

Świadomie wybrać `ref` / `out` / wartość zwracaną. W nowym kodzie `out` jest rzadziej potrzebny niż krotka.

## Zadania

1. `Swap(ref int a, ref int b)` — test, że wartości się zamieniły.
2. `TryDivide(int a, int b, out int result)` → `false` przy `b == 0` (nie rzucaj).
3. Przeciąż `Max`: `Max(int, int)`, `Max(int, int, int)`, `Max(params int[])`. Pusta `params` → wyjątek.
4. Zamień `TryDivide` na wersję zwracającą `(bool ok, int result)` i porównaj czytelność w README (5 zdań).

## Kryteria

- [ ] `ref` tylko tam, gdzie mutacja argumentu jest celem
- [ ] dzielenie przez zero nie rzuca w `TryDivide`
- [ ] przeciążenia pokryte testami, w tym 1 element i wyjątek na pustą tablicę
