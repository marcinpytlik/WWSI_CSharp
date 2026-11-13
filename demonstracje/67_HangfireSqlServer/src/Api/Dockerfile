FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /repo
COPY Directory.Build.props Directory.Packages.props ./
COPY demonstracje/67_HangfireSqlServer/src/Shared/ demonstracje/67_HangfireSqlServer/src/Shared/
COPY demonstracje/67_HangfireSqlServer/src/Api/ demonstracje/67_HangfireSqlServer/src/Api/
RUN dotnet publish demonstracje/67_HangfireSqlServer/src/Api/Api.csproj -c Release -o /app
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Api.dll"]
