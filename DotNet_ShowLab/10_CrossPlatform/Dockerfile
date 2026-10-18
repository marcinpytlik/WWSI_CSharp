FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY DotNet_ShowLab/03_Async_Parallel/src/ ./app/
RUN dotnet publish ./app/Demo.csproj -c Release -o /out --self-contained false
FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app
COPY --from=build /out/ .
ENTRYPOINT ["dotnet", "Demo.dll"]
