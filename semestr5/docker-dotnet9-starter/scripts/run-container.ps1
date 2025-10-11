# Starts a PowerShell session inside the .NET 9 SDK container with the current folder mounted at /workspace
param(
  [string]$Image = "mcr.microsoft.com/dotnet/sdk:9.0"
)

$workdir = (Get-Location).Path
docker run --rm -it -v "$workdir:/workspace" -w /workspace $Image pwsh
