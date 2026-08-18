# Tydzień 8 – Architektura warstwowa

**Maksymalna liczba punktów: 100 pkt**

Ćwiczenia wykonuj w .NET 10. Tam, gdzie ma to sens, rozdziel logikę od wejścia/wyjścia i dbaj o czytelne nazwy.

## Ćwiczenie 1. Podział na warstwy — 10 pkt

Rozdziel prostą aplikację na Core, Infrastructure i UI.

**Kryteria zaliczenia:**

- 3 projekty/warstwy.

- poprawne zależności.

- brak zależności Core→Infra.

## Ćwiczenie 2. Interfejs repozytorium — 15 pkt

Umieść IRepository<T> w Core, implementację InMemoryRepository<T> w Infrastructure.

**Kryteria zaliczenia:**

- Dependency inversion.

- interfejs w Core.

- działająca implementacja.

## Ćwiczenie 3. Serwis aplikacyjny — 20 pkt

Dodaj Service używający repozytorium i zawierający logikę biznesową.

**Kryteria zaliczenia:**

- Logika poza UI.

- konstruktor DI.

- testowalne API.

## Ćwiczenie 4. Wymiana infrastruktury — 25 pkt

Dodaj drugą implementację repozytorium plikowego i przełączaj implementację bez zmiany Core.

**Kryteria zaliczenia:**

- 2 implementacje.

- brak zmian domeny.

- konfiguracja w composition root.

## Ćwiczenie 5. Mini Clean Architecture — 30 pkt

Zaprojektuj małą aplikację ToDo z Domain/Core, Application, Infrastructure i UI wraz z diagramem zależności w README.

**Kryteria zaliczenia:**

- 4 warstwy.

- zgodne kierunki zależności.

- DI.

- dokumentacja architektury.

---

**Suma: 100 pkt**
