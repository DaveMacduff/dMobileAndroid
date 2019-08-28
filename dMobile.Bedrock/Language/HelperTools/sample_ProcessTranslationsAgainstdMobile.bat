@echo off
set "executable=C:\Work\Personal\ExcelWriter\ExcelWriter\bin\Release\ExcelWriter.exe"
set "excelfile=C:\Users\Owner\Desktop\dMobile Translation\June 8 2016 Translations\excel_fixed.xlsx"
set "translatedtoken=%~dp0translationEnglishFrench.txt"
set "translatedengcol=2"
set "translatedfrccol=3"
set "header=1"


set "dmobileeng=C:\Work\dMobile\dMobile.Bedrock\Language\dMobileTranslation.ENU.txt"
set "dmobilefrc=C:\Work\dMobile\dMobile.Bedrock\Language\dMobileTranslation.FRC.txt"
set "dmobileengfrcmerge=%~dp0%engfrcmerged.txt"

set "exclusion=%~dp0exclusion.txt"

set "cleaneng=%~dp0cleanEnglish.txt"
set "cleanfrc=%~dp0cleanFrench.txt"

set "finalexcel=%~dp0newTranslations.xlsx"

:: columns have 1-based indexing

echo.
echo Step 1 Import Translations done by translator
"%executable%" import "%excelfile%" "%translatedtoken%" %translatedengcol% %translatedfrccol% %header%
pause

echo.
echo Step 2 Merge English and French dMobile automatic translations
"%executable%" merge "%dmobileeng%" "%dmobilefrc%" "%dmobileengfrcmerge%"
pause

echo.
echo Step 3 Create exclusion file
echo - List of lines where the automatic translation matches the manually translated
echo - We have less words to give to the translator
"%executable%" fuzzy "%translatedtoken%" "%dmobileengfrcmerge%" "%exclusion%" 0 1
pause

echo.
echo Step 4 Remove exclusion list from dMobile translation files
"%executable%" remove "%dmobileeng%" "%exclusion%" "%cleaneng%"
"%executable%" remove "%dmobilefrc%" "%exclusion%" "%cleanfrc%"
pause

echo.
echo Step 5 Export Cleaned dMobile translation files to excel
"%executable%" export "%finalexcel%" "%cleaneng%" "%cleanfrc%"
pause

timeout /t 5