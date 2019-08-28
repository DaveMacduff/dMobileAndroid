@echo off
set "projexe=C:\Work\Personal\ConvertFormToMultilanguage\ConvertFormToMultilanguage\bin\Debug"
set "markfile=C:\Users\Owner\Desktop\Marks.txt"
set "cachefile=C:\Users\Owner\Desktop\Cache.txt"

cmd /c echo Updating Forms for Multilanguage && %projexe%\ConvertFormToMultilanguage.exe /p C:\Work\dMobile\dMobile.View240x320\ /t C:\Work\dMobile\dMobile.Bedrock\Language\dMobileTranslation.ENU.txt /r /s /c %cachefile% /m %markfile%

echo Bedrock
timeout 5

cmd /c echo Updating Forms for Multilanguage && %projexe%\ConvertFormToMultilanguage.exe /p C:\Work\dMobile\dMobile.Bedrock\ /t C:\Work\dMobile\dMobile.Bedrock\Language\dMobileTranslation.ENU.txt /r /s /c %cachefile% /m %markfile%

echo Done
timeout 5

cmd /c echo Updating Forms for Multilanguage && %projexe%\ConvertFormToMultilanguage.exe /p C:\Work\dMobile\dMobile.BarcodeScanner\ /t C:\Work\dMobile\dMobile.Bedrock\Language\dMobileTranslation.ENU.txt /r /s /c %cachefile% /m %markfile%

timeout 5

cmd /c echo Updating Forms for Multilanguage && %projexe%\ConvertFormToMultilanguage.exe /p C:\Work\dMobile\dMobile.Controller\ /t C:\Work\dMobile\dMobile.Bedrock\Language\dMobileTranslation.ENU.txt /r /s /c %cachefile% /m %markfile%


timeout 5

cmd /c echo Updating Forms for Multilanguage && %projexe%\ConvertFormToMultilanguage.exe /p C:\Work\dMobile\dMobile.GroundWork\ /t C:\Work\dMobile\dMobile.Bedrock\Language\dMobileTranslation.ENU.txt /r /s /c %cachefile% /m %markfile%