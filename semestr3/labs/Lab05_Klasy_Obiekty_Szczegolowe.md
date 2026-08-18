# Lab05 Klasy i obiekty — szczegółowe laboratorium

**Czas:** 120 min · **Stack:** .NET 10, xUnit

## Cel

Zbudować hermetyczny model `BankAccount` bez publicznych pól.

## Zadania

1. Klasa `BankAccount`: `Id` (guid), `Owner` (niepusty), `Balance` (`private set`, >= 0).
2. `Deposit(decimal)` / `Withdraw(decimal)` — kwota > 0; withdraw nie może zejść poniżej 0 (wyjątek `InvalidOperationException`).
3. Rekord `Money(decimal Amount, string Currency)` z walidacją w konstruktorze (`Amount >= 0`, waluta 3 znaki).
4. Nie wystawiaj `public decimal Balance;` jako pola.

## Testy

- wpłata zwiększa saldo
- wypłata większa niż saldo rzuca
- dwa konta to dwa obiekty (`Assert.NotSame`)

## Kryteria

- [ ] brak publicznych pól
- [ ] saldo nieujemne niezmiennikiem
- [ ] 6+ testów
