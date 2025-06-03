@echo off
setlocal

:: Show help if /? or -h or -help is given, or if no solution name is provided
if "%~1"=="" goto :help
if "%~1"=="/?" goto :help
if "%~1"=="-h" goto :help
if "%~1"=="-help" goto :help

set "SolutionName=%~1"
set "ProjectName=%~2"

if "%ProjectName%"=="" set "ProjectName=%SolutionName%"

:: Create solution
dotnet new sln -o %SolutionName%
cd %SolutionName%

:: Create class library project
dotnet new classlib -o %ProjectName% --framework net8.0
dotnet sln add .\%ProjectName%\%ProjectName%.csproj

cd %ProjectName%
dotnet add package OutSystems.ExternalLibraries.SDK
cd ..

set "TestProjectName=%ProjectName%.Tests"
dotnet new xunit -o %TestProjectName%
dotnet sln add .\%TestProjectName%\%TestProjectName%.csproj
dotnet add .\%TestProjectName%\%TestProjectName%.csproj reference .\%ProjectName%\%ProjectName%.csproj

echo Setup complete.
goto :eof

:help
echo Usage: create-external-logic.bat SolutionName [ProjectName] [-h]
echo.
echo   SolutionName   (required)   Name of the solution and default project name
echo   ProjectName    (optional)   Name of the project (defaults to SolutionName)
echo   -h, -help, /?  (optional)   Show this help message
echo.
echo Example:
echo   create-external-logic.bat QiitaSamples ExternalLogicExample1
exit /b 0