function Log-BuildTaskStart {
  [CmdletBinding()]
  Param([Parameter(Mandatory)] [string] $task)

  Write-Host ""
  Write-Host -ForegroundColor "Yellow" "===== BUILD TASK:   $task   =====`n"
}

function Log-BuildTaskEnd {
  [CmdletBinding()]
  Param([Parameter(Mandatory)] [string] $task)
}

function Log-BuildStepStart {
  [CmdletBinding()]
  Param([Parameter(Mandatory)] [string] $step)

  Write-Host ""
  Write-Host -ForegroundColor "Magenta" "===== BUILD STEP:   $step   =====`n"
}

function Log-BuildStepEnd {
  [CmdletBinding()]
  Param([Parameter(Mandatory)] [string] $step)
}