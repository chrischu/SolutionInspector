using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolutionInspector.Configuration;

namespace SolutionInspector.Api.Configuration.RuleAssemblyImports
{
  /// <summary>
  ///   Configuration used to import rule assemblies.
  /// </summary>
  public interface IRuleAssemblyImportsConfiguration
  {
    /// <summary>
    ///   A collection of paths (files and directories) from which to load assemblies.
    /// </summary>
    IReadOnlyCollection<string> Imports { get; }
  }

  internal class RuleAssemblyImportsConfigurationSection : ConfigurationElement, IRuleAssemblyImportsConfiguration
  {
    public const string ExampleConfiguration = @"<ruleAssemblyImports>
  <import path=""C:\Path\To\Assembly.dll"" />
</ruleAssemblyImports>";

    [UsedImplicitly]
    [ConfigurationCollection (IsDefaultCollection = true, ElementName = "import", IsOptional = false)]
    public ConfigurationElementCollection<RuleAssemblyImportConfigurationElement> Imports
        => GetConfigurationCollection<RuleAssemblyImportConfigurationElement>();

    IReadOnlyCollection<string> IRuleAssemblyImportsConfiguration.Imports => Imports.Select(i => i.Path).ToArray();
  }
}