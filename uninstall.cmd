@echo off
setlocal


:Main: "args="
:: Some initialization work
call :Initialize
call :PrintHeader

:: First argument required
set "adminSwitch=%~1"

:: Check admin rights
call :IsElevatedCMD
if not "%errorlevel%"=="0" (
    if "%adminSwitch%"=="/uac" (
        call :PrintFooter "Failed to elevate CMD."
        call :Exit
    ) else (
        call :PrintFooter "Elevating..."
        call :RestartWithUAC "%setupSwitch%"
    )
)

:: Uninstall Svetomech.GamesCleaner
sc.exe delete Svetomech.GamesCleaner

call :PrintFooter "Done!"
call :Exit

exit


:: PRIVATE

:PrintHeader: ""
echo #######################################################
echo ##        Svetomech.GamesCleaner Setup               ##
echo #######################################################
exit /b

:PrintFooter: "message"
echo %~1
echo.
echo /-------------------------------------------------------------------\
echo  Fork me on GitHub: https://github.com/Svetomech/GamesCleaner
echo \-------------------------------------------------------------------/
exit /b

:: PUBLIC

:Initialize: ""
title %~n0
color 07
cls
chdir /d "%~dp0"
exit /b

:IsElevatedCMD: ""
set "errorlevel=0"
net session >nul 2>&1 || set "errorlevel=1"
exit /b %errorlevel%

:RestartWithUAC: "args="
set "_helperPath=%temp%\%~n0.helper-%random%.vbs"
echo Set UAC = CreateObject^("Shell.Application"^) > "%_helperPath%"
echo UAC.ShellExecute "%~f0", "/uac", "", "runas", 1 >> "%_helperPath%"
cscript "%_helperPath%" //b //nologo >nul 2>&1
erase /f /s /q /a "%_helperPath%" >nul 2>&1
set "_helperPath="
exit

:Exit: ""
timeout /t 2 /nobreak >nul 2>&1
exit