# Semestr 3 — Tydzień 5: rozwiązania przykładowe

Projekt konsolowy **.NET 10**, zgodny z [zadaniami](../README.md) (100 pkt). Menu 1–5 pozwala uruchamiać ćwiczenia niezależnie.

## Uruchomienie

~~~powershell
cd semestr3/exercises/week05/solution
dotnet restore Week05.Solutions.slnx
dotnet build Week05.Solutions.slnx
dotnet run --project Week05.Solutions
~~~

## Ćwiczenia

| Opcja | Punkty | Przykładowa realizacja |
|---|---:|---|
| 1 | 10 | Person: konstruktor, właściwości, GetAge, dwie instancje |
| 2 | 15 | Product: hermetyzacja, decimal, sprawdzanie rabatu 0–100% |
| 3 | 20 | BankAccount: prywatne saldo, Deposit/Withdraw, ochrona przed przekroczeniem salda |
| 4 | 25 | Book i Library: lista, dodawanie, wyszukiwanie fragmentu tytułu, usuwanie |
| 5 | 30 | Order, OrderItem, Product: pozycje, suma i raport ToString |

## Sprawdzenie

- Opcja 2: rabat 15% od 4200 daje 3570,00; rabat 120% jest odrzucany.
- Opcja 3: 500 - 125 = 375; wypłata 1000 jest odrzucana, saldo pozostaje 375.
- Opcja 4: wyszukiwanie ignoruje wielkość liter; po usunięciu zostaje jedna książka.
- Opcja 5: 2 × 120 + 75 = 315,00.

**Uwagi dydaktyczne:** GetAge zna tylko rok urodzenia, więc zwraca wiek przybliżony. Ceny i salda to decimal. To wzorzec do nauki OOP, nie prawdziwy system bankowy ani księgowy. Konstruktor i metody walidują dane; stan kolekcji jest chroniony przez IReadOnlyList.
