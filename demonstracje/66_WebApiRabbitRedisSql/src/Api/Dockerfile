FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /repo
COPY Directory.Build.props Directory.Packages.props ./
COPY demonstracje/66_WebApiRabbitRedisSql/src/Shared/ demonstracje/66_WebApiRabbitRedisSql/src/Shared/
COPY demonstracje/66_WebApiRabbitRedisSql/src/Api/ demonstracje/66_WebApiRabbitRedisSql/src/Api/
RUN dotnet publish demonstracje/66_WebApiRabbitRedisSql/src/Api/Api.csproj -c Release -o /app
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet","Api.dll"]
