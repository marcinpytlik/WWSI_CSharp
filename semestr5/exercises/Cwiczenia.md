# 📘 Semestr 5 – ASP.NET + EF Core + JWT (30 ćwiczeń)

1. Utwórz Minimal API z endpointem `/health`.  
2. Dodaj endpoint `/hello/{name}` zwracający powitanie.  
3. Zrób model `User` z polami `Id, Username, Email`.  
4. Skonfiguruj DbContext z EF Core InMemory.  
5. Dodaj endpoint `POST /users` zapisujący użytkownika w bazie.  
6. Dodaj endpoint `GET /users` zwracający listę.  
7. Dodaj endpoint `GET /users/{id}`.  
8. Dodaj endpoint `PUT /users/{id}` aktualizujący dane.  
9. Dodaj endpoint `DELETE /users/{id}`.  
10. Utwórz migrację EF Core i uruchom.  
11. Dodaj relację 1‑w‑wiele `User` → `Task`.  
12. Dodaj endpoint `POST /tasks` przypisujący zadanie do użytkownika.  
13. Dodaj endpoint `GET /users/{id}/tasks`.  
14. Zaimplementuj rejestrację użytkownika z hasłem (hash).  
15. Zaimplementuj logowanie i zwracanie JWT.  
16. Dodaj middleware autoryzacji JWT.  
17. Zabezpiecz endpoint `/tasks` – tylko zalogowany użytkownik.  
18. Dodaj logowanie błędów do konsoli.  
19. Skonfiguruj Serilog do pliku.  
20. Dodaj walidację e‑maila (DataAnnotations).  
21. Dodaj DTO do `User` (bez hasła).  
22. Użyj AutoMapper do mapowania `User → UserDto`.  
23. Dodaj endpoint raportowy `/reports/new-users?from&to`.  
24. Dodaj test integracyjny `/health`.  
25. Dodaj test integracyjny rejestracji użytkownika.  
26. Dodaj test integracyjny logowania i dostępu do zasobu chronionego.  
27. Dodaj seeding początkowych użytkowników w DbContext.  
28. Dodaj swagger (`Swashbuckle`) do projektu.  
29. Skonfiguruj versioning API.  
30. Napisz testy, które sprawdzą różne wersje API.  
