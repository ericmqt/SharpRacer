[CmdletBinding()]
Param(
    [Parameter(Mandatory=$false)]
    [switch]$GenerateReport
)

$script:RepositoryRoot = Split-Path $PSScriptRoot -Parent
$script:SolutionPath = Join-Path $script:RepositoryRoot "SharpRacer.sln"
$script:ArtifactsDir = Join-Path $script:RepositoryRoot "artifacts"

dotnet test $script:SolutionPath /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura

if ($GenerateReport)
{
    dotnet reportgenerator -reports:$script:ArtifactsDir\test\**\coverage.cobertura.xml -targetdir:$script:ArtifactsDir/test/reports -reporttypes:HtmlInline_AzurePipelines
}