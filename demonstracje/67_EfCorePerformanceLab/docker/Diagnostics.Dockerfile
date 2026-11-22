# Dedicated tools sidecar, deliberately separate from the measured API image.
FROM mcr.microsoft.com/dotnet/sdk:10.0
RUN dotnet tool install dotnet-counters --tool-path /tools \
 && dotnet tool install dotnet-trace --tool-path /tools
ENTRYPOINT ["sleep", "infinity"]
