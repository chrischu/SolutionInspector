using System;
using System.Configuration;
using JetBrains.Annotations;
using SolutionInspector.Api.Configuration.Infrastructure;

namespace SolutionInspector.Api.Configuration.RuleAssemblyImports
{
  [UsedImplicitly]
  internal class RuleAssemblyImportsConfigurationCollection : KeyedConfigurationElementCollectionBase<RuleAssemblyImportConfigurationElement, string>
  {
    protected override string ElementName => "import";
  }
}