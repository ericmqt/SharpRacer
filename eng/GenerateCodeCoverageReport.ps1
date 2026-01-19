<#
    .SYNOPSIS
    Generates code coverage reports using ReportGenerator.

    .PARAMETER OutputDirectory
    The directory where generated reports will be stored.

    .PARAMETER SharpRacer
    Generates code coverage for the main SharpRacer library.

    .PARAMETER SourceGenerator
    Generates code coverage for the SharpRacer.SourceGenerators library.

    .PARAMETER ExcludeIntegrationTests
    Excludes integration tests from code coverage reports.

    .PARAMETER Json
    Generate a JSON code coverage report for each library in the $OutputDirectory/json directory.

    .PARAMETER Html
    Generate an HTML code coverage report for each library in the $OutputDirectory/html directory.

    .PARAMETER NoBuild
    If specified, projects will not be compiled before running tests.
#>


[CmdletBinding()]
Param(
    [Parameter(Mandatory=$true)]
    [string]$OutputDirectory,
    [Parameter(Mandatory=$false)]
    [switch]$SharpRacer,
    [Parameter(Mandatory=$false)]
    [switch]$SourceGenerator,
    [Parameter(Mandatory=$false)]
    [switch]$ExcludeIntegrationTests,
    [Parameter(Mandatory=$false)]
    [switch]$Json,
    [Parameter(Mandatory=$false)]
    [switch]$Html,
    [Parameter(Mandatory=$false)]
    [switch]$NoBuild,
    [Parameter(Mandatory=$false)]
    [string]$Configuration = "Debug"
)

. $PSScriptRoot\ps\CodeCoverage.ps1
. $PSScriptRoot\ps\ReportGenerator.ps1

$ErrorActionPreference = 'Stop'

if ((-not $SharpRacer) -and (-not $SourceGenerator))
{
    throw "At least one switch must be set: -SharpRacer, -SourceGenerator"
}

# Configure collection options
$script:outputRootDirectory = (Get-RelativeRepositoryPath -Path $OutputDirectory)

$script:codeCoverageOptions = New-CodeCoverageCollectionOptions `
    -CodeCoverageDirectory (Join-Path $script:outputRootDirectory "coverage") `
    -HtmlReportsDirectory (Join-Path $script:outputRootDirectory "html") `
    -JsonReportsDirectory (Join-Path $script:outputRootDirectory "json")

$script:defaultExcludedAssemblies = @("SharpRacer.Testing")
$script:defaultExcludedClasses = @("Windows.Win32.*", "System.*")
$script:sourceGenExcludedAssemblies = $script:defaultExcludedAssemblies + @("SharpRacer")

# Set up test projects
$script:sharpRacerTestProjects = @()
$script:sourceGeneratorTestProjects = @()

$script:sharpRacerTestProjects += New-CodeCoverageTestProject `
        -ProjectFileName (Get-RelativeRepositoryPath -Path ".\src\libraries\tests\SharpRacer.UnitTests\SharpRacer.UnitTests.csproj") `
        -ProjectType UnitTest `
        -Options $script:codeCoverageOptions `
        -TargetPlatform Any

$script:sharpRacerTestProjects += New-CodeCoverageTestProject `
        -ProjectFileName (Get-RelativeRepositoryPath -Path ".\src\libraries\tests\SharpRacer.UnitTests.Windows\SharpRacer.UnitTests.Windows.csproj") `
        -ProjectType UnitTest `
        -Options $script:codeCoverageOptions `
        -TargetPlatform Windows

$script:sharpRacerTestProjects += New-CodeCoverageTestProject `
        -ProjectFileName (Get-RelativeRepositoryPath -Path ".\src\libraries\tests\SharpRacer.IntegrationTests\SharpRacer.IntegrationTests.csproj") `
        -ProjectType IntegrationTest `
        -Options $script:codeCoverageOptions `
        -TargetPlatform Any

$script:sourceGeneratorTestProjects += New-CodeCoverageTestProject `
        -ProjectFileName (Get-RelativeRepositoryPath -Path ".\src\generators\tests\SharpRacer.SourceGenerators.UnitTests\SharpRacer.SourceGenerators.UnitTests.csproj") `
        -ProjectType UnitTest `
        -Options $script:codeCoverageOptions `
        -TargetPlatform Any

$script:sourceGeneratorTestProjects += New-CodeCoverageTestProject `
        -ProjectFileName (Get-RelativeRepositoryPath -Path ".\src\generators\tests\SharpRacer.SourceGenerators.IntegrationTests\SharpRacer.SourceGenerators.IntegrationTests.csproj") `
        -ProjectType IntegrationTest `
        -Options $script:codeCoverageOptions `
        -TargetPlatform Any

# Build projects
if (-not $NoBuild)
{
    Invoke-DotNetBuild -Project (Get-RepositoryDirectoryPath) -Configuration $Configuration
}

# Collect code coverage files
[string[]]$script:sharpRacerCodeCoverageFiles = @()
[string[]]$script:sourceGeneratorCodeCoverageFiles = @()

if ($SharpRacer)
{
    try
    {
        $script:sharpRacerCodeCoverageFiles = Build-CodeCoverageResults `
            -Projects $script:sharpRacerTestProjects `
            -IncludeUnitTests `
            -IncludeIntegrationTests:(-not $ExcludeIntegrationTests) `
            -Configuration $Configuration `
            -NoBuild
    }
    catch
    {
        Write-Output "Collection failed. Terminating."
        throw
    }
}

if ($SourceGenerator)
{
    try
    {
        $script:sourceGeneratorCodeCoverageFiles = Build-CodeCoverageResults `
            -Projects $script:sourceGeneratorTestProjects `
            -IncludeUnitTests `
            -IncludeIntegrationTests:(-not $ExcludeIntegrationTests) `
            -Configuration $Configuration `
            -NoBuild
    }
    catch
    {
        Write-Output "Collection failed. Terminating."
        throw
    }
}

# Generate report(s)
[bool]$script:generateCombinedReport = ($SharpRacer -and $SourceGenerator)

if ($Json)
{
    Clear-ReportOutputDirectory $script:codeCoverageOptions.JsonReportsDirectory

    if ($SharpRacer)
    {
        New-JsonCodeCoverageReport -FileName (Join-Path $script:codeCoverageOptions.JsonReportsDirectory "SharpRacer.json") `
            -CodeCoverageFileNames $script:sharpRacerCodeCoverageFiles `
            -ExcludedAssemblies $script:defaultExcludedAssemblies `
            -ExcludedClasses $script:defaultExcludedClasses `
            -ReportTitle "SharpRacer Library"
    }

    if ($SourceGenerator)
    {
        New-JsonCodeCoverageReport -FileName (Join-Path $script:codeCoverageOptions.JsonReportsDirectory "SharpRacer.SourceGenerators.json") `
            -CodeCoverageFileNames $script:sourceGeneratorCodeCoverageFiles `
            -ExcludedAssemblies $script:sourceGenExcludedAssemblies `
            -ExcludedClasses $script:defaultExcludedClasses `
            -ReportTitle "SharpRacer.SourceGenerators"
    }

    if ($script:generateCombinedReport)
    {
        New-JsonCodeCoverageReport -FileName (Join-Path $script:codeCoverageOptions.JsonReportsDirectory "SharpRacer.all.json") `
            -CodeCoverageFileNames @($script:sharpRacerCodeCoverageFiles + $script:sourceGeneratorCodeCoverageFiles) `
            -ExcludedAssemblies $script:defaultExcludedAssemblies `
            -ExcludedClasses $script:defaultExcludedClasses `
            -ReportTitle "SharpRacer"
    }
}

if ($Html)
{
    Clear-ReportOutputDirectory $script:codeCoverageOptions.HtmlReportsDirectory

    if ($SharpRacer)
    {
        New-HtmlCodeCoverageReport -OutputDirectory (Join-Path $script:codeCoverageOptions.HtmlReportsDirectory "SharpRacer") `
            -CodeCoverageFileNames $script:sharpRacerCodeCoverageFiles `
            -ExcludedAssemblies $script:defaultExcludedAssemblies `
            -ExcludedClasses $script:defaultExcludedClasses `
            -ReportTitle "SharpRacer Library"
    }

    if ($SourceGenerator)
    {
        New-HtmlCodeCoverageReport -OutputDirectory (Join-Path $script:codeCoverageOptions.HtmlReportsDirectory "SharpRacer.SourceGenerators") `
            -CodeCoverageFileNames $script:sourceGeneratorCodeCoverageFiles `
            -ExcludedAssemblies $script:sourceGenExcludedAssemblies `
            -ExcludedClasses $script:defaultExcludedClasses `
            -ReportTitle "SharpRacer.SourceGenerators"
    }

    if ($script:generateCombinedReport)
    {
        New-HtmlCodeCoverageReport -OutputDirectory (Join-Path $script:codeCoverageOptions.HtmlReportsDirectory "SharpRacer.all") `
            -CodeCoverageFileNames @($script:sharpRacerCodeCoverageFiles + $script:sourceGeneratorCodeCoverageFiles) `
            -ExcludedAssemblies $script:defaultExcludedAssemblies `
            -ExcludedClasses $script:defaultExcludedClasses `
            -ReportTitle "SharpRacer"
    }
}