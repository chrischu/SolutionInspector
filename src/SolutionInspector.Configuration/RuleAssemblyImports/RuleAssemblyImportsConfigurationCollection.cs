using System;
using JetBrains.Annotations;
using SolutionInspector.Api.Configuration;

namespace SolutionInspector.Configuration.RuleAssemblyImports
{
  [UsedImplicitly]
  internal class RuleAssemblyImportsConfigurationCollection : KeyedConfigurationElementCollectionBase<RuleAssemblyImportConfigurationElement, string>
  {
    protected override string ElementName => "import";
  }
}