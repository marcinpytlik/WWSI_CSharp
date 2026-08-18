# Tydzień 9 – Wyjątki i obsługa błędów

**Maksymalna liczba punktów: 100 pkt**

Ćwiczenia wykonuj w .NET 10. Tam, gdzie ma to sens, rozdziel logikę od wejścia/wyjścia i dbaj o czytelne nazwy.

## Ćwiczenie 1. Try/catch w praktyce — 10 pkt

Napisz program dzielący liczby i obsłuż FormatException oraz DivideByZeroException osobno.

**Kryteria zaliczenia:**

- 2 konkretne catch.

- komunikaty.

- brak catch(Exception) jako jedynego.

## Ćwiczenie 2. Własny wyjątek wieku — 15 pkt

Utwórz InvalidAgeException i użyj go podczas tworzenia osoby z niepoprawnym wiekiem.

**Kryteria zaliczenia:**

- Własna klasa wyjątku.

- throw.

- obsługa.

## Ćwiczenie 3. Walidacja pliku — 20 pkt

Odczytaj wskazany plik i obsłuż brak pliku, brak dostępu oraz inne błędy IO.

**Kryteria zaliczenia:**

- FileNotFoundException.

- UnauthorizedAccessException.

- komunikaty.

## Ćwiczenie 4. Transakcja logiczna — 25 pkt

Zasymuluj przelew między dwoma kontami; w razie błędu stan obu kont ma pozostać spójny.

**Kryteria zaliczenia:**

- Walidacja.

- wyjątek domenowy.

- spójność stanu.

- finally lub kontrola przepływu.

## Ćwiczenie 5. Centralna obsługa błędów CLI — 30 pkt

Zbuduj małe menu aplikacji z jedną warstwą obsługi nieoczekiwanych wyjątków i osobną obsługą błędów domenowych.

**Kryteria zaliczenia:**

- Rozróżnienie błędów.

- brak połykanych wyjątków.

- sensowne komunikaty.

- logika retry/exit.

---

**Suma: 100 pkt**
