@echo off
setlocal enabledelayedexpansion

echo Building the application...
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true

echo Searching for the built EXE...
for /r ".\bin\Release" %%F in (*.exe) do (
    set "exePath=%%F"
)

if not defined exePath (
    echo Error: EXE file not found. Ensure the project is properly built.
    exit /b 1
)

echo Moving EXE to the main directory...
move /Y "!exePath!" "%~dp0"

echo Install complete! You can now run the application from the main folder.
pause
