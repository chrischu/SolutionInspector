# It is important that there is either no enviroment variable "VisualStudioVersion" or it is set to at least "14.0", 
# otherwise building/cleaning solutions that require the VSSDK won't work anymore.

BuildStep Clean-Solution {
  Param([Parameter(Mandatory)] [string] $solutionFile)

  Exec { & $MSBuildExecutable $solutionFile /t:Clean /m /nr:False } -ErrorMessage "Could not clean solution '$solutionFile'"
}

BuildStep Build-Solution {
  Param(
      [Parameter(Mandatory)] [string] $solutionFile, 
      [Parameter(Mandatory)] [string] $configuration, 
      [Parameter(Mandatory)] [boolean] $treatWarningsAsErrors, 
      [Parameter(Mandatory)] [boolean] $runFxCopCodeAnalysis,
      [Parameter(Mandatory)] [string] $fxCopResultsDirectory)

  $treatCodeAnalysisWarningsAsErrorsParam = ""
  if (($Mode -eq "Local") -and $runFxCopCodeAnalysis) {
    $treatCodeAnalysisWarningsAsErrorsParam = ";CodeAnalysisTreatWarningsAsErrors=True"
  }

  Exec { & $MSBuildExecutable $solutionFile /t:Build /m /nr:False "/p:Configuration=$configuration;TreatWarningsAsErrors=$treatWarningsAsErrors;RunCodeAnalysis=$runFxCopCodeAnalysis$treatCodeAnalysisWarningsAsErrorsParam" } -ErrorMessage "Could not build solution '$solutionFile'"

  $builtProjects = Get-ChildItem -Directory | where { Test-Path "$_\bin\$configuration" }
  $fxCopResultsFiles = $builtProjects | % { Join-Path $_.FullName "bin\$configuration\" } | % { Get-ChildItem $_ -Filter "*.CodeAnalysisLog.xml" }
  
  $fxCopResultsFiles | %{ Copy-Item $_.FullName $fxCopResultsDirectory -PassThru } | %{ Report-FxCopCodeAnalysisResults $_.FullName }
}

BuildStep Package-Projects {
  Param(
      [Parameter(Mandatory)] [string] $solutionDirectory, 
      [Parameter(Mandatory)] [string[]] $projectFiles, 
      [Parameter(Mandatory)] [string] $configuration)

  # We explicitly use '\' here (instead of sth like Path.DirectorySeparatorChar) because it is a convention in
  # MSBuild to use backslashes for directory separators.
  $solutionDirectory = Ensure-StringEndsWith $solutionDirectory '\'

  foreach ($projectFile in $projectFiles) {
    Exec { & $MSBuildExecutable $projectFile /t:Package /m /nr:False "/p:Configuration=$configuration;SolutionDir=$solutionDirectory" } -ErrorMessage "Could not package project '$projectFile'"
  }
}