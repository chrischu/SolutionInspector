using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using SolutionInspector.Configuration;

namespace SolutionInspector.Api.Configuration.Ruleset
{
  public interface IProjectRuleGroupConfiguration
  {
    INameFilter AppliesTo { get; }
    IReadOnlyCollection<IRuleConfiguration> Rules { get; }
  }

  public class ProjectRuleGroupConfigurationElement : ConfigurationElement, IProjectRuleGroupConfiguration
  {
    [ConfigurationValue]
    public NameFilter AppliesTo
    {
      get { return GetConfigurationProperty<NameFilter>(); }
      set { SetConfigurationProperty(value); }
    }

    [ConfigurationCollection (IsDefaultCollection = true, ElementName = "rule", IsOptional = true)]
    public ConfigurationElementCollection<RuleConfigurationElement> Rules => GetConfigurationCollection<RuleConfigurationElement>();

    [ExcludeFromCodeCoverage]
    INameFilter IProjectRuleGroupConfiguration.AppliesTo => AppliesTo;

    [ExcludeFromCodeCoverage]
    IReadOnlyCollection<IRuleConfiguration> IProjectRuleGroupConfiguration.Rules => Rules;
  }
}