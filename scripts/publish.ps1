$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
$publish = Join-Path $root "artifacts\portable"

dotnet run --project (Join-Path $root "tests\JobbKalk.FormulaTests\JobbKalk.FormulaTests.csproj") -c Release
dotnet publish (Join-Path $root "src\JobbKalk\JobbKalk.csproj") `
  -c Release -r win-x64 --self-contained true -o $publish `
  -p:PublishSingleFile=true `
  -p:IncludeNativeLibrariesForSelfExtract=true `
  -p:EnableCompressionInSingleFile=true `
  -p:DebugType=None `
  -p:DebugSymbols=false

Write-Host "Bærbar versjon: $publish\JobbKalk.exe"
