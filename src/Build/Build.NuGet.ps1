Add-Type @"
public struct NuGetPackProject {
  public string CsProjFile;
  public string OutputPath;
}
"@

BuildStep Restore-NuGetPackagesForSolution {
  Param([Parameter(Mandatory)] [string] $solutionFile)

  Exec { & $NuGetExecutable restore $solutionFile } -ErrorMessage "Could not bootstrap solution '$solutionFile' - failed to restore NuGet packages"
}

BuildStep Create-NuGetPackagesFromSolution -LogMessage 'Create-NuGetPackagesFromSolution in ''$solutionDirectory''' {
  Param(
      [Parameter(Mandatory)] [string] $solutionDirectory, 
      [Parameter(Mandatory)] [string] $version, 
      [Parameter(Mandatory)] [string] $configuration, 
      [Parameter(Mandatory)] [string] $vcsUrlTemplate, 
      [Parameter(Mandatory)] [string] $resultsDirectory)

  $nuGetPackProjects = _Get-NuGetProjectFiles $solutionDirectory $configuration

  try {
    _Create-DummyNuSpecAndDirectoriesForNuGet $solutionDirectory $nuGetPackProjects

    _Insert-PdbSourceLinks $nuGetPackProjects $solutionDirectory $vcsUrlTemplate

    foreach ($nuGetPackProject in $nuGetPackProjects) {
      # IDEA: Add parameter for excludes
      $csProjFile = $nuGetPackProject.CsProjFile
      $outputPath = $nuGetPackProject.OutputPath
      Exec { 
        & $NuGetExecutable pack $csProjFile `
          -Version $version `
          -Properties "Configuration=$configuration;OutputDir=$outputPath" `
          -IncludeReferencedProjects `
          -OutputDirectory $resultsDirectory `
      } -ErrorMessage "Could not create NuGet package for '$csProjFile'"
    }
  } finally {
    _Remove-DummyNuSpecAndDirectoriesForNuGet $nuGetPackProjects
  }
}

BuildStep Create-NuGetPackageFromNuSpec -LogMessage 'Create-NuGetPackageFromNuSpec ''$nuSpecFile''' {
  Param(
      [Parameter(Mandatory)] [string] $nuSpecFile, 
      [Parameter(Mandatory)] [string] $version, 
      [Parameter(Mandatory)] [string] $resultsDirectory, 
      [switch] $noPackageAnalysis)

  $packageAnalysis = ""
  if($noPackageAnalysis.IsPresent) {
    $packageAnalysis = "-NoPackageAnalysis"
  }

  Exec { 
    & $NuGetExecutable pack $nuSpecFile `
      -Version $version `
      -Properties Configuration=$Configuration `
      -OutputDirectory $resultsDirectory $packageAnalysis 
  } -ErrorMessage "Could not create NuGet package for '$nuSpecFile'"
}

BuildStep Push-AllNuGetPackages -LogMessage 'Push-AllNuGetPackages from ''$packageDirectory'' to ''$targetFeed''' {
  Param(
      [Parameter(Mandatory)] [string] $packageDirectory, 
      [Parameter(Mandatory)] [string] $targetFeed,
      [Parameter(Mandatory)] [string] $nuGetApiKey)

  Exec { & $NuGetExecutable push "$packageDirectory\*.nupkg" -Source $targetFeed -ApiKey $nuGetApiKey } -ErrorMessage "Could not push NuGet packages to '$targetFeed'"
}

function Get-NuGetSolutionPackagePath {
  [CmdletBinding()]
  Param([Parameter(Mandatory)] [string] $solutionPackage)

  $solutionPackagesConfigFile = ".\SolutionPackages\packages.config"

  [xml] $xml = Get-Content $solutionPackagesConfigFile
  $package = $xml.SelectSingleNode("/packages/package[@id = '$solutionPackage']")

  if($package -eq $null) {
    throw "ERROR: Cannot find solution package '$solutionPackage'."
  }

  $version = $package.Version

  return ".\packages\$solutionPackage.$version"
}

function _Insert-PdbSourceLinks {
  [CmdletBinding()]
  Param(
      [Parameter(Mandatory)] [NuGetPackProject[]] $nuGetPackProjects, 
      [Parameter(Mandatory)] [string] $solutionDirectory, 
      [Parameter(Mandatory)] [string] $vcsUrlTemplate)

  foreach ($nuGetPackProject in $nuGetPackProjects) {
    $projectDirectory = [System.IO.Path]::GetDirectoryName($nuGetPackProject.CsProjFile)
    $projectOutputPath = $nuGetPackProject.OutputPath
    $projectName = [System.IO.Path]::GetFileName($projectDirectory)
    $pdbFile = [System.IO.Path]::Combine($projectDirectory, $projectOutputPath, "$projectName.pdb")

    $remotionMSBuildTasksDll = Join-Path (Get-NuGetSolutionPackagePath "Remotion.BuildTools.MSBuildTasks") "tools\Remotion.BuildTools.MSBuildTasks.dll"
    $insertSourceLinksMsBuildFile = Join-Path $solutionDirectory "InsertSourceLinks.msbuild"

    Exec { 
      & $MSBuildExecutable $insertSourceLinksMsBuildFile `
        /t:InsertSourceLinks `
        "/p:RemotionMSBuildTasksDll=$remotionMSBuildTasksDll;PdbFile=$pdbFile;SolutionDirectory=$solutionDirectory;VcsUrlTemplate=`"$vcsUrlTemplate`"" 
    } -ErrorMessage "Could not insert source links into PDB '$($nuGetPackProject.CsProjFile)'"
  }
}

function _Get-NuGetProjectFiles {
  [CmdletBinding()]
  Param(
    [Parameter(Mandatory)] [string] $solutionDirectory,
    [Parameter(Mandatory)] [string] $configuration)

  $nuGetPackProjects = @()

  $allCsProjFiles = Get-ChildItem $solutionDirectory -Recurse -Filter "*.csproj" | %{ $_.FullName }
  foreach ($csProjFile in $allCsProjFiles) {
    [xml] $xml = Get-Content $csProjFile
    [System.Xml.XmlNamespaceManager] $nsmgr = $xml.NameTable
    $nsmgr.AddNamespace("msb", "http://schemas.microsoft.com/developer/msbuild/2003")
    $elem = $xml.SelectNodes("//msb:Link[starts-with(text(), 'Properties') and contains(text(), '.nuspec')]", $nsmgr)

    if ($elem.Count -gt 0) {
      $nuGetPackProject = New-Object NuGetPackProject
      $nuGetPackProject.CsProjFile = $csProjFile

      $propertyGroup = $xml.SelectNodes("//msb:PropertyGroup[contains(@Condition, '`$(Configuration)|') and contains(@Condition, '$configuration|')]", $nsmgr)

      if($propertyGroup.Count -ne 1) {
        throw "Found $($propertyGroup.Count) property groups for configuration '$configuration'."
      }
      
      $nuGetPackProject.OutputPath = $propertyGroup.OutputPath

      $nuGetPackProjects = $nuGetPackProjects + $nuGetPackProject
    }
  }

  return $nuGetPackProjects
}

function _Create-DummyNuSpecAndDirectoriesForNuGet {
  [CmdletBinding()]
  Param(
      [Parameter(Mandatory)] [string] $solutionDirectory, 
      [Parameter(Mandatory)] [NuGetPackProject[]] $nuGetPackProjects)

  _Create-DummyNuSpecFiles $nuGetPackProjects

  # Create special dummy directory as a workaround for a NuGet bug that occurs when NuGet pack is used for projects which are referencing other projects (see https://github.com/NuGet/Home/issues/1299)
  foreach($nuGetPackProject in $nuGetPackProjects) {
    $dummyDirectory = Join-Path $solutionDirectory $nuGetPackProject.OutputPath
    if (-not (Test-Path $dummyDirectory)) {
      New-Item $dummyDirectory -Type Directory | Out-Null
    }
  }
}

function _Remove-DummyNuSpecAndDirectoriesForNuGet {
  [CmdletBinding()]
  Param([Parameter(Mandatory)] [NuGetPackProject[]] $nuGetPackProjects)

  _Remove-DummyNuSpecFiles $nuGetPackProjects

  foreach($nuGetPackProject in $nuGetPackProjects) {
    $dummyBaseDirectory = Join-Path $solutionDirectory ($nuGetPackProject.OutputPath.Split('\')[0]) # we hard code "\" here because this is the convention of the OutputPath in .csprojs.
    if (Test-Path $dummyBaseDirectory) {
      if (Get-ChildItem $dummyBaseDirectory -Recurse -File) {
        throw "Didn't expect any files in temporarily created directory '$dummyBaseDirectory'."
      } else {
        Remove-Item $dummyBaseDirectory -Recurse
      }
    }
  }
}

function _Create-DummyNuSpecFiles {
  [CmdletBinding()]
  Param([Parameter(Mandatory)] [NuGetPackProject[]] $nuGetPackProjects)

  foreach ($nuGetPackProject in $nuGetPackProjects) {
    $dummyNuSpecFile = [System.IO.Path]::ChangeExtension($nuGetPackProject.CsProjFile, "nuspec")
    New-Item $dummyNuSpecFile -Type file | Out-Null
  }
}

function _Remove-DummyNuSpecFiles {
  [CmdletBinding()]
  Param([Parameter(Mandatory)] [NuGetPackProject[]] $nuGetPackProjects)

  foreach ($nuGetPackProject in $nuGetPackProjects) {
    $dummyNuSpecFile = [System.IO.Path]::ChangeExtension($nuGetPackProject.CsProjFile, "nuspec")
    Remove-Item $dummyNuSpecFile -Force
  }
}