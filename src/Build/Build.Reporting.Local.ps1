function Report-ReSharperInspectCodeResults {
  [CmdletBinding()]
  Param([Parameter(Mandatory)] [string] $resultsFile)
}

function Report-FxCopCodeAnalysisResults {
  [CmdletBinding()]
  Param([Parameter(Mandatory)] [string] $resultsFile)
}

function Report-DotCoverCoverageAnalysisResults {
  [CmdletBinding()]
  Param([Parameter(Mandatory)] [string] $resultsFile)
}

function Report-NuGetPackage {
  [CmdletBinding()]
  Param([Parameter(Mandatory)] [string] $nuGetPackagePath)
}

function Report-Archive {
  [CmdletBinding()]
  Param([Parameter(Mandatory)] [string] $archivePath)
}

function Report-TestError {
  [CmdletBinding()]
  Param([Parameter(Mandatory)] $error)

  throw $error
}