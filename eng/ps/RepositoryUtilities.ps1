function Get-RepositoryDirectory
{
    [CmdletBinding()]
    [OutputType([System.IO.DirectoryInfo])]
    param()

    $local:dir = $PSScriptRoot
    Write-Output "dir = $local:dir"
    $local:repositoryRootFile = ".gitattributes"

    while($null -ne $local:dir)
    {
        $local:repoRootFilePath = Join-Path $local:dir $local:repositoryRootFile

        if (Test-Path -Path $local:repoRootFilePath)
        {
            return $local:dir
        }

        $local:dir = (Get-Item $local:dir).Parent
    }
    
    return $local:dir
}

function Get-RepositoryDirectoryPath
{
    [CmdletBinding()]
    [OutputType([string])]
    param()

    return (Get-RepositoryDirectory).FullName
}

function Get-RelativeRepositoryPath
{
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true)]
        [string]$Path
    )

    $local:fullPath = Join-Path (Get-RepositoryDirectoryPath) $Path

    return [System.IO.Path]::GetRelativePath((Get-Location), $local:fullPath)
}