#Requires -Version 5.0

[CmdletBinding()]
Param (
  [Parameter(Mandatory)]
  [ValidateNotNullOrEmpty()]
  [ValidateSet("Local", "AppVeyor")]
  [string] $Mode,
  [Parameter(Mandatory)]
  [Version] $Version,
  [Parameter(Mandatory)]
  [bool] $IsPreRelease,
  [Parameter()]
  [string] $Configuration = "Release",
  [Parameter()]
  [bool] $RunTests = $True,
  [Parameter()]
  [bool] $RunDotCoverCoverageAnalysis = $True,
  [Parameter()]
  [bool] $RunReSharperCodeInspection = $True,
  [Parameter()]
  [bool] $CreateNuGetPackages = $False,
  [Parameter()]
  [bool]$PushNuGetPackages = $False,
  [Parameter()]
  [string] $TargetNuGetFeed = "https://www.nuget.org",
  [Parameter()]
  [string] $NuGetApiKey,
  [Parameter()] 
  [bool] $CreateArchives = $False
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = "Stop"
$ConfirmPreference = "None"
trap { $error[0] | Format-List -Force; $host.SetShouldExit(1) }

. $PSScriptRoot\Shared\Build\BuildLibrary.ps1

$TreatWarningsAsErrors = $True
$MSBuildToolset = "15.0"
$MSBuildExecutable = "C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\$MSBuildToolset\Bin\MSBuild.exe"
$NuGetExecutable = ".\.nuget\NuGet.exe"

$SolutionDirectory = $PSScriptRoot
$SolutionFile = Join-Path $SolutionDirectory "SolutionInspector.sln"
$DotSettingsFile = "${SolutionFile}.DotSettings"

$AssemblyInfoSharedFile = Join-Path $SolutionDirectory "AssemblyInfoShared.cs"

$BuildOutputDirectory = Join-Path $SolutionDirectory "Build"
$AnalysisResultsDirectory = Join-Path $BuildOutputDirectory "AnalysisResults"
$DotCoverResultsDirectory = Join-Path $AnalysisResultsDirectory "dotCover"
$NuGetPackagesDirectory = Join-Path $BuildOutputDirectory "NuGetPackages"

$NUnitResultsFile = Join-Path $BuildOutputDirectory "NUnitResults.xml"

$DotCoverCoverageSnapshotFile = Join-Path $DotCoverResultsDirectory "coverageSnapshot.dcvr"
$DotCoverCoverageBadgeFile = Join-Path $DotCoverResultsDirectory "coverageBadge.svg"
$DotCoverCoverageReportFile = Join-Path $DotCoverResultsDirectory "coverageReport.html"

$Projects = Get-ProjectsFromSolution $SolutionFile $Configuration | ? { $_.ProjectName -ne "SolutionPackages" }

$TestProjects = $Projects | ? { $_.ProjectName.EndsWith("Tests") }
$TestAssemblies = $TestProjects | % { $_.TargetPath }

$VersionPrefix = "$($Version.Major).$($Version.Minor).$($Version.Build)"
$VersionSuffix = $null
if ($IsPreRelease) {
  $VersionSuffix = "pre$($Version.Revision)"
}
$VersionString = "${VersionPrefix}-${VersionSuffix}"

$NuGetVersion = $Version.Major, $Version.Minor, $Version.Build

$BranchName = Get-CurrentBranchName
$CommitHash = Get-CurrentCommitHash

function Run() {
  Write-Host "Running in '$Mode' mode"
  Write-Host "Building '$SolutionFile'"
  Write-Host "Version: $VersionString"
  Write-Host "Configuration: $Configuration"
  Write-Host "Run tests: $RunTests (with DotCover coverage analysis: $RunDotCoverCoverageAnalysis)"
  Write-Host "Run ReSharper code inspection: $RunReSharperCodeInspection"
  Write-Host "Build NuGetPackages: $CreateNuGetPackages (with version: $VersionString)"
  Write-Host "Push NuGet packages: $PushNuGetPackages (to $TargetNuGetFeed)"
  Write-Host "Create archives: $CreateArchives"

  Clean
  Restore-NuGetPackages
  Build
  Run-ReSharperCodeInspection -Condition $RunReSharperCodeInspection
  Run-Tests -Condition $RunTests
  Create-NuGetPackages -Condition $CreateNuGetPackages
  Push-Packages -Condition ($PushNuGetPackages -And $CreateNuGetPackages)
  Create-Archives -Condition $CreateArchives
  
  Publish-CoverageReports -Condition ($Mode -eq "AppVeyor" -and -not $IsPreRelease -and $RunDotCoverCoverageAnalysis -and $PushNuGetPackages)
}

BuildTask Clean {
  Clean-BuildDirectory $BuildOutputDirectory
  Clean-Solution $SolutionFile
  New-Item $AnalysisResultsDirectory, $DotCoverResultsDirectory, $NuGetPackagesDirectory -Type Directory | Out-Null
}

BuildTask Restore-NuGetPackages {
  Restore-NuGetPackagesForSolution $SolutionFile
}

BuildTask Build {
  Build-Solution `
    -SolutionFile $SolutionFile `
    -Project $Projects `
    -Configuration $Configuration `
    -TreatWarningsAsErrors $TreatWarningsAsErrors `
    -VersionPrefix $VersionPrefix -VersionSuffix $VersionSuffix
}

BuildTask Run-ReSharperCodeInspection {
  $ReSharperCodeInspectionResultsFile = Join-Path $AnalysisResultsDirectory "ReSharperCodeInspectionResults.xml"
  $numberOfIssues = Execute-ReSharperCodeInspection $SolutionFile $Configuration $ReSharperCodeInspectionResultsFile $BranchName
}

BuildTask Run-Tests {
  if ($RunDotCoverCoverageAnalysis) {
    Execute-NUnitTests -WithDotCover -TestAssemblies $TestAssemblies -ResultsFile $NUnitResultsFile -DotSettingsFile $DotSettingsFile -DotCoverResultsFile $DotCoverCoverageSnapshotFile

    Create-DotCoverCoverageBadge $DotCoverCoverageSnapshotFile $DotCoverCoverageBadgeFile
    Create-DotCoverCoverageReport $DotCoverCoverageSnapshotFile $DotCoverCoverageReportFile
  }
  else {
    Execute-NUnitTests -TestAssemblies $TestAssemblies -ResultsFile $NUnitResultsFile 
  }
}

BuildTask Create-NuGetPackages {
  $vcsUrlTemplate = "https://raw.githubusercontent.com/chrischu/SolutionInspector/${CommitHash}/${BranchName}/{0}"
  Create-NuGetPackagesFromSolution `
    -SolutionDirectory $SolutionDirectory `
    -Projects $Projects `
    -Configuration $Configuration `
    -Version $VersionPrefix -VersionSuffix $VersionSuffix `
    -VcsUrlTemplate $vcsUrlTemplate `
    -ResultsDirectory $NuGetPackagesDirectory

  Get-ChildItem $NuGetPackagesDirectory *.nupkg | %{ Report-NuGetPackage $_.FullName }
}

BuildTask Push-Packages {
   Push-AllNuGetPackages $NuGetPackagesDirectory $TargetNuGetFeed $NuGetApiKey
}

BuildTask Create-Archives {
  $archiveName = "SolutionInspector-${VersionString}.zip"
  $archivePath = Join-Path $BuildOutputDirectory $archiveName

  Write-Host "Creating archive '$archiveName'"

  $buildDirectory = Join-Path $SolutionDirectory "SolutionInspector\bin\$Configuration"
  $sourceDirectory = Join-Path ([System.IO.Path]::GetTempPath()) ([System.Guid]::NewGuid())
  New-Item -ItemType Directory -Path $sourceDirectory | Out-Null
  Copy-Item -Path "${buildDirectory}\*" -Destination $sourceDirectory -Exclude "*CodeAnalysisLog.xml","*.lastcodeanalysissucceeded"
  Zip-Directory -ZipFilePath $archivePath -SourceDirectory $sourceDirectory
  Remove-Item $sourceDirectory -Recurse

  Report-Archive $archivePath "SolutionInspector.zip"
}

BuildTask Publish-CoverageReports {
  Publish-CoverageReport $VersionString $DotCoverCoverageBadgeFile $DotCoverCoverageReportFile
}

return Run