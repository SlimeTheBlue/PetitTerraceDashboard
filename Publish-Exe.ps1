param([string]$DotnetPath='dotnet')
$ErrorActionPreference='Stop'
& $DotnetPath publish "$PSScriptRoot/PetitDashboard.csproj" -c Release -r win-x64 --self-contained true -p:UseAppHost=true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:PublishTrimmed=false -o "$PSScriptRoot/artifacts/publish" --source 'https://api.nuget.org/v3/index.json' --disable-build-servers --nologo
if($LASTEXITCODE -ne 0){throw 'Publish failed'}
