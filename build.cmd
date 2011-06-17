@echo off

set msbuild=%windir%\Microsoft.NET\Framework\v4.0.30319\MSBuild
set cfg=%1%

if "%cfg%"=="" set cfg=Release

%msbuild% OoMapper.build /t:Full /p:Configuration=%cfg%

pause