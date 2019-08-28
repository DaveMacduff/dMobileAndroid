@echo off

set "programLocation=C:\Work\dMobile\dMobile.Bedrock\Language\HelperTools\Copy240x320Viewsto320x240.exe"
set "fromdir=C:\Work\dMobile\dMobile.View240x320"
set "todir=C:\Work\dMobile\dMobile.View320x240"
set "copiedFileList=C:\Users\Owner\Desktop\NewFilesList.txt"

echo Be sure to check out files that will be changing
echo Copy from %fromdir%
echo Copy to %todir%
pause

%programLocation% %fromdir% %todir% /new %copiedFileList%

echo Complete.
echo Updated file list %copiedFileList%
pause