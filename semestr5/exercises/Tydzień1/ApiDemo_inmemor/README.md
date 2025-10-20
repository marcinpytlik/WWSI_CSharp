# MinimalApiDemo (.NET 8) - In-Memory Users

Endpoints:
- `GET /health` -> `{ "status": "OK" }`
- `GET /hello/{name}` -> `{ "message": "Hello, {name}!" }`
- `GET /users` -> list of users (in-memory)
- `POST /users` -> create user (JSON body)
- `GET /users/{id}` -> single user by id

## Run
```pwsh
dotnet restore
dotnet run --project src/MinimalApiDemo.csproj
# Example:
#   GET  http://localhost:5187/health
#   GET  http://localhost:5187/users
#   POST http://localhost:5187/users   body: { "username": "marcin", "email": "m@x.com" }
```

## Sample cURL
```bash
curl http://localhost:5187/users

curl -X POST http://localhost:5187/users \
  -H "Content-Type: application/json" \
  -d '{ "username": "marcin", "email": "marcin@example.com" }'
```

## Files
```
/MinimalApiDemo/
  .vscode/
    launch.json
    tasks.json
  src/
    MinimalApiDemo.csproj
    Program.cs
    Models/
      User.cs
      UserCreateDto.cs
```
