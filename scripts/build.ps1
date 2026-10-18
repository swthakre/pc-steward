param([switch]$Publish)
$ErrorActionPreference = 'Stop'
Set-Location (Split-Path $PSScriptRoot)
$env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'
$env:DOTNET_CLI_HOME = Join-Path $PWD '.tools/cli-home'
$sdk = Join-Path $PWD '.tools/dotnet/dotnet.exe'
if (!(Test-Path $sdk)) { $sdk = 'dotnet' }
& $sdk restore PCSteward.slnx --configfile NuGet.Config
if ($LASTEXITCODE) { throw 'Restore failed' }
& $sdk build PCSteward.slnx -c Release --no-restore
if ($LASTEXITCODE) { throw 'Build failed' }
& $sdk run --project tests/Steward.Tests -c Release --no-build
if ($LASTEXITCODE) { throw 'Tests failed' }
& $sdk run --project tests/Steward.UiTests -c Release --no-build
if ($LASTEXITCODE) { throw 'Native UI tests failed' }
if ($Publish) {
    & $sdk publish src/Steward.App -c Release -r win-x64 --self-contained true -o artifacts/PCSteward-win-x64
    if ($LASTEXITCODE) { throw 'Publish failed' }
    Copy-Item LICENSE,THIRD-PARTY-NOTICES.md,README.md artifacts/PCSteward-win-x64
    $sdkRoot = Split-Path (Get-Command $sdk).Source
    Copy-Item (Join-Path $sdkRoot 'LICENSE.txt') artifacts/PCSteward-win-x64/DOTNET-LICENSE.txt
    Copy-Item (Join-Path $sdkRoot 'ThirdPartyNotices.txt') artifacts/PCSteward-win-x64/DOTNET-ThirdPartyNotices.txt
    Get-FileHash artifacts/PCSteward-win-x64/Steward.App.exe -Algorithm SHA256 | Format-List | Out-File artifacts/PCSteward-win-x64/SHA256.txt
}
