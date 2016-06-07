@echo off

REM in order to get the result (commit hash) from the git command (or any command) we have to use the "for loop pattern".
REM this seems to be the default way in cmd
set git="C:\Program Files\Git\bin\git.exe"
for /f "delims=" %%a in ('%git% rev-parse HEAD') do @set commitHash=%%a

Shared\Build\OutputSplitter.exe "powershell" "-NonInteractive -Command "" & { . .\Build.ps1 -Mode "Local" -Version ""0.0.0"" -CommitHash ""%commitHash%"" -RunFxCopCodeAnalysis $False -RunReSharperCodeInspection $False }""" "Build.log"
if not %ERRORLEVEL%==0 goto build_failed
goto build_succeeded

:build_failed
pause
exit /b 1

:build_succeeded
exit /b 0
