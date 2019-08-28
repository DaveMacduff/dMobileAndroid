@echo off
set "executable=C:\Work\Personal\ExcelWriter\ExcelWriter\bin\Debug\ExcelWriter.exe"
set "excelfile=C:\Users\Owner\Desktop\Exported dMobile Languages.xlsx"
set "outputfile=C:\Users\Owner\Desktop\Imported French.txt"
set "keycolumn=1"
set "languagecolumn=3"
set "header=1"

:: columns have 1-based indexing

:: if you need more languages just append to the end

"%executable%" import "%excelfile%" "%outputfile%" %keycolumn% %languagecolumn% %header%

timeout /t 5