# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Directory.Build.props Directory.Packages.props global.json ./
COPY demonstracje/61_PostgresEf/ ./demonstracje/61_PostgresEf/
RUN dotnet restore demonstracje/61_PostgresEf/src/Demo61_PostgresEf.csproj && \
    dotnet publish demonstracje/61_PostgresEf/src/Demo61_PostgresEf.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "Demo61_PostgresEf.dll"]
