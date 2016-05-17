function Report-ReSharperInspectCodeResults {
  [CmdletBinding()]
  Param([Parameter(Mandatory)] [string] $resultsFile)

  Write-Host "Reporting ReSharper InspectCode result file '$resultsFile' as AppVeyor artifact..."
  Push-AppVeyorArtifact $resultsFile
}

function Report-FxCopCodeAnalysisResults {
  [CmdletBinding()]
  Param([Parameter(Mandatory)] [string] $resultsFile)

  Write-Host "Reporting FxCop result file '$resultsFile' as AppVeyor artifact..."
  Push-AppVeyorArtifact $resultsFile
}

function Report-DotCoverCoverageAnalysisResults {
  [CmdletBinding()]
  Param([Parameter(Mandatory)] [string] $resultsFile)

  Write-Host "Reporting dotCover result file '$resultsFile' as AppVeyor artifact..."
  Push-AppVeyorArtifact $resultsFile
}

function Report-NuGetPackage {
  [CmdletBinding()]
  Param([Parameter(Mandatory)] [string] $nuGetPackagePath)

  Write-Host "Reporting NuGet package '$nuGetPackagePath' as AppVeyor artifact..."
  Push-AppVeyorArtifact $nuGetPackagePath
}

function Report-Archive {
  [CmdletBinding()]
  Param([Parameter(Mandatory)] [string] $archivePath)

  Write-Host "Reporting archive '$archivePath' as AppVeyor artifact..."
  Push-AppVeyorArtifact $archivePath -DeploymentName SolutionInspector.zip
}

function Report-TestError {
  [CmdletBinding()]
  Param([Parameter(Mandatory)] $error)

  throw $error
}