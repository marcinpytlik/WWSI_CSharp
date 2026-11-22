# Experiment 03: JIT vs ReadyToRun vs Composite ReadyToRun vs no-tiering.
# All variants: Release / linux-x64 / self-contained / identical source and dependencies.
# Native AOT is deliberately NOT enabled here; EF Core AOT needs separate compatibility work.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /repo
COPY Directory.Build.props Directory.Packages.props ./
COPY demonstracje/67_EfCorePerformanceLab/src/PerformanceLab.Api/ demonstracje/67_EfCorePerformanceLab/src/PerformanceLab.Api/

ARG LAB_COMPILATION_MODE=jit
RUN set -eu; \
    case "$LAB_COMPILATION_MODE" in \
      jit|no-tiering) extra="-p:PublishReadyToRun=false" ;; \
      r2r) extra="-p:PublishReadyToRun=true" ;; \
      composite) extra="-p:PublishReadyToRun=true -p:PublishReadyToRunComposite=true" ;; \
      *) echo "Invalid LAB_COMPILATION_MODE=$LAB_COMPILATION_MODE" >&2; exit 2 ;; \
    esac; \
    dotnet publish demonstracje/67_EfCorePerformanceLab/src/PerformanceLab.Api/PerformanceLab.Api.csproj \
      -c Release -r linux-x64 --self-contained true -p:UseAppHost=true \
      -p:PublishAot=false $extra -o /published

FROM mcr.microsoft.com/dotnet/runtime-deps:10.0
WORKDIR /app
COPY --from=build /published ./
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["/app/PerformanceLab.Api"]
