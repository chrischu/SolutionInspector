using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using JetBrains.Annotations;

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

  internal class RuleAssemblyImportsConfigurationSection : ConfigurationSection, IRuleAssemblyImportsConfiguration
  {
    public const string ExampleConfiguration = @"<ruleAssemblyImports>
  <import path=""C:\Path\To\Assembly.dll"" />
</ruleAssemblyImports>";

    [UsedImplicitly]
    [ConfigurationProperty ("", IsRequired = true, IsKey = false, IsDefaultCollection = true)]
    public RuleAssemblyImportsConfigurationCollection Imports
    {
      get { return (RuleAssemblyImportsConfigurationCollection) base[""]; }
      set { base[""] = value; }
    }

    IReadOnlyCollection<string> IRuleAssemblyImportsConfiguration.Imports => Imports.Select(i => i.Path).ToArray();
  }
}