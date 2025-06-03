param(
    [Parameter(Mandatory = $false)]
    [string]$SolutionName,
    [string]$ProjectName,
    [switch]$WithTest,
    [switch]$Help
)

if ($Help -or -not $SolutionName) {
    Write-Host "Usage: .\create-external-logic.ps1 -SolutionName <SolutionName> [-ProjectName <ProjectName>] [-WithTest] [-Help]"
    Write-Host ""
    Write-Host "Parameters:"
    Write-Host "  -SolutionName   (required)   Name of the solution and default project name"
    Write-Host "  -ProjectName    (optional)   Name of the project (defaults to SolutionName)"
    Write-Host "  -WithTest       (optional)   Add this switch to create a test project"
    Write-Host "  -Help           (optional)   Show this help message"
    Write-Host ""
    Write-Host "Example:"
    Write-Host "  .\create-external-logic.ps1 -SolutionName QiitaSamples -ProjectName ExternalLogicExample1 -WithTest"
    exit 0
}

if (-not $ProjectName) {
    $ProjectName = $SolutionName
}

# Create solution
dotnet new sln -o $SolutionName
cd $SolutionName

# Create class library project
dotnet new classlib -o $ProjectName --framework net8.0
dotnet sln add .\$ProjectName\$ProjectName.csproj

cd $ProjectName
dotnet add package OutSystems.ExternalLibraries.SDK
cd ..

if ($WithTest) {
    # Create test project
    $TestProjectName = "$ProjectName.Tests"
    dotnet new xunit -o $TestProjectName
    dotnet sln add .\$TestProjectName\$TestProjectName.csproj
    dotnet add .\$TestProjectName\$TestProjectName.csproj reference .\$ProjectName\$ProjectName.csproj
}

Write-Host "Setup complete."