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
  [bool] $RunFxCopCodeAnalysis = $True,
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
$MSBuildToolset = "14.0"
$MSBuildExecutable = "C:\Program Files (x86)\MSBuild\$MSBuildToolset\Bin\MSBuild.exe"
$NuGetExecutable = ".\.nuget\NuGet.exe"

$SolutionDirectory = $PSScriptRoot
$SolutionFile = Join-Path $SolutionDirectory "SolutionInspector.sln"
$DotSettingsFile = "${SolutionFile}.DotSettings"

$AssemblyInfoSharedFile = Join-Path $SolutionDirectory "AssemblyInfoShared.cs"

$BuildOutputDirectory = Join-Path $SolutionDirectory "Build"
$AnalysisResultsDirectory = Join-Path $BuildOutputDirectory "AnalysisResults"
$FxCopResultsDirectory = Join-Path $AnalysisResultsDirectory "FxCop"
$DotCoverResultsDirectory = Join-Path $AnalysisResultsDirectory "dotCover"
$NuGetPackagesDirectory = Join-Path $BuildOutputDirectory "NuGetPackages"

$DotCoverCoverageSnapshotFile = Join-Path $DotCoverResultsDirectory "coverageSnapshot.dcvr"
$DotCoverCoverageBadgeFile = Join-Path $DotCoverResultsDirectory "coverageBadge.svg"
$DotCoverCoverageReportFile = Join-Path $DotCoverResultsDirectory "coverageReport.html"

$Projects = Get-ProjectsFromSolution $SolutionFile $Configuration | ? { $_.ProjectName -ne "SolutionPackages" }

$TestProjects = $Projects | ? { $_.ProjectName.EndsWith("Tests") }
$TestAssemblies = $TestProjects | % { $_.TargetPath }

$AssemblyVersion = "$($Version.Major).0.0.0"
$AssemblyFileVersion = "$($Version.Major).$($Version.Minor).$($Version.Build).$($Version.Revision)"
$AssemblyInformationalVersion = "$($Version.Major).$($Version.Minor).$($Version.Build)"
if ($IsPreRelease) {
  $AssemblyInformationalVersion = "${AssemblyInformationalVersion}-pre$($Version.Revision)"
}

$NuGetVersion = $Version.Major, $Version.Minor, $Version.Build

function Run() {
  try {
    Write-Host "Running in '$Mode' mode"
    Write-Host "Building '$SolutionFile'"
    Write-Host "Version: $Version"
    Write-Host "Configuration: $Configuration"
    Write-Host "Run tests: $RunTests (with DotCover coverage analysis: $RunDotCoverCoverageAnalysis)"
    Write-Host "Run FxCop code analysis: $RunFxCopCodeAnalysis"
    Write-Host "Run ReSharper code inspection: $RunReSharperCodeInspection"
    Write-Host "Build NuGetPackages: $CreateNuGetPackages (with version: $AssemblyInformationalVersion)"
    Write-Host "Push NuGet packages: $PushNuGetPackages (to $TargetNuGetFeed)"
    Write-Host "Create archives: $CreateArchives"

    Clean
    Restore-NuGetPackages
    Update-AssemblyInfos
    Update-SolutionInspectorConfig
    Build
    Run-ReSharperCodeInspection -Condition $RunReSharperCodeInspection
    Run-Tests -Condition $RunTests
    Create-NuGetPackages -Condition $CreateNuGetPackages
    Push-Packages -Condition ($PushNuGetPackages -And $CreateNuGetPackages)
    Create-Archives -Condition $CreateArchives
  } finally {
    Restore-AssemblyInfos
  }
  
  Publish-CoverageReports -Condition ($Mode -eq "AppVeyor" -and -not $IsPreRelease -and $RunDotCoverCoverageAnalysis)
}

BuildTask Clean {
  Clean-BuildDirectory $BuildOutputDirectory
  Clean-Solution $SolutionFile
  New-Item $AnalysisResultsDirectory, $FxCopResultsDirectory, $DotCoverResultsDirectory, $NuGetPackagesDirectory -Type Directory | Out-Null
}

BuildTask Restore-NuGetPackages {
  Restore-NuGetPackagesForSolution $SolutionFile
}

BuildTask Update-AssemblyInfos { 
  Update-AssemblyInfo $AssemblyInfoSharedFile $Configuration $AssemblyVersion $AssemblyFileVersion $AssemblyInformationalVersion
}

