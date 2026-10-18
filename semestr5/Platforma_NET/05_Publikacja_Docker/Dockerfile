FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY semestr5/Platforma_NET/01_Pipeline_DI/src/ ./app/
RUN dotnet publish ./app/PipelineDemo.csproj -c Release -o /out --self-contained false

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /out/ ./
ENTRYPOINT ["dotnet", "PipelineDemo.dll"]
