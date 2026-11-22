# Dedicated tools sidecar, deliberately separate from the measured API image.
FROM mcr.microsoft.com/dotnet/sdk:10.0
RUN apt-get update && apt-get install -y --no-install-recommends util-linux && rm -rf /var/lib/apt/lists/*
RUN dotnet tool install dotnet-counters --tool-path /tools \
 && dotnet tool install dotnet-trace --tool-path /tools
COPY demonstracje/67_EfCorePerformanceLab/docker/collect-counters.sh /tools/collect-counters.sh
RUN sed -i 's/\r$//' /tools/collect-counters.sh && chmod 755 /tools/collect-counters.sh
ENTRYPOINT ["sleep", "infinity"]
