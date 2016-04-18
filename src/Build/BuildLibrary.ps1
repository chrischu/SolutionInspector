. $PSScriptRoot\Build.Logging.ps1

# Creates a function that represents a build task.
# Build tasks have no parameters, except a condition that controls if the task should be executed (like the MSBuild Condition property). 
# Additionally build task do some logging when they start and end.
function BuildTask {
  [CmdletBinding()]
  Param(
      [Parameter(Mandatory)] [string] $name, 
      [Parameter(Mandatory)] [ScriptBlock] $scriptBlock, 
      [string] $logMessage)

  $logStart = Get-Item Function:/Log-BuildTaskStart
  $logEnd = Get-Item Function:/Log-BuildTaskEnd

  if ([string]::IsNullOrEmpty($logMessage)) {
    $logMessage = $name
  }

  $wrapper = {
    [CmdletBinding()]
    Param([bool] $Condition = $true)

    if($Condition) {
      $expandedMessage = $ExecutionContext.InvokeCommand.ExpandString($logMessage)

      & $logStart $expandedMessage
      & $scriptBlock
      & $logEnd $expandedMessage
    }
  }.GetNewClosure()

  New-Item -Force -Path "function:global:$name" -Value $wrapper | Out-Null
}

# Creates a function that represents a build step.
# Build step can have parameters, additionally they do some logging when they start and end.
function BuildStep {
  [CmdletBinding()]
  Param(
      [Parameter(Mandatory)] [string] $name, 
      [Parameter(Mandatory)] [ScriptBlock] $scriptBlock, 
      [string] $logMessage)

  $logStart = Get-Item Function:/Log-BuildStepStart
  $logEnd = Get-Item Function:/Log-BuildStepEnd

  if ([string]::IsNullOrEmpty($logMessage)) {
    $logMessage = $name
  }

  $params = ""
  if ($scriptBlock.AST.ParamBlock -ne $null) {
    $params = $scriptBlock.AST.ParamBlock.ToString()
  }

  # Add parameters to wrapper, to allow them to be passed on and also be contained in the Help
  $wrapperCodeWithParams = @"
    [CmdletBinding()]
    $params

    `$expandedMessage = `$ExecutionContext.InvokeCommand.ExpandString(`$logMessage)
    
    & `$logStart `$expandedMessage
    & `$scriptBlock `@PSBoundParameters
    & `$logEnd `$expandedMessage
"@

  $wrapper = [ScriptBlock]::Create($wrapperCodeWithParams).GetNewClosure()

  New-Item -Force -Path "function:global:$name" -Value $wrapper | Out-Null
}

. $PSScriptRoot\Build.Common.ps1

switch($Mode) {
  "Local" {
    . $PSScriptRoot\Build.Reporting.Local.ps1
  }
  "AppVeyor" {
    . $PSScriptRoot\Build.Reporting.AppVeyor.ps1
  }
}

. $PSScriptRoot\Build.MSBuild.ps1
. $PSScriptRoot\Build.NuGet.ps1
. $PSScriptRoot\Build.Testing.ps1
. $PSScriptRoot\Build.ReSharperInspectCode.ps1