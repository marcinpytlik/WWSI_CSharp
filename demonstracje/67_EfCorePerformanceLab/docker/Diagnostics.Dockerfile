# Dedicated tools sidecar, deliberately separate from the measured API image.
FROM mcr.microsoft.com/dotnet/sdk:10.0
RUN apt-get update && apt-get install -y --no-install-recommends util-linux && rm -rf /var/lib/apt/lists/*
RUN dotnet tool install dotnet-counters --tool-path /tools \
 && dotnet tool install dotnet-trace --tool-path /tools
ENTRYPOINT ["sleep", "infinity"]
