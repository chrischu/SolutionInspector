@echo off

echo $version = .\Shared\Build\Versioning.ps1 > tmp.ps1
echo .\Build.ps1 `>> tmp.ps1
echo   -Mode "Local" `>> tmp.ps1
echo   -Version $version `>> tmp.ps1
echo   -IsPreRelease $True `>> tmp.ps1
echo   -CreateNuGetPackages $True `>> tmp.ps1
echo   -CreateArchives $True >> tmp.ps1

Shared\Build\OutputSplitter.exe "powershell" "-NonInteractive -File tmp.ps1" "Build.log"
del tmp.ps1

if not %ERRORLEVEL%==0 goto build_failed
goto build_succeeded

:build_failed
pause
exit /b 1

:build_succeeded
pause
exit /b 0
