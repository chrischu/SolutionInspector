function Exec {
  [CmdletBinding()]
  Param(
      [Parameter(Mandatory)] [scriptblock] $command, 
      [Parameter(Mandatory)] [string] $errorMessage, 
      [bool] $failOnErrorCode = $True)

  $expandedCommandString = $ExecutionContext.InvokeCommand.ExpandString($command)
  Write-Host "Executing: $expandedCommandString"

  & $command | Out-Host
  if ($failOnErrorCode -and $LastExitCode -ne 0) {
    throw "BUILD FAILED: $errorMessage (LastExitCode: $LastExitCode)."
  }
}

function Get-BackupFileName {
  [CmdletBinding()]
  Param([Parameter(Mandatory)] [string] $file)

  return "$file.bak"
}

function Backup-File {
  [CmdletBinding()]
  Param([Parameter(Mandatory)] [string] $file)

  Write-Host "Backing up '$file'"

  $backupFile = Get-BackupFileName $file
  Copy-Item $file $backupFile | Out-Null
}

function Restore-File {
  [CmdletBinding()]
  Param([Parameter(Mandatory)] [string] $file)

  $backupFile = Get-BackupFileName $file
  if (Test-Path $backupFile) {
    Write-Host "Restoring '$file'"

    Move-Item -Force $backupFile $file | Out-Null
  }
}

BuildStep Clean-BuildDirectory {
  Param([Parameter(Mandatory)] [string] $buildDirectory)

  if (Test-Path $buildDirectory) {
    Remove-Item $buildDirectory -Recurse -Force
  }
}

function Get-ProjectDirectoriesFromSolution {
  [CmdletBinding()]
  Param([Parameter(Mandatory)] [string] $solutionFile)

  $solutionFolderProjectType = "2150E333-8FDC-42A3-9474-1A3956D46DE8"

  $solutionFileLines = Get-Content $solutionFile
  foreach($line in $solutionFileLines) {
    if($line.StartsWith("Project(") -and !$line.Contains($solutionFolderProjectType)) {
      [void] ($line -match '.*?= ".*?", "(?<projectDirectory>.*?)\\.*"')
      $projectDirectory = $matches['projectDirectory']
      Get-Item $projectDirectory
    }
  }
}

function Get-BinDirectory {
  [CmdletBinding()]
  Param(
      [Parameter(Mandatory)] [string] $projectDirectory, 
      [Parameter(Mandatory)] [string] $configuration)

  return "$projectDirectory\bin\$configuration"
}

BuildStep Update-AssemblyInfo -LogMessage 'Update-AssemblyInfo ''$file''' {
  Param(
    [Parameter(Mandatory)] [string] $file, 
    [Parameter(Mandatory)] [string] $configuration, 
    [Parameter(Mandatory)] [string] $assemblyVersion, 
    [Parameter(Mandatory)] [string] $assemblyFileVersion,
    [Parameter(Mandatory)] [string] $assemblyInformationalVersion)

  Backup-File $file

  Update-File $file {
    $_ -Replace 'AssemblyConfiguration\s*\(".+"\)', ('AssemblyConfiguration ("' + $configuration + '")') `
    -Replace 'AssemblyVersion\s*\(".+"\)', ('AssemblyVersion ("' + $assemblyVersion + '")') `
    -Replace 'AssemblyFileVersion\s*\(".+"\)', ('AssemblyFileVersion ("' + $assemblyFileVersion + '")') `
    -Replace 'AssemblyInformationalVersion\s*\(".+"\)', ('AssemblyInformationalVersion ("' + $assemblyInformationalVersion + '")')
  }
}

function Update-File {
  Param(
      [Parameter(Mandatory)] [string] $file, 
      [Parameter(Mandatory)] [ScriptBlock] $updateLine, 
      [System.Text.Encoding] $encoding = [System.Text.Encoding]::UTF8)

  $lines = [System.IO.File]::ReadAllLines($file, $encoding)

  $updatedLines = $lines | % { & $updateLine $_ }
  $text = $updatedLines -join [Environment]::NewLine

  [System.IO.File]::WriteAllText($file, $text, $encoding)
}

BuildStep Restore-AssemblyInfo -LogMessage 'Restore-AssemblyInfo ''$file''' {
  Param([Parameter(Mandatory)] [string] $file)

  Restore-File $file
}

function Ensure-StringEndsWith {
  [CmdletBinding()]
  Param(
    [Parameter(Mandatory)] [string] $string,
    [Parameter(Mandatory)] [char] $char
  )

  if ($string.EndsWith($char)) {
    return $string
  }

  return $string + $char
}

Add-Type -As System.IO.Compression.FileSystem

function Zip-Directory {
  [CmdletBinding()]
  Param(
      [Parameter(Mandatory)] [string] $zipFilePath, 
      [Parameter(Mandatory)] [string] $sourceDirectory, 
      [System.IO.Compression.CompressionLevel] $compression = "Fastest")

  [System.IO.Compression.ZipFile]::CreateFromDirectory($sourceDirectory, $zipFilePath, $compression, $false)
}