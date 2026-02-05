$scriptName = $MyInvocation.MyCommand.Name

Write-Host "Owner ${Env:REPO_OWNER}"
Write-Host "Repository ${Env:REPO}"

if ($Env:REPO_OWNER -ne "BpsLogicBuilder") {
    Write-Host "${scriptName}: Only create packages on BpsLogicBuilder repositories."
} else {
    dotnet pack --configuration Release -o ./nupkg --no-build
    dotnet nuget push ./nupkg/*.nupkg --skip-duplicate --api-key $Env:GITHUB_NUGET_AUTH_TOKEN
}