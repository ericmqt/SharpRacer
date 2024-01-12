class DotNetCommandException : System.Exception
{
    DotNetCommandException([string]$Command, [int]$ExitCode)
        : base("dotnet $Command returned exit code $ExitCode")
    {
        $this.Command = $Command
        $this.ExitCode = $ExitCode
    }

    DotNetCommandException([string]$Message, [string]$Command, [int]$ExitCode)
        : base($Message)
    {
        $this.Command = $Command
        $this.ExitCode = $ExitCode
    }

    DotNetCommandException([string]$Message, [string]$Command, [int]$ExitCode, [System.Exception]$InnerException)
        : base($Message, $InnerException)
    {
        $this.Command = $Command
        $this.ExitCode = $ExitCode
    }

    [string]$Command
    [int]$ExitCode

    [System.Management.Automation.ErrorRecord] ToErrorRecord()
    {
        return $this.ToErrorRecord('DotNetCommandException')
    }

    [System.Management.Automation.ErrorRecord] ToErrorRecord([string]$ErrorId)
    {
        return [System.Management.Automation.ErrorRecord]::new(
            $this,
            $ErrorId,
            [System.Management.Automation.ErrorCategory]::InvalidResult,
            $null)
    }
}

class DotNetTestCommandException : DotNetCommandException
{
    DotNetTestCommandException(
        [string]$Message,
        [string]$Project,
        [string[]]$AdditionalArguments,
        [int]$ExitCode)
        : base($Message, 'test', $ExitCode)
    {
        $this.Project = $Project
        $this.AdditionalArguments = $AdditionalArguments
    }

    DotNetTestCommandException(
        [string]$Message,
        [string]$Project,
        [string[]]$AdditionalArguments,
        [int]$ExitCode,
        [System.Exception]$InnerException)
        : base($Message, 'test', $ExitCode, $InnerException)
    {
        $this.Project = $Project
        $this.AdditionalArguments = $AdditionalArguments
    }

    [string[]]$AdditionalArguments
    [string]$Project
}

function Invoke-DotNet
{
    [CmdletBinding()]
    Param (
        [Parameter(Mandatory = $true)]
        [System.String]
        $Command,

        [Parameter(Mandatory = $false)]
        [string[]]
        $ArgumentList
    )

    $local:dotnetArgumentList = @( $Command ) + $ArgumentList
    
    & dotnet $local:dotnetArgumentList

    if ($? -eq $false)
    {
        $local:ex = [DotNetCommandException]::new($Command, $LASTEXITCODE)
            
        $PSCmdlet.ThrowTerminatingError($local:ex.ToErrorRecord())
    }
}

function Invoke-DotNetTest
{
    [CmdletBinding()]
    Param (
        [Parameter(Mandatory = $true, Position = 0)]
        [string]
        $Project,
        [Parameter(Mandatory = $false, Position = 1)]
        [string[]]
        $AdditionalArguments
    )

    $local:args = @($Project) + $AdditionalArguments

    try
    {
        Invoke-DotNet -Command "test" -ArgumentList $local:args
    }
    catch [DotNetCommandException]
    {
        $local:projectFileName = [System.IO.Path]::GetFileNameWithoutExtension($Project)

        $local:testEx = [DotNetTestCommandException]::new(
            "[$local:projectFileName] dotnet test failed (exit code $($PSItem.Exception.ExitCode))",
            $Project,
            $AdditionalArguments,
            $PSItem.Exception.ExitCode,
            $PSItem.Exception)
        
        $PSCmdlet.ThrowTerminatingError($local:testEx.ToErrorRecord('DotNetTestCommandException'))
    }
}

function New-TestCoverageReport
{
    [CmdletBinding()]
    Param (
        [Parameter(Mandatory=$true)]
        [string[]]$CoverageFiles,

        [Parameter(Mandatory=$true)]
        [string]$OutputDirectory,

        [Parameter(Mandatory=$false)]
        [string[]]$ExcludedAssemblies,

        [Parameter(Mandatory=$false)]
        [string]$ReportTitle,

        [Parameter(Mandatory=$false)]
        [string]$ReportFormat = "HtmlInline_AzurePipelines"
    )

    $local:coverageFilesArg = $CoverageFiles -join ";"
    $local:args = @("`"-reports:$local:coverageFilesArg`"", "-targetdir:$OutputDirectory", "-reporttypes:$ReportFormat")

    if (![string]::IsNullOrEmpty($ReportTitle))
    {
        $local:args += "`"-title:$ReportTitle`""
    }

    if ($ExcludedAssemblies.Length -gt 0)
    {
        $local:excludedAssemblyFilters = "-{0}" -f $ExcludedAssemblies
        $local:assemblyFiltersArg = $local:excludedAssemblyFilters -join ";"
        $local:args += "`"-assemblyfilters:$local:assemblyFiltersArg`""
    }

    try
    {
        Invoke-DotNet -Command 'reportgenerator' -ArgumentList $local:args
    }
    catch [DotNetCommandException]
    {
        $PSCmdlet.ThrowTerminatingError($PSItem)
    }
}