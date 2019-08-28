@echo off
set "executable=C:\Work\Personal\ExcelWriter\ExcelWriter\bin\Debug\ExcelWriter.exe"
set "outputfile=C:\Users\Owner\Desktop\Exported dMobile Languages.xlsx"
set "englishfile=C:\Work\dMobile\dMobile.Bedrock\Language\dMobileTranslation.ENU.txt"
set "frenchfile=C:\Work\dMobile\dMobile.Bedrock\Language\dMobileTranslation.FRC.txt"

:: if you need more languages just append to the end

"%executable%" export "%outputfile%" "%englishfile%" "%frenchfile%"

timeout /t 5