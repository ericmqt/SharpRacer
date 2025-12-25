. $PSScriptRoot\RepositoryUtilities.ps1
. $PSScriptRoot\DotNet.ps1

function Clear-ReportOutputDirectory
{
    [CmdletBinding()]
    Param (
        [Parameter(Mandatory=$true, Position = 0)]
        [ValidateNotNullOrEmpty()]
        [string]$OutputDirectory
    )

    if (Test-Path -Path $OutputDirectory -PathType Container)
    {
        Remove-Item -Path "$OutputDirectory\*" -Recurse -Force | Out-Null
    }
    else
    {
        New-Item -Path $OutputDirectory -ItemType Directory | Out-Null
    }
}

function Format-ReportGeneratorArguments
{
    [CmdletBinding()]
    Param (
        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [string]$OutputDirectory,

        [Parameter(Mandatory=$true)]
        [ValidateNotNull()]
        [string[]]$CoverageFiles,

        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [string]$ReportFormat,

        [Parameter(Mandatory=$false)]
        [string[]]$ExcludedAssemblies,

        [Parameter(Mandatory=$false)]
        [string[]]$ExcludedClasses,

        [Parameter(Mandatory=$false)]
        [string]$ReportTitle
    )

    $local:coverageFilesArg = $CoverageFiles -join ";"
    $local:args = @("`"-reports:$local:coverageFilesArg`"", "-targetdir:$OutputDirectory", "-reporttypes:$ReportFormat")

    if (![string]::IsNullOrEmpty($ReportTitle))
    {
        $local:args += "`"-title:$ReportTitle`""
    }

    if ($ExcludedAssemblies.Length -gt 0)
    {
        $local:excludedAssemblyFilters = $ExcludedAssemblies | ForEach-Object { "-{0}" -f $_ }
        $local:assemblyFiltersArg = $local:excludedAssemblyFilters -join ";"
        $local:args += "`"-assemblyfilters:$local:assemblyFiltersArg`""
    }

    if ($ExcludedClasses.Length -gt 0)
    {
        $local:excludedClassesFilters = $ExcludedClasses | ForEach-Object { "-{0}" -f $_ }
        $local:classFiltersArg = $local:excludedClassesFilters -join ";"

        $local:args += "`"-classfilters:$local:classFiltersArg`""
    }

    return $local:args
}

function Invoke-ReportGenerator
{
    [CmdletBinding()]
    Param (
        [Parameter(Mandatory = $true, Position = 0)]
        [string[]]
        $ArgumentList
    )

    try
    {
        Invoke-DotNet -Command 'reportgenerator' -ArgumentList $ArgumentList
    }
    catch [DotNetCommandException]
    {
        $PSCmdlet.ThrowTerminatingError($PSItem)
    }
}

function New-HtmlCodeCoverageReport
{
    [CmdletBinding()]
    Param (
        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [string]$OutputDirectory,

        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [string[]]$CodeCoverageFileNames,

        [Parameter(Mandatory=$false)]
        [string[]]$ExcludedAssemblies,

        [Parameter(Mandatory=$false)]
        [string[]]$ExcludedClasses,

        [Parameter(Mandatory=$false)]
        [string]$ReportTitle
    )

    if ($CodeCoverageFileNames.Length -eq 0)
    {
        throw "No code coverage files specified!"
    }

    $local:reportGeneratorArgs = Format-ReportGeneratorArguments `
        -OutputDirectory $OutputDirectory `
        -CoverageFiles $CodeCoverageFileNames `
        -ExcludedAssemblies $ExcludedAssemblies `
        -ExcludedClasses $ExcludedClasses `
        -ReportTitle $ReportTitle `
        -ReportFormat "HtmlInline_AzurePipelines"

    try
    {
        Invoke-ReportGenerator $local:reportGeneratorArgs
    }
    catch [DotNetCommandException]
    {
        $PSCmdlet.ThrowTerminatingError($PSItem)
    }
}

function New-JsonCodeCoverageReport
{
    [CmdletBinding()]
    Param (
        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [string]$FileName,

        [Parameter(Mandatory=$true)]
        [string[]]$CodeCoverageFileNames,

        [Parameter(Mandatory=$false)]
        [string[]]$ExcludedAssemblies,

        [Parameter(Mandatory=$false)]
        [string[]]$ExcludedClasses,

        [Parameter(Mandatory=$false)]
        [string]$ReportTitle
    )

    if ($CodeCoverageFileNames.Length -eq 0)
    {
        throw "No code coverage files specified!"
    }

    $local:outputDir = [System.IO.Path]::GetDirectoryName($FileName)

    # We can't specify the Summary.json filename so we'll generate it in a temporary directory and copy
    # it to our output filename
    $local:tempDir = Join-Path -Path $local:outputDir -ChildPath ([Guid]::NewGuid().ToString())

    if (Test-Path -Path $local:tempDir)
    {
        Remove-Item -Path $local:tempDir -Recurse -Force
    }

    New-Item -Path $local:tempDir -ItemType Directory -Force | Out-Null

    try
    {
         $local:reportGeneratorArgs = Format-ReportGeneratorArguments `
            -OutputDirectory $local:tempDir `
            -CoverageFiles $CodeCoverageFileNames `
            -ExcludedAssemblies $ExcludedAssemblies `
            -ExcludedClasses $ExcludedClasses `
            -ReportTitle $ReportTitle `
            -ReportFormat "JsonSummary"

        Invoke-ReportGenerator $local:reportGeneratorArgs

        # Copy Summary.json to our desired output filename
        Get-ChildItem -Path $local:tempDir -Filter "Summary.json" -Recurse | ForEach-Object {
            Copy-Item -Path $_.FullName -Destination $FileName -Force
        }

        Remove-Item -Path $local:tempDir -Recurse -Force -ErrorAction SilentlyContinue

        if (-not (Test-Path -Path $FileName))
        {
            throw "JSON summary file was not found: $FileName"
        }
    }
    catch
    {
        Remove-Item -Path $local:tempDir -Recurse -Force -ErrorAction SilentlyContinue

        throw
    }
}