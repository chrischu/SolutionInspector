@echo off

Shared\Build\OutputSplitter.exe "powershell" "-NonInteractive -Command "" & { . .\Build.ps1 -Mode "Local" -CreateNuGetPackages $True -CreateArchives $True }""" "Build.log"
if not %ERRORLEVEL%==0 goto build_failed
goto build_succeeded

:build_failed
pause
exit /b 1

:build_succeeded
pause
exit /b 0
