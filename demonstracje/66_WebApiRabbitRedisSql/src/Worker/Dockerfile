FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /repo
COPY Directory.Build.props Directory.Packages.props ./
COPY demonstracje/66_WebApiRabbitRedisSql/src/Shared/ demonstracje/66_WebApiRabbitRedisSql/src/Shared/
COPY demonstracje/66_WebApiRabbitRedisSql/src/Worker/ demonstracje/66_WebApiRabbitRedisSql/src/Worker/
RUN dotnet publish demonstracje/66_WebApiRabbitRedisSql/src/Worker/Worker.csproj -c Release -o /app
FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet","Worker.dll"]
