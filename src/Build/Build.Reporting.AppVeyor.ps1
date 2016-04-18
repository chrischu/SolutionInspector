function Report-ReSharperInspectCodeResults {
  [CmdletBinding()]
  Param([Parameter(Mandatory)] [string] $resultsFile)

  Push-AppVeyorArtifact $resultsFile
}

function Report-FxCopCodeAnalysisResults {
  [CmdletBinding()]
  Param([Parameter(Mandatory)] [string] $resultsFile)

  Push-AppVeyorArtifact $resultsFile
}

function Report-DotCoverCoverageAnalysisResults {
  [CmdletBinding()]
  Param([Parameter(Mandatory)] [string] $resultsFile)

  Push-AppVeyorArtifact $resultsFile
}

function Report-NuGetPackage {
  [CmdletBinding()]
  Param([Parameter(Mandatory)] [string] $nuGetPackagePath)

  Push-AppVeyorArtifact $nuGetPackagePath -Type "NuGetPackage"
}

function Report-TestError {
  [CmdletBinding()]
  Param([Parameter(Mandatory)] $error)

  throw $error
}