FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /repo
COPY Directory.Build.props Directory.Packages.props ./
COPY demonstracje/66_WebApiRabbitRedisSql/src/Shared/ demonstracje/66_WebApiRabbitRedisSql/src/Shared/
COPY demonstracje/66_WebApiRabbitRedisSql/tools/DlqReplay/ demonstracje/66_WebApiRabbitRedisSql/tools/DlqReplay/
RUN dotnet publish demonstracje/66_WebApiRabbitRedisSql/tools/DlqReplay/DlqReplay.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "DlqReplay.dll"]
