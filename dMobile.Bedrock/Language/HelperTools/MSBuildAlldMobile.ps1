# Edit the Local Paths then run this script (Right-click > Run with Powershell).

##### Local Paths - EDIT THESE #####

#MSBuild Location
$msbuild = "$env:windir\Microsoft.NET\Framework\v3.5\MSBuild.exe"
# Project Paths
$proj240x320Path = "C:\Work\dMobile\dMobile.View240x320\dMobile.View240x320.csproj"
$proj320x240Path = "C:\Work\dMobile\dMobile.View320x240\dMobile.View320x240.csproj"
# Cab Wizard Local Location
$cabwizProperties = "CabWizExe=c:\Program Files (x86)\Microsoft Visual Studio 9.0\smartdevices\sdk\sdktools\cabwiz.exe"

##### End Local Paths #####




# Custom Date formatting
$customDate = GET-Date -format yyyyMMdd_HH_mm

# releaseServer properties (Where files are stored internally for manual release)
$pcjunkName = "\\DTEC3"
$pcjunkUser = "robert"
$pcjunkPass = "XK1ByPhrLwMtvzZgervfvJvf3PB1TX5k"
$pcjunkAuthentication = "ServerName=$pcjunkName;LoginUser=$pcjunkUser;LoginPassword=$pcjunkPass"

# AutoUpdate DTEC2 properties (Where files are stored internally for internal automatic update)
$autoUpdateName = "\\DTEC3"
$autoUpdateUser = "robert"
$autoUpdatePass = "XK1ByPhrLwMtvzZgervfvJvf3PB1TX5k"
$autoUpdateProperties = "ServerName=$autoUpdateName;LoginUser=$autoUpdateUser;LoginPassword=$autoUpdatePass"

# LXE Settings
$LXEDevice = "LXE"
$LXEDeviceLong = "LXE MX7"
$LXEPlatform = "WindowsCE"
$LXEPlatformID = "E2BECB1F-8C8C-41ba-B736-9BE7D946A398"
$LXEOS = "5.00"
$LXENative = "Windows CE"
$LXEServerFilePath = "\((FTP))\((ship))\dMobile\Releases\AutoUpdate Programs\dMobile\dMobile $LXEDevice $customDate"
$LXEServerCabPath = "\((FTP))\((ship))\dMobile\Releases\LXE MX7 WinCE\dMobile $LXEDeviceLong $($customDate)_00.CAB"
$LXEProperties = "DeviceName=$LXEDevice;PlatformFamilyName=$LXEPlatform;PlatformID=$LXEPlatformID;OSVersion=$LXEOS;NativePlatformName=$LXENative;ToServerFilePath=$LXEServerFilePath;ToServerCabPath=$LXEServerCabPath"

# Psion Settings
$PsionDevice = "Psion"
$PsionDeviceLong = "Psion"
$PsionPlatform = "WindowsCE"
$PsionPlatformID = "E2BECB1F-8C8C-41ba-B736-9BE7D946A398"
$PsionOS = "5.00"
$PsionNative = "Windows CE"
$PsionServerFilePath = "\((FTP))\((ship))\dMobile\Releases\AutoUpdate Programs\dMobile\dMobile $PsionDevice $customDate"
$PsionServerCabPath = "\((FTP))\((ship))\dMobile\Releases\Psion Omnii XT15f\dMobile $PsionDeviceLong $($customDate)_00.CAB"
$PsionProperties = "DeviceName=$PsionDevice;PlatformFamilyName=$PsionPlatform;PlatformID=$PsionPlatformID;OSVersion=$PsionOS;NativePlatformName=$PsionNative;ToServerFilePath=$PsionServerFilePath;ToServerCabPath=$PsionServerCabPath"

# Intermec Settings
$IntermecDevice = "Intermec"
$IntermecDeviceLong = "Intermec WM6"
$IntermecPlatform = "PocketPC"
$IntermecPlatformID = "b2c48bd2-963d-4549-9169-1fa021dce484"
$IntermecOS = "5.02"
$IntermecNative = "Windows Mobile 6 Professional SDK"
$IntermecServerFilePath = "\((FTP))\((ship))\dMobile\Releases\AutoUpdate Programs\dMobile\dMobile $IntermecDevice $customDate"
$IntermecServerCabPath = "\((FTP))\((ship))\dMobile\Releases\Intermec WM6\dMobile $IntermecDeviceLong $($customDate)_00.CAB"
$IntermecProperties = "DeviceName=$IntermecDevice;PlatformFamilyName=$IntermecPlatform;PlatformID=$IntermecPlatformID;OSVersion=$IntermecOS;NativePlatformName=$IntermecNative;ToServerFilePath=$IntermecServerFilePath;ToServerCabPath=$IntermecServerCabPath"

$LXEBuildProperties = "DateFormat=$customDate;$pcjunkAuthentication;$cabwizProperties;$LXEProperties"
$IntermecProperties = "DateFormat=$customDate;$pcjunkAuthentication;$cabwizProperties;$IntermecProperties"
$PsionBuildProperties = "DateFormat=$customDate;$pcjunkAuthentication;$cabwizProperties;$PsionProperties"


##### Program Execution #####

# Build View240x320 #
# Build LXE
Write-Host "Building LXE 240x320 Project"
& $msbuild $proj240x320Path /t:AllPlatforms /p:$LXEBuildProperties

# Build Intermec
Write-Host "Building Intermec 240x320 Project"
& $msbuild $proj240x320Path /t:AllPlatforms /p:$IntermecProperties

# Build Psion
Write-Host "Building Psion 240x320 Project"
& $msbuild $proj240x320Path /t:AllPlatforms /p:$PsionBuildProperties

Write-Host ""
Write-Host "Finished building and copying LXE 240x320"
Write-Host "LXE AutoBuild files      $pcjunkName$LXEServerFilePath"
Write-Host "LXE Cab file             $pcjunkName$LXEServerCabPath"
Write-Host ""
Write-Host "Finished building and copying Intermec 240x320"
Write-Host "Intermec AutoBuild files $pcjunkName$IntermecServerFilePath"
Write-Host "Intermec Cab file        $pcjunkName$IntermecServerCabPath"
Write-Host ""
Write-Host "Finished building and copying Psion 240x320"
Write-Host "Psion AutoBuild files $pcjunkName$PsionServerFilePath"
Write-Host "Psion Cab file        $pcjunkName$PsionServerCabPath"
Write-Host ""
Write-Host "Press any key to continue ..."s
$x = $host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")