FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY semestr3/Platforma_NET/01_Kompilacja_IL_JIT/src/ ./app/
RUN dotnet publish ./app/IlDemo.csproj -c Release -o /out --self-contained false

FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app
COPY --from=build /out/ ./
ENTRYPOINT ["dotnet", "IlDemo.dll"]
