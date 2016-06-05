#Requires -Version 3.0

[CmdletBinding()]
Param (
  [Parameter()]
  [string] $Version = "0.0.0",
  [Parameter()]
  [int] $BuildCounter = 0,
  [Parameter()]
  [string] $CommitHash,
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
  [bool] $CreateArchives = $False,
  [Parameter()]
  [bool]$PushNuGetPackages = $False,
  [Parameter()]
  [bool] $IsPreReleaseBuild = $False,
  [Parameter()] 
  [string] $TargetNuGetFeed = "https://www.nuget.org",
  [Parameter()]
  [string] $NuGetApiKey,
  [Parameter(Mandatory)]
  [ValidateNotNullOrEmpty()]
  [ValidateSet("Local", "AppVeyor")]
  [string] $Mode
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = "Stop"
$ConfirmPreference = "None"
trap { $error[0] | Format-List -Force; $host.SetShouldExit(1) }

. $PSScriptRoot\Build\BuildLibrary.ps1

$TreatWarningsAsErrors = $True
$MSBuildToolset = "14.0"
$MSBuildExecutable = "C:\Program Files (x86)\MSBuild\$MSBuildToolset\Bin\MSBuild.exe"
$NuGetExecutable = ".\.nuget\NuGet.exe"

$SolutionDirectory = $PSScriptRoot
$SolutionFile = Join-Path $SolutionDirectory "SolutionInspector.sln"
$DotSettingsFile = "${SolutionFile}.DotSettings"

$AssemblyInfoSharedFile = Join-Path $SolutionDirectory "AssemblyInfoShared.cs"

$BuildOutputDirectory = Join-Path $SolutionDirectory "BuildOutput"
$AnalysisResultsDirectory = Join-Path $BuildOutputDirectory "AnalysisResults"
$FxCopResultsDirectory = Join-Path $AnalysisResultsDirectory "FxCop"
$NuGetPackagesDirectory = Join-Path $BuildOutputDirectory "NuGetPackages"

# Version
$ParsedVersion = New-Object Version $Version
$AssemblyFileVersion = New-Object Version $ParsedVersion.Major,$ParsedVersion.Minor,$ParsedVersion.Build,0
$AssemblyVersion = New-Object Version $AssemblyFileVersion.Major,0,0,0
$AssemblyInformationalVersion = New-Object Version $AssemblyFileVersion.Major,$AssemblyFileVersion.Minor,$AssemblyFileVersion.Build

if ($IsPreReleaseBuild) {
  $AssemblyInformationalVersion = "$AssemblyInformationalVersion-pre$BuildCounter"
}

$ProjectDirectories = Get-ProjectDirectoriesFromSolution $SolutionFile

$TestProjectDirectories = $ProjectDirectories | ? { $_.Name.EndsWith("Tests") }
$TestAssemblies = $TestProjectDirectories | % { Join-Path $_.FullName "bin\$Configuration\$($_.Name).dll" }

function Run() {
  try {
    Write-Host "Building '$SolutionFile'"
    Write-Host "Version: $Version"
    Write-Host "Configuration: $Configuration"
    Write-Host "Run tests: $RunTests (with DotCover coverage analysis: $RunDotCoverCoverageAnalysis)"
    Write-Host "Run FxCop code analysis: $RunFxCopCodeAnalysis"
    Write-Host "Run ReSharper code inspection: $RunReSharperCodeInspection"
    Write-Host "Build NuGetPackages: $CreateNuGetPackages (with version: $AssemblyInformationalVersion)"
    Write-Host "Push NuGet packages: $PushNuGetPackages (to $TargetNuGetFeed)"

    Clean
    Restore-NuGetPackages
    Update-AssemblyInfos
    Build
    Run-ReSharperCodeInspection -Condition $RunReSharperCodeInspection
    Run-Tests -Condition $RunTests
    Create-NuGetPackages -Condition $CreateNuGetPackages
    Push-Packages -Condition ($PushNuGetPackages -And $CreateNuGetPackages)
    Create-Archives -Condition $CreateArchives
  } finally {
    Restore-AssemblyInfos
  }
}

BuildTask Clean {
  Clean-BuildDirectory $BuildOutputDirectory
  Clean-Solution $SolutionFile
  New-Item $AnalysisResultsDirectory, $FxCopResultsDirectory, $NuGetPackagesDirectory -Type Directory | Out-Null
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


BuildTask Build {
  Build-Solution $SolutionFile $Configuration $TreatWarningsAsErrors $RunFxCopCodeAnalysis $FxCopResultsDirectory
}

BuildTask Run-ReSharperCodeInspection {
  $ReSharperCodeInspectionResultsFile = Join-Path $AnalysisResultsDirectory "ReSharperCodeInspectionResults.xml"
  $numberOfIssues = Execute-ReSharperCodeInspection $SolutionFile $Configuration $ReSharperCodeInspectionResultsFile
}

BuildTask Run-Tests {
  if ($RunDotCoverCoverageAnalysis) {
    $DotCoverCoverageAnalysisResultsFile = Join-Path $AnalysisResultsDirectory "DotCoverCoverageAnalysisResults.dcvr"
    Execute-MSpecTests -WithDotCover -TestAssemblies $TestAssemblies -DotSettingsFile $DotSettingsFile -DotCoverResultsFile $DotCoverCoverageAnalysisResultsFile
  }
  else {
    Execute-MSpecTests -TestAssemblies $TestAssemblies 
  }
}

BuildTask Create-NuGetPackages {
  if ([string]::IsNullOrEmpty($CommitHash)) {
    throw "BUILD FAILED: Cannot build NuGet packages without VCS commit hash."
  }
  
  $vcsUrlTemplate = "https://raw.githubusercontent.com/chrischu/SolutionInspector/$CommitHash/{0}"
  Create-NuGetPackagesFromSolution $SolutionDirectory $AssemblyInformationalVersion $Configuration $vcsUrlTemplate $NuGetPackagesDirectory

  Get-ChildItem $NuGetPackagesDirectory *.nupkg | %{ Report-NuGetPackage $_.FullName }
}

BuildTask Push-Packages {
   Push-AllNuGetPackages $NuGetPackagesDirectory $TargetNuGetFeed $NuGetApiKey
}

BuildTask Create-Archives {
  $archivePath = Join-Path $BuildOutputDirectory "SolutionInspector-$AssemblyInformationalVersion.zip"

  $buildDirectory = Join-Path $SolutionDirectory "SolutionInspector\bin\$Configuration"
  $sourceDirectory = Join-Path ([System.IO.Path]::GetTempPath()) ([System.Guid]::NewGuid())
  New-Item -ItemType Directory -Path $sourceDirectory
  Copy-Item -Path "${buildDirectory}\*" -Destination $sourceDirectory -Exclude "Microsoft.Build*.dll","*CodeAnalysisLog.xml","*.lastcodeanalysissucceeded"
  Zip-Directory -ZipFilePath $archivePath -SourceDirectory $sourceDirectory
  Remove-Item $sourceDirectory -Recurse

  Report-Archive $archivePath
}

Run