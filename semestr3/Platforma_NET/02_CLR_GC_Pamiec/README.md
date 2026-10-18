# 02 — CLR, alokacje, GC i Dispose

Z korzenia repo:
```powershell
$proj = 'semestr3/Platforma_NET/02_CLR_GC_Pamiec/src/GcDemo.csproj'
dotnet run --project $proj -c Release
```

Program pokazuje licznik kolekcji gen. 0, pamięć zarządzaną przed/po alokacjach i automatyczne wywołanie Dispose na zasobie zarządzającym cyklem życia. Dokładne wartości zależą od środowiska; brak wzrostu licznika podczas jednego uruchomienia nie oznacza, że GC nie działa.

**Opcjonalna obserwacja w czasie rzeczywistym:**
```powershell
dotnet tool install --global dotnet-counters
dotnet-counters ps
dotnet-counters monitor -p <PID> System.Runtime
```
Uruchom drugi terminal, podczas pracy programu ustal PID i obserwuj dostępne metryki runtime. Nazwy i zestaw metryk zależą od wersji narzędzia. Alternatywnie wykorzystaj `demonstracje/15_UsingDispose`.

**Pytania:** Czy `using` wywołuje GC? Czy GC natychmiast zwalnia każdą nieużywaną tablicę? Jakie ograniczenia ma ta demonstracja?
