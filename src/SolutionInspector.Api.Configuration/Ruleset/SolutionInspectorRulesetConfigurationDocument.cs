using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SolutionInspector.Configuration;

namespace SolutionInspector.Api.Configuration.Ruleset
{
  public interface ISolutionInspectorRulesetConfiguration
  {
    IReadOnlyCollection<string> RuleAssemblyImports { get; }
    IRulesConfiguration Rules { get; }
  }

  public class SolutionInspectorRulesetConfigurationDocument : ConfigurationDocument, ISolutionInspectorRulesetConfiguration
  {
    [ConfigurationCollection (ElementName = "import")]
    public ConfigurationElementCollection<RuleAssemblyImportConfigurationElement> RuleAssemblyImports
        => GetConfigurationCollection<RuleAssemblyImportConfigurationElement>();

    [ConfigurationValue]
    public RulesConfigurationElement Rules => GetConfigurationProperty<RulesConfigurationElement>();

    [ExcludeFromCodeCoverage]
    IReadOnlyCollection<string> ISolutionInspectorRulesetConfiguration.RuleAssemblyImports => RuleAssemblyImports.Select(i => i.Path).ToArray();

    [ExcludeFromCodeCoverage]
    IRulesConfiguration ISolutionInspectorRulesetConfiguration.Rules => Rules;
  }
}