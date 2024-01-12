[CmdletBinding()]
Param(
    [Parameter(Mandatory=$false)]
    [switch]$UnitTests,
    [Parameter(Mandatory=$false)]
    [switch]$IntegrationTests,
    [Parameter(Mandatory=$false)]
    [switch]$GenerateReport
)

. $PSScriptRoot\ps\RepositoryUtilities.ps1
. $PSScriptRoot\ps\DotNet.ps1

$ErrorActionPreference = 'Stop'

function Invoke-TestProject
{
    [CmdletBinding()]
    Param (
        [Parameter(Mandatory=$true)]
        [string]$Project
    )

    Invoke-DotNetTest $Project @("/p:CollectCoverage=true", "/p:CoverletOutputFormat=cobertura")
}

# Run all tests if neither switch is specified
if (!$UnitTests -and !$IntegrationTests)
{
    $UnitTests = $true
    $IntegrationTests = $true
}

# Test project paths
$script:IntegrationTestsProjectPath = Get-RelativeRepositoryPath -Path "src/tests/SharpRacer.SourceGenerators.IntegrationTests/SharpRacer.SourceGenerators.IntegrationTests.csproj"
$script:UnitTestsProjectPath = Get-RelativeRepositoryPath -Path "src/tests/SharpRacer.SourceGenerators.UnitTests/SharpRacer.SourceGenerators.UnitTests.csproj"

# Coverage output files
$script:IntegrationTestsCoverageFile = Get-RelativeRepositoryPath -Path "artifacts/test/coverage/SharpRacer.SourceGenerators.IntegrationTests/coverage.cobertura.xml"
$script:IntegrationTestsReportPath =  Get-RelativeRepositoryPath -Path "artifacts/test/reports/SharpRacer.SourceGenerators.IntegrationTests"
$script:UnitTestsCoverageFile = Get-RelativeRepositoryPath -Path "artifacts/test/coverage/SharpRacer.SourceGenerators.UnitTests/coverage.cobertura.xml"
$script:UnitTestsReportPath =  Get-RelativeRepositoryPath -Path "artifacts/test/reports/SharpRacer.SourceGenerators.UnitTests"
$script:CombinedTestReportPath = Get-RelativeRepositoryPath -Path "artifacts/test/reports/SharpRacer.SourceGenerators"

if ($UnitTests)
{
    Write-Output "Running unit tests..."
    Invoke-TestProject $script:UnitTestsProjectPath

    if ($GenerateReport)
    {
        Write-Output "Generating coverage report..."

        New-TestCoverageReport `
            -CoverageFiles @($script:UnitTestsCoverageFile) `
            -OutputDirectory $script:UnitTestsReportPath `
            -ReportTitle "SharpRacer.SourceGenerators Unit Test Results" `
            -ExcludedAssemblies @("SharpRacer")
    }
}

if ($IntegrationTests)
{
    Write-Output "Running integration tests..."
    Invoke-TestProject $script:IntegrationTestsProjectPath

    if ($GenerateReport)
    {
        Write-Output "Generating coverage report..."

        New-TestCoverageReport `
            -CoverageFiles @($script:IntegrationTestsCoverageFile) `
            -OutputDirectory $script:IntegrationTestsReportPath `
            -ReportTitle "SharpRacer.SourceGenerators Integration Test Results" `
            -ExcludedAssemblies @("SharpRacer")
    }
}

# Generate combined report
if ($GenerateReport)
{
    New-TestCoverageReport `
        -CoverageFiles @($script:UnitTestsCoverageFile, $script:IntegrationTestsCoverageFile) `
        -OutputDirectory $script:CombinedTestReportPath `
        -ReportTitle "SharpRacer.SourceGenerators Combined Test Results" `
        -ExcludedAssemblies @("SharpRacer")
}
