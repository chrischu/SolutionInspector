@echo off

Shared\Build\OutputSplitter.exe "powershell" "-NonInteractive -Command "" & { . .\Build.ps1 -Mode "Local" -Version ""0.0.0"" -RunTests $False }""" "Build.log"
if not %ERRORLEVEL%==0 goto build_failed
goto build_succeeded

:build_failed
pause
exit /b 1

:build_succeeded
exit /b 0
