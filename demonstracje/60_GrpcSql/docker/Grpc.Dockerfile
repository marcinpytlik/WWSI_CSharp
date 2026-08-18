# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Directory.Build.props Directory.Packages.props global.json ./
COPY demonstracje/60_GrpcSql/ ./demonstracje/60_GrpcSql/
RUN dotnet restore demonstracje/60_GrpcSql/src/Grpc/Demo60_Grpc.csproj && \
    dotnet publish demonstracje/60_GrpcSql/src/Grpc/Demo60_Grpc.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "Demo60_Grpc.dll"]