BuildTask Restore-AssemblyInfos {
  Restore-AssemblyInfo $AssemblyInfoSharedFile
}

BuildTask Update-SolutionInspectorConfig {
  $solutionInspectorProject = $Projects | ?{ $_.ProjectName -eq "SolutionInspector" }

  Apply-XdtTransform $solutionInspectorProject "Template.SolutionInspectorConfig" "..\SolutionInspector.Api\App.config.uninstall.xdt"
  Apply-XdtTransform $solutionInspectorProject "Template.SolutionInspectorConfig" "..\SolutionInspector.Api\App.config.install.xdt"
  Apply-XdtTransform $solutionInspectorProject "Template.SolutionInspectorConfig" "..\SolutionInspector.DefaultRules\App.config.install.xdt"

  Apply-XdtTransform $solutionInspectorProject "App.config" "..\SolutionInspector.Api\App.config.uninstall.xdt"
  Apply-XdtTransform $solutionInspectorProject "App.config" "..\SolutionInspector.Api\App.config.install.xdt"
  Apply-XdtTransform $solutionInspectorProject "App.config" "..\SolutionInspector.DefaultRules\App.config.install.xdt"
}


BuildTask Build {
  Build-Solution $SolutionFile $Projects $Configuration $TreatWarningsAsErrors $RunFxCopCodeAnalysis $FxCopResultsDirectory
}

BuildTask Run-ReSharperCodeInspection {
  $ReSharperCodeInspectionResultsFile = Join-Path $AnalysisResultsDirectory "ReSharperCodeInspectionResults.xml"
  $numberOfIssues = Execute-ReSharperCodeInspection $SolutionFile $Configuration $ReSharperCodeInspectionResultsFile
}

BuildTask Run-Tests {
  if ($RunDotCoverCoverageAnalysis) {
    Execute-MSpecTests -WithDotCover -TestAssemblies $TestAssemblies -DotSettingsFile $DotSettingsFile -DotCoverResultsFile $DotCoverCoverageSnapshotFile

    Create-DotCoverCoverageBadge $DotCoverCoverageSnapshotFile $DotCoverCoverageBadgeFile
    Create-DotCoverCoverageReport $DotCoverCoverageSnapshotFile $DotCoverCoverageReportFile
  }
  else {
    Execute-MSpecTests -TestAssemblies $TestAssemblies 
  }
}

BuildTask Create-NuGetPackages {
  $commitHash = Get-CurrentCommitHash
  $branchName = Get-CurrentBranchName
  
  $vcsUrlTemplate = "https://raw.githubusercontent.com/chrischu/SolutionInspector/${commitHash}/${branchName}/{0}"
  Create-NuGetPackagesFromSolution $SolutionDirectory $Projects $AssemblyInformationalVersion $Configuration $vcsUrlTemplate $NuGetPackagesDirectory

  Get-ChildItem $NuGetPackagesDirectory *.nupkg | %{ Report-NuGetPackage $_.FullName }
}

BuildTask Push-Packages {
   Push-AllNuGetPackages $NuGetPackagesDirectory $TargetNuGetFeed $NuGetApiKey
}

BuildTask Create-Archives {
  $archiveName = "SolutionInspector-${AssemblyInformationalVersion}.zip"
  $archivePath = Join-Path $BuildOutputDirectory $archiveName

  Write-Host "Creating archive '$archiveName'"

  $buildDirectory = Join-Path $SolutionDirectory "SolutionInspector\bin\$Configuration"
  $sourceDirectory = Join-Path ([System.IO.Path]::GetTempPath()) ([System.Guid]::NewGuid())
  New-Item -ItemType Directory -Path $sourceDirectory | Out-Null
  Copy-Item -Path "${buildDirectory}\*" -Destination $sourceDirectory -Exclude "Microsoft.Build*.dll","*CodeAnalysisLog.xml","*.lastcodeanalysissucceeded"
  Zip-Directory -ZipFilePath $archivePath -SourceDirectory $sourceDirectory
  Remove-Item $sourceDirectory -Recurse

  Report-Archive $archivePath
}

BuildTask Publish-CoverageReports {
  Publish-CoverageReport $AssemblyInformationalVersion $DotCoverCoverageBadgeFile $DotCoverCoverageReportFile
}

return Run