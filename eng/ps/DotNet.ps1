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

class DotNetBuildCommandException : DotNetCommandException
{
    DotNetBuildCommandException(
        [string]$Message,
        [string]$Project,
        [string[]]$AdditionalArguments,
        [int]$ExitCode)
        : base($Message, 'test', $ExitCode)
    {
        $this.Project = $Project
        $this.AdditionalArguments = $AdditionalArguments
    }

    DotNetBuildCommandException(
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

function Invoke-DotNetBuild
{
    [CmdletBinding()]
    Param (
        [Parameter(Mandatory = $true, Position = 0)]
        [ValidateNotNullOrWhiteSpace()]
        [string]$Project,
        [Parameter(Mandatory=$false, Position = 1)]
        [ValidateNotNullOrWhiteSpace()]
        [string]$Configuration = "Debug",
        [Parameter(Mandatory = $false, Position = 2)]
        [string[]]$AdditionalArguments
    )

    $local:args = @($Project) + @("--configuration", $Configuration) + $AdditionalArguments

    try
    {
        Invoke-DotNet -Command "build" -ArgumentList $local:args
    }
    catch [DotNetCommandException]
    {
        $local:projectFileName = [System.IO.Path]::GetFileNameWithoutExtension($Project)

        $local:testEx = [DotNetBuildCommandException]::new(
            "[$local:projectFileName] dotnet build failed (exit code $($PSItem.Exception.ExitCode))",
            $Project,
            $AdditionalArguments,
            $PSItem.Exception.ExitCode,
            $PSItem.Exception)
        
        $PSCmdlet.ThrowTerminatingError($local:testEx.ToErrorRecord('DotNetBuildCommandException'))
    }
}

function Invoke-DotNetTest
{
    [CmdletBinding()]
    Param (
        [Parameter(Mandatory = $true, Position = 0)]
        [string] $Project,
        [Parameter(Mandatory=$false, Position = 1)]
        [ValidateNotNullOrWhiteSpace()]
        [string]$Configuration = "Debug",
        [Parameter(Mandatory = $false, Position = 2)]
        [string[]]
        $AdditionalArguments
    )

    $local:args = @($Project) + $AdditionalArguments

    try
    {
        Invoke-DotNet -Command "test" -Configuration $Configuration -ArgumentList $local:args
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