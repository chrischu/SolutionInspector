using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SolutionInspector.Configuration;

namespace SolutionInspector.Api.Configuration.Ruleset
{
  public interface IRulesetConfiguration
  {
    IReadOnlyCollection<string> RuleAssemblyImports { get; }
    IRulesConfiguration Rules { get; }
  }

  public class RulesetConfigurationDocument : ConfigurationDocument, IRulesetConfiguration
  {
    [ConfigurationCollection (ElementName = "import")]
    public ConfigurationElementCollection<RuleAssemblyImportConfigurationElement> RuleAssemblyImports
        => GetConfigurationCollection<RuleAssemblyImportConfigurationElement>();

    [ConfigurationSubelement]
    public RulesConfigurationElement Rules => GetConfigurationSubelement<RulesConfigurationElement>();

    [ExcludeFromCodeCoverage]
    IReadOnlyCollection<string> IRulesetConfiguration.RuleAssemblyImports => RuleAssemblyImports.Select(i => i.Path).ToArray();

    [ExcludeFromCodeCoverage]
    IRulesConfiguration IRulesetConfiguration.Rules => Rules;
  }
}