BuildStep Execute-ReSharperCodeInspection {
  Param(
      [Parameter(Mandatory)] [string] $solutionFile,
      [Parameter(Mandatory)] [string] $configuration,
      [Parameter(Mandatory)] [string] $resultsFile)

  $reSharperCommandLineToolsPath = Get-NuGetSolutionPackagePath "JetBrains.ReSharper.CommandLineTools"
  $reSharperInspectCodeExecutable = "$reSharperCommandLineToolsPath\tools\inspectcode.exe"
  $reSharperInspectCodeExtensions = "ReSharper.ImplicitNullability;ReSharper.SerializationInspections;ReSharper.XmlDocInspections"

  Exec { 
    & $reSharperInspectCodeExecutable `
      --caches-home=_ReSharperInspectCodeCache `
      --toolset=$MSBuildToolset `
      -o="$resultsFile" `
      --properties="Configuration=$configuration" `
      /x=$reSharperInspectCodeExtensions `
       $solutionFile
  } -ErrorMessage "ReSharper code inspection failed"

  Report-ReSharperInspectCodeResults $resultsFile

  [xml] $xml = Get-Content $resultsFile
  $numberOfIssues = $xml.CreateNavigator().Evaluate("count(//Issue)")

  if (($Mode -eq "Local") -and $numberOfIssues -gt 0) {
    throw "BUILD FAILED: There are $numberOfIssues ReSharper code inspection issues."
  }

  Write-Host "ReSharper InspectCode found $numberOfIssues issues."

  return $numberOfIssues
}