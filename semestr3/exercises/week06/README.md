# Tydzień 6 – Dziedziczenie, polimorfizm i abstrakcje

**Maksymalna liczba punktów: 100 pkt**

Ćwiczenia wykonuj w .NET 10. Tam, gdzie ma to sens, rozdziel logikę od wejścia/wyjścia i dbaj o czytelne nazwy.

## Ćwiczenie 1. Hierarchia pojazdów — 10 pkt

Utwórz bazową klasę Vehicle i klasy Car oraz Bicycle z własnymi właściwościami.

**Kryteria zaliczenia:**

- Dziedziczenie.

- min. 2 klasy pochodne.

- konstruktor bazowy.

## Ćwiczenie 2. Wirtualny opis — 15 pkt

Dodaj virtual Describe() w Vehicle i override w klasach pochodnych.

**Kryteria zaliczenia:**

- virtual/override.

- różne wyniki.

- demonstracja polimorfizmu.

## Ćwiczenie 3. Abstrakcyjne figury — 20 pkt

Utwórz abstract Shape z CalculateArea() oraz Circle, Rectangle i Triangle.

**Kryteria zaliczenia:**

- abstract.

- 3 implementacje.

- poprawne wzory.

## Ćwiczenie 4. Polimorficzna lista — 25 pkt

Umieść różne Shape w List<Shape>, policz pola i sumę wszystkich pól bez sprawdzania konkretnego typu.

**Kryteria zaliczenia:**

- Polimorfizm.

- kolekcja bazowa.

- brak if po typach.

## Ćwiczenie 5. System płatności — 30 pkt

Zaprojektuj abstrakcyjne Payment z Process(decimal amount) i implementacje CardPayment, TransferPayment, CashPayment. Dodaj historię wyników operacji.

**Kryteria zaliczenia:**

- Abstrakcja.

- 3 strategie.

- wspólne API.

- raport operacji.

---

**Suma: 100 pkt**
