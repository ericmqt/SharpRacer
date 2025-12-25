[CmdletBinding()]
Param(
    [Parameter(Mandatory=$true)]
    [ValidateNotNullOrEmpty()]
    [string]$SourceBranch,

    [Parameter(Mandatory=$true)]
    [ValidateNotNullOrEmpty()]
    [string]$CodeCoverageBranch,

    [Parameter(Mandatory=$true)]
    [ValidateNotNullOrEmpty()]
    [string]$ArtifactDirectory
)

$ErrorActionPreference = 'Stop'

# Configure git user
git config --global user.name 'github-actions[bot]'
git config --global user.email '41898282+github-actions[bot]@users.noreply.github.com'

# Fetch remote branches and check if the badges branch exists
git fetch
git ls-remote --heads origin $CodeCoverageBranch

if ($?)
{
    Write-Output "Checking out code coverage branch: $CodeCoverageBranch"
    git checkout $CodeCoverageBranch
}
else
{
    Write-Output "Code coverage branch does not exist: $CodeCoverageBranch"

    git switch --orphan $CodeCoverageBranch

    Write-Output "Created code coverage branch: $CodeCoverageBranch"
}

# Create directories if they don't already exist
$script:outputDir = $SourceBranch
$script:codeCoverageDir = Join-Path $script:outputDir "coverage"
$script:jsonDir = Join-Path $script:outputDir "json"

if (-not (Test-Path -Path $script:outputDir -PathType Container))
{
    New-Item -Path $script:outputDir -ItemType Directory
}

if (-not (Test-Path -Path $script:codeCoverageDir -PathType Container))
{
    New-Item -Path $script:codeCoverageDir -ItemType Directory
}

if (-not (Test-Path -Path $script:jsonDir -PathType Container))
{
    New-Item -Path $script:jsonDir -ItemType Directory
}

# Copy code coverage files into their respective directories
Copy-Item -Path $ArtifactDirectory\*.xml -Destination $script:codeCoverageDir -Force
Copy-Item -Path $ArtifactDirectory\*.json -Destination $script:jsonDir -Force

git add $script:jsonDir/*.json
git add $script:codeCoverageDir/*.xml
git commit -m "Updated code coverage files for branch '$SourceBranch'"
git push origin $CodeCoverageBranch