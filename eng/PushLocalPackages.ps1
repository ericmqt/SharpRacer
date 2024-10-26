[CmdletBinding()]
Param(
    [Parameter(Mandatory=$true)]
    [string]$FeedPath
)

. $PSScriptRoot\ps\RepositoryUtilities.ps1
. $PSScriptRoot\ps\DotNet.ps1

$ErrorActionPreference = 'Stop'

$script:NuGetTempOutputPath = Get-RelativeRepositoryPath -Path "artifacts\nupkg\pushtemp"

# Clean temporary output directory if it exists
if (Test-Path -Path $script:NuGetTempOutputPath)
{
    Remove-Item -Path $script:NuGetTempOutputPath -Recurse
}

# Pack the projects
Invoke-DotNet -Command 'pack' @('-o', $script:NuGetTempOutputPath)

# Add each package to the local feed
foreach ($pkgFile in Get-ChildItem -Path $script:NuGetTempOutputPath *.nupkg) {
    & nuget add $pkgFile.FullName -source $FeedPath
}

# Delete temporary output directory
Remove-Item -Path $script:NuGetTempOutputPath -Recurse