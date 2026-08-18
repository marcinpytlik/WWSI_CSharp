# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Directory.Build.props Directory.Packages.props global.json ./
COPY demonstracje/40_MinioUploadSql/ ./demonstracje/40_MinioUploadSql/
RUN dotnet restore demonstracje/40_MinioUploadSql/src/Demo40_MinioUploadSql.csproj && \
    dotnet publish demonstracje/40_MinioUploadSql/src/Demo40_MinioUploadSql.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "Demo40_MinioUploadSql.dll"]
