$customDate = GET-Date -format yyyyMMdd_HH_mm
$projpath = {C:\Users\Owner\Documents\Visual Studio 2008\Projects\dMobileAutoUpdates\Client}

& $env:windir\Microsoft.NET\Framework\v3.5\MSBuild.exe "$projpath\Client.csproj" /t:AllPlatforms /p:TargetEnvPropsFile=EnvConfig\Env-PCJUNK.csproj /p:DateFormat=$customDate

Write-Host "Press any key to continue ..."s
$x = $host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")