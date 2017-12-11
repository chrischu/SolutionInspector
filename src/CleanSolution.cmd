rem @echo off

rem Delete local build
DEL ".\Build.log"
RMDIR /S /Q ".\Build"

rem Delete NuGet packages
FOR /D %%P IN (.\packages\*) DO RMDIR /S /Q "%%P"

rem Delete all bin/obj folders
FOR /F "tokens=*" %%G IN ('DIR /B /AD /S bin') DO RMDIR /S /Q "%%G"
FOR /F "tokens=*" %%G IN ('DIR /B /AD /S obj') DO RMDIR /S /Q "%%G"

pause
