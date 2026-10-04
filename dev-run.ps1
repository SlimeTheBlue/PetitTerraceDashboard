param([string]$DotnetPath = 'dotnet', [switch]$SkipBuild)
$ErrorActionPreference = 'Stop'
$dashboardProjectRoot = $PSScriptRoot
if (-not $SkipBuild) {
    & $DotnetPath build (Join-Path $dashboardProjectRoot 'PetitDashboard.csproj') --configfile (Join-Path $dashboardProjectRoot 'NuGet.Config') --disable-build-servers --nologo
    if ($LASTEXITCODE -ne 0) { throw '개발 빌드가 실패했습니다. .NET 10 SDK 경로를 확인하세요.' }
}
$dashboardDll = Join-Path $dashboardProjectRoot 'bin\Debug\net10.0-windows\PetitDashboard.dll'
if (-not (Test-Path -LiteralPath $dashboardDll)) { throw '개발 DLL이 없습니다. 먼저 빌드해 주세요.' }
& $DotnetPath $dashboardDll
