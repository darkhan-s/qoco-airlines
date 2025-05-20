@echo off 

cd /d %~dp0

dotnet clean
dotnet restore
dotnet build
dotnet test

IF %ERRORLEVEL% NEQ 0 (
    echo Tests failed..
	pause
    exit /b %ERRORLEVEL%
)
dotnet run --project src\Qoco-Airlines.csproj

pause