FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /repo
COPY Directory.Build.props Directory.Packages.props ./
COPY demonstracje/67_EfCorePerformanceLab/src/PerformanceLab.Api/ demonstracje/67_EfCorePerformanceLab/src/PerformanceLab.Api/
RUN dotnet publish demonstracje/67_EfCorePerformanceLab/src/PerformanceLab.Api/PerformanceLab.Api.csproj -c Release -o /published
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /published ./
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "PerformanceLab.Api.dll"]
