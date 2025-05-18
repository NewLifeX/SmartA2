@echo off

set name=StarWeb
set clover=..\..\Tools\clover.exe
if not exist "%clover%" (
    set clover=..\..\Doc\clover.exe
)

for %%f in (*.exe) do (
    rem 获取文件名（去掉扩展名）
    set "name=%%~nf"
    goto :found
)

:found
if defined name (
    rd runtimes\osx-arm64 /s/q
    rd runtimes\osx-x64 /s/q
    rd runtimes\win /s/q
    rd runtimes\linux-arm64 /s/q
    rd runtimes\linux-x64 /s/q
    del %name%.zip /f/q
    %clover% zip -r %name%.zip *.exe *.dll *.pdb *.so appsettings.json *.runtimeconfig.json *.deps.json
) else (
    echo No exe file found in the current directory.
)
