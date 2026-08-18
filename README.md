# 📦 Program C# – semestr 3–5 
# 📦 Repozytorium – C# (Semestr 3–5)

Repozytorium z materiałami dydaktycznymi C# podzielone na trzy semestry (3, 4, 5).  
Każdy semestr zawiera: projekty, ćwiczenia, zadania, laboratoria oraz dokumenty programowe.

---

## 📘 Semestr 3 – Podstawy C#
- [README.md](semestr3/README.md)
- [Syllabus](semestr3/Semestr3_Syllabus.md)
- [Plan tygodniowy](semestr3/Semestr3_PlanTygodniowy.md)

## 📘 Semestr 4 – LINQ, IO, Async, Architektura
- [README.md](semestr4/README.md)
- [Syllabus](semestr4/Semestr4_Syllabus.md)
- [Plan tygodniowy](semestr4/Semestr4_PlanTygodniowy.md)

## 📘 Semestr 5 – ASP.NET + EF Core
- [README.md](semestr5/README.md)
- [Syllabus](semestr5/Semestr5_Syllabus.md)
- [Plan tygodniowy](semestr5/Semestr5_PlanTygodniowy.md)

---

## 📑 Zawartość repozytorium
- `src/` – projekty C# dla każdego semestru  
- `tasks/` – zadania i prace domowe  
- `labs/` – instrukcje krok po kroku  
- `exercises/` – zestawy ćwiczeń z punktacją  
- `cheat-sheets/` – ściągi tematyczne  
- `tests/` – projekty testów xUnit  

---

### ⚙️ Wymagania
- .NET 10 SDK
- C# 14 (domyślna wersja języka dla `net10.0`)
- [Narzędzia do pobrania](https://drive.google.com/drive/folders/1p_Eo9aKg1kuXuYTIpseYUgEiwTDkyj6S?usp=sharing)  
- Visual Studio Code / Visual Studio / Rider

## Uruchomienie przykładowego projektu
```bash
cd semestr3/labs/Lab01_Wprowadzenie/src
dotnet run
```

---

## 🧩 Solution .NET 10

Repo zawiera nową solution `WWSI_CSharp_NET10.sln`, która obejmuje wszystkie istniejące projekty `.csproj` w repozytorium.

```powershell
dotnet restore .\WWSI_CSharp_NET10.sln
dotnet build .\WWSI_CSharp_NET10.sln
dotnet test .\WWSI_CSharp_NET10.sln --no-build
```

> `WWSI_CSharp.sln` pozostaje jako plik legacy. Zawiera wpisy do projektów, których nie ma już w bieżącej strukturze repozytorium.
