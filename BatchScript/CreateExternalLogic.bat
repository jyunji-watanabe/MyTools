@echo off
setlocal

:: Show help if /? or -h or -help is given, or if no solution name is provided
if "%~1"=="" goto :help
if "%~1"=="/?" goto :help
if "%~1"=="-h" goto :help
if "%~1"=="-help" goto :help

set "SolutionName=%~1"
set "ProjectName=%~2"
set "WithTest=%~3"

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

if /I "%WithTest%"=="-WithTest" (
    set "TestProjectName=%ProjectName%.Tests"
    dotnet new xunit -o %TestProjectName%
    dotnet sln add .\%TestProjectName%\%TestProjectName%.csproj
    dotnet add .\%TestProjectName%\%TestProjectName%.csproj reference .\%ProjectName%\%ProjectName%.csproj
)

echo Setup complete.
goto :eof

:help
echo Usage: create-external-logic.bat SolutionName [ProjectName] [-WithTest] [-h]
echo.
echo   SolutionName   (required)   Name of the solution and default project name
echo   ProjectName    (optional)   Name of the project (defaults to SolutionName if you pass "" as this parameter)
echo   -WithTest      (optional)   Add this switch to create a test project
echo   -h, -help, /?  (optional)   Show this help message
echo.
echo Example:
echo   create-external-logic.bat QiitaSamples ExternalLogicExample1 -WithTest
exit /b 0