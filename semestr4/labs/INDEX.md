# INDEX — mapowanie Lab ↔ Task (repo)

Ten plik spina materiały w `labs/` z projektami w `src/` oraz testami w `tests/`.

## Lab01 — LINQ Podstawy
**Tasky:**
- `src/Task01_EvenNumbers` — Where: wybór parzystych
- `src/Task02_Average` — Average: średnia
- `src/Task05_SortStringsDesc` — OrderByDescending: sort malejąco

## Lab02 — LINQ Zaawansowane
**Tasky:**
- `src/Task03_GroupByAge` — GroupBy: grupowanie osób po wieku
- `src/Task04_JoinCollections` — Join: połączenie kolekcji
- `src/Task23_OrderServiceTotal` — Sum/Agregacje na domenie
- `src/Task29_TodoFilterByStatus` — LINQ filter po statusie

## Lab03 — Query Syntax
Ten lab to ćwiczenie „przepisz styl” (metody ↔ query syntax) na tych samych danych.  
**Baza ćwiczeń (te same projekty):**
- `src/Task03_GroupByAge`
- `src/Task04_JoinCollections`
- `src/Task05_SortStringsDesc`

## Lab04 — IO Plik
**Tasky:**
- `src/Task06_ReadCsv` — wczytanie CSV
- `src/Task18_CopyFileStreamReaderWriter` — kopiowanie pliku StreamReader/Writer
- `src/Task19_SumNumbersLargeFile` — suma liczb z dużego pliku (File.ReadLines)
- `src/Task25_FileLogger` — logowanie do pliku (append)

## Lab05 — JSON / XML
**Tasky:**
- `src/Task08_ReadJsonObject` — JSON → obiekt
- `src/Task09_WriteUserJson` — obiekt → JSON + zapis do pliku
- `src/Task24_ProductPriceValidation` — walidacja domenowa (cena nieujemna)
- `src/Task26_ReadXmlTitles` — XML: wyciąganie `<title>`
- `src/Task30_JsonRoundtripTestTarget` — roundtrip JSON jako „kontrakt”

## Lab06 — Async / Await
**Tasky:**
- `src/Task12_AsyncExceptionHandling` — obsługa wyjątków async (Result)
- `src/Task13_TaskDelayLogging` — Task.Delay + log czasu
- `src/Task11_Fetch5Parallel` — Task.WhenAll: równoległość (koncept async)

## Lab07 — HttpClient
**Tasky:**
- `src/Task10_FetchApiAsync` — GET + EnsureSuccessStatusCode
- `src/Task11_Fetch5Parallel` — 5 requestów równolegle
- `src/Task20_FetchJsonListFromApi` — GET JSON → lista obiektów

## Lab08 — Architektura
**Tasky:**
- `src/Task15_IRepositoryGeneric` — kontrakt (interfejs) + generyki
- `src/Task14_InMemoryProductRepo` — implementacja repo (in-memory)
- `src/Task23_OrderServiceTotal` — serwis domenowy (logika)
- `src/Task22_MoqRepositoryTarget` — zależność od interfejsu → testowalność

## Lab09 — Wzorce
**Tasky:**
- `src/Task14_InMemoryProductRepo` — Repository (konkret)
- `src/Task15_IRepositoryGeneric` — Repository (abstrakcja)
- `src/Task16_SingletonPattern` — Singleton
- `src/Task17_ShapeFactory` — Factory

## Lab10 — Testowanie
**Tasky „flagowe” pod lab:**
- `src/Task21_IsPrimeTestsTarget` + `tests/Task21_IsPrimeTestsTarget.Tests`
- `src/Task22_MoqRepositoryTarget` + `tests/Task22_MoqRepositoryTarget.Tests`
- `src/Task30_JsonRoundtripTestTarget` + `tests/Task30_JsonRoundtripTestTarget.Tests`

(W repo testy są przy KAŻDYM tasku — powyższe są najczytelniejsze do omówienia.)

## Lab11 — Projekt ToDo
**Tasky:**
- `src/Task28_TodoConsoleJson` — ToDo + persystencja JSON
- `src/Task29_TodoFilterByStatus` — filtrowanie LINQ po statusie
- `src/Task30_JsonRoundtripTestTarget` — test roundtrip serializacji

## Lab12 — Powtórka
Cel: refaktoryzacja + domknięcie przypadków brzegowych + wzmocnienie testów.  
**Checklisty na powtórkę:**
- dopisz testy na `null/empty/invalid input` tam gdzie ma sens
- rozdziel IO od logiki (czyste metody łatwiej testować)
- dołóż walidacje argumentów (`ArgumentException`, `ArgumentOutOfRangeException`)
- popraw czytelność: nazwy, małe metody, brak duplikacji

---

## Szybkie komendy
```powershell
dotnet restore
dotnet build
dotnet test
```
