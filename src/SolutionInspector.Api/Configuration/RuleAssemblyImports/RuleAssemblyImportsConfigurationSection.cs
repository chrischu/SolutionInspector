using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace SolutionInspector.Api.Configuration.RuleAssemblyImports
{
  public interface IRuleAssemblyImportsConfiguration
  {
    IReadOnlyCollection<string> Imports { get; }
  }

  internal class RuleAssemblyImportsConfigurationSection : ConfigurationSection, IRuleAssemblyImportsConfiguration
  {
    public const string ExampleConfiguration = @"<ruleAssemblyImports>
  <import path=""C:\Path\To\Assembly.dll"" />
</ruleAssemblyImports>";

    [ConfigurationProperty("", IsRequired = true, IsKey = false, IsDefaultCollection = true)]
    public RuleAssemblyImportsConfigurationCollection Imports
    {
      get { return (RuleAssemblyImportsConfigurationCollection)base[""]; }
      set { base[""] = value; }
    }
    
    IReadOnlyCollection<string> IRuleAssemblyImportsConfiguration.Imports => Imports.Select(i => i.Path).ToArray();
  }
}