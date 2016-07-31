function Apply-XmlDocTransform([string] $xml, [string] $xdt)
{
  if (!$xml -or !(Test-Path -path $xml -PathType Leaf)) {
    throw "Could not find file '$xml'."
  }

  if (!$xdt -or !(Test-Path -path $xdt -PathType Leaf)) {
    throw "Could not find file '$xdt'."
  }

  Add-Type -LiteralPath "C:\Program Files (x86)\MSBuild\Microsoft\VisualStudio\v14.0\Web\Microsoft.Web.XmlTransform.dll"

  $xmldoc = New-Object Microsoft.Web.XmlTransform.XmlTransformableDocument
  $xmldoc.PreserveWhitespace = $true
  $xmldoc.Load($xml)

  $transf = New-Object Microsoft.Web.XmlTransform.XmlTransformation($xdt)

  Write-Host "Transforming '$xml' with '$xdt'..."

  if (-not $transf.Apply($xmldoc))
  {
    throw "Transformation failed."
  }

  $xmldoc.Save($xml);
}

function Format-Xml([string] $path) {
  [xml] $xml = Get-Content $path

  $xws = New-Object System.Xml.XmlWriterSettings
  $xws.Indent = $true
  $xws.IndentChars = "  "

  $xw = [System.Xml.XmlWriter]::Create($path, $xws)
  $xml.Save($xw)
}

Apply-XmlDocTransform "SolutionInspector\App.config" "SolutionInspector.Api\App.config.uninstall.xdt"
Apply-XmlDocTransform "SolutionInspector\App.config" "SolutionInspector.Api\App.config.install.xdt"
Apply-XmlDocTransform "SolutionInspector\App.config" "SolutionInspector.DefaultRules\App.config.install.xdt"
Format-Xml "SolutionInspector\App.config"

Apply-XmlDocTransform "SolutionInspector\Template.SolutionInspectorConfig" "SolutionInspector.Api\App.config.uninstall.xdt"
Apply-XmlDocTransform "SolutionInspector\Template.SolutionInspectorConfig" "SolutionInspector.Api\App.config.install.xdt"
Apply-XmlDocTransform "SolutionInspector\Template.SolutionInspectorConfig" "SolutionInspector.DefaultRules\App.config.install.xdt"
Format-Xml "SolutionInspector\Template.SolutionInspectorConfig"
