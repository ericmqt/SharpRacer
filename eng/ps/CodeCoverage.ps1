. $PSScriptRoot\RepositoryUtilities.ps1
. $PSScriptRoot\DotNet.ps1

enum CodeCoverageTestProjectType
{
    UnitTest = 0
    IntegrationTest = 1
}

enum CodeCoverageTestProjectPlatform
{
    Any = 0
    Windows = 1
}

class CodeCoverageTestProject
{
    CodeCoverageTestProject([string]$Name, [string]$ProjectFileName, [CodeCoverageTestProjectType]$ProjectType, [CodeCoverageTestProjectPlatform]$TargetPlatform, [string]$CodeCoverageFileName)
    {
        $this.Name = $Name
        $this.ProjectFileName = $ProjectFileName
        $this.CodeCoverageFileName = $CodeCoverageFileName
        $this.ProjectType = $ProjectType
        $this.TargetPlatform = $TargetPlatform
    }

    [ValidateNotNullOrEmpty()]
    [string]$CodeCoverageFileName

    [ValidateNotNullOrEmpty()]
    [string]$Name

    [ValidateNotNullOrEmpty()]
    [string]$ProjectFileName

    [CodeCoverageTestProjectType]$ProjectType

    [CodeCoverageTestProjectPlatform]$TargetPlatform
}

class CodeCoverageCollectionOptions
{
    CodeCoverageCollectionOptions([string]$CodeCoverageDirectory, [string]$HtmlReportsDirectory, [string]$JsonReportsDirectory)
    {
        $this.CodeCoverageDirectory = $CodeCoverageDirectory
        $this.HtmlReportsDirectory = $HtmlReportsDirectory
        $this.JsonReportsDirectory = $JsonReportsDirectory
    }

    [ValidateNotNullOrEmpty()]
    [string]$CodeCoverageDirectory

    [ValidateNotNullOrEmpty()]
    [string]$HtmlReportsDirectory

    [ValidateNotNullOrEmpty()]
    [string]$JsonReportsDirectory
}

function New-CodeCoverageCollectionOptions
{
    [CmdletBinding()]
    Param (
        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [string]$CodeCoverageDirectory,

        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [string]$HtmlReportsDirectory,

        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [string]$JsonReportsDirectory
    )
    
    return [CodeCoverageCollectionOptions]::new($CodeCoverageDirectory, $HtmlReportsDirectory, $JsonReportsDirectory)
}

function New-CodeCoverageTestProject
{
    [CmdletBinding()]
    Param (
        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [string]$ProjectFileName,

        [Parameter(Mandatory=$true)]
        [CodeCoverageTestProjectType]$ProjectType,

        [Parameter(Mandatory=$true)]
        [ValidateNotNull()]
        [CodeCoverageCollectionOptions]$Options,

        [Parameter(Mandatory=$false)]
        [CodeCoverageTestProjectPlatform]$TargetPlatform
    )

    if (-not (Test-Path -Path $ProjectFileName -PathType Leaf)) {
        throw "Project does not exist or is not accessible: $ProjectFileName"
    }

    $name = [System.IO.Path]::GetFileNameWithoutExtension($ProjectFileName)
    $codeCoverageFile = Join-Path (Get-RelativeRepositoryPath -Path $Options.CodeCoverageDirectory) "$($name).xml"

    return [CodeCoverageTestProject]::new($name, $ProjectFileName, $ProjectType, $TargetPlatform, $codeCoverageFile)
}

function Get-CodeCoverageTestProjectPlatforms
{
    $local:targetPlatforms = @( ([CodeCoverageTestProjectPlatform]::Any) )

    if ($IsWindows -eq $true)
    {
        $local:targetPlatforms += @( ([CodeCoverageTestProjectPlatform]::Windows) )
    }

    return @($local:targetPlatforms)
}

function Get-CodeCoverageTestProjectTypes
{
    [CmdletBinding()]
    Param (
        [Parameter(Mandatory=$false)]
        [switch]$IncludeUnitTests,

        [Parameter(Mandatory=$false)]
        [switch]$IncludeIntegrationTests
    )

    [CodeCoverageTestProjectType[]]$local:testProjectTypes = @()
    
    if ($IncludeUnitTests)
    {
        $local:testProjectTypes += [CodeCoverageTestProjectType]::UnitTest
    }

    if ($IncludeIntegrationTests)
    {
        $local:testProjectTypes += [CodeCoverageTestProjectType]::IntegrationTest
    }

    return $local:testProjectTypes
}

function Select-CodeCoverageTestProject
{
    [CmdletBinding()]
    Param (
        [Parameter(Mandatory=$true, Position = 0, ValueFromPipeline = $true)]
        [CodeCoverageTestProject]$Project,

        [Parameter(Mandatory=$false)]
        [switch]$IncludeUnitTests,

        [Parameter(Mandatory=$false)]
        [switch]$IncludeIntegrationTests
    )

    process
    {
        $local:testProjectTypes = Get-CodeCoverageTestProjectTypes `
            -IncludeUnitTests:$IncludeUnitTests `
            -IncludeIntegrationTests:$IncludeIntegrationTests

        if (($Project.TargetPlatform -in (Get-CodeCoverageTestProjectPlatforms)) `
            -and ($Project.ProjectType -in $local:testProjectTypes ))
        {
            return $Project
        }
        else
        {
            return $null
        }
    }
}

function New-CodeCoverageTempOutputDirectory
{
    [CmdletBinding()]
    Param (
        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [string]$ParentDirectory
    )

    $pathStr = Join-Path -Path $ParentDirectory -ChildPath ([Guid]::NewGuid().ToString())

    if (Test-Path -Path $pathStr)
    {
        Remove-Item -Path $pathStr -Recurse -Force
    }

    $dir = New-Item -Path $pathStr -ItemType Directory

    return $dir.FullName
}

function Measure-CodeCoverage
{
    [CmdletBinding()]
    Param (
        [Parameter(Mandatory=$true, ValueFromPipeline=$true)]
        [ValidateNotNull()]
        [CodeCoverageTestProject]$Project,

        [Parameter(Mandatory=$true)]
        [switch]$NoBuild
    )

    $local:parentDir = [System.IO.Path]::GetDirectoryName($Project.CodeCoverageFileName)

    # Ensure target directory exists
    if (-not (Test-Path -Path $local:parentDir -PathType Container))
    {
        New-Item -Path $local:parentDir -ItemType Directory
    }

    $local:tempDir = (New-CodeCoverageTempOutputDirectory -ParentDirectory $local:parentDir)

    try 
    {
        # Delete existing file, if it exists
        if (Test-Path -Path $Project.CodeCoverageFileName)
        {
            Write-Output "Removing existing code coverage output: $($Project.CodeCoverageFileName)"
            Remove-Item -Path $Project.CodeCoverageFileName -Force
        }

        # Build the 'dotnet test' command
        $dotnetTestExpr = "dotnet test $($Project.ProjectFileName) --collect:`"XPlat Code Coverage`" --results-directory `"$local:tempDir`""

        if ($NoBuild)
        {
            $dotnetTestExpr += " --no-build"
        }
        
        Invoke-Expression $dotnetTestExpr
        $local:dotnetExitCode = $LASTEXITCODE

        # Copy XML file out of the VSTest randomly-generated directory that can't be disabled for some god-forsaken reason
        # Try copying even if 'dotnet test' failed in case an output was produced, i.e. build error due to failing test
        Get-ChildItem -Path $local:tempDir -Filter "coverage.cobertura.xml" -Recurse | ForEach-Object {
            Copy-Item -Path $_.FullName -Destination $Project.CodeCoverageFileName -Force
        }

        # Delete the temporary directory now that we've copied the output file
        Remove-Item -Path $local:tempDir -Recurse -Force -ErrorAction SilentlyContinue

        # Throw for non-zero exit code, otherwise throw generally
        if ($local:dotnetExitCode -ne 0)
        {
            $local:ex = [DotNetCommandException]::new($Command, $LASTEXITCODE)
        
            $PSCmdlet.ThrowTerminatingError($local:ex.ToErrorRecord())
        }
        elseif (-not (Test-Path -Path $Project.CodeCoverageFileName))
        {
            throw "Code coverage file not generated: $($Project.CodeCoverageFileName)"
        }
        else
        {
            Write-Output "Code coverage file generated successfully: $($Project.CodeCoverageFileName)"
        }
    }
    catch
    {
        Remove-Item -Path $local:tempDir -Recurse -Force -ErrorAction SilentlyContinue

        throw
    }
}

function Build-CodeCoverageResults
{
    [CmdletBinding()]
    Param (
        [Parameter(Mandatory=$true)]
        [ValidateNotNull()]
        [CodeCoverageTestProject[]]$Projects,

        [Parameter(Mandatory=$false)]
        [switch]$IncludeUnitTests,

        [Parameter(Mandatory=$false)]
        [switch]$IncludeIntegrationTests,

        [Parameter(Mandatory=$true)]
        [switch]$NoBuild
    )

    if ($Projects.Length -eq 0)
    {
        throw "No projects specified."
    }

    $local:codeCoverageFiles = [System.Collections.Generic.List[string]]::new()

    $local:targetProjects = $Projects `
        | Select-CodeCoverageTestProject -IncludeUnitTests:$IncludeUnitTests -IncludeIntegrationTests:$IncludeIntegrationTests
    
    foreach ($project in $local:targetProjects)
    {
        try
        {
            $null = Measure-CodeCoverage -Project $project -NoBuild

            $local:codeCoverageFiles.Add($project.CodeCoverageFileName)
        }
        catch
        {
            Write-Output "Code coverage collection failed for project '$($project.Name)'. Terminating."
            throw
        }
    }

    return $local:codeCoverageFiles
}