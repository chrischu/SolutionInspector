using System.Collections.Generic;
using SolutionInspector.Configuration;

namespace SolutionInspector.Api.Configuration.Ruleset
{
  public interface IProjectItemRuleGroupConfiguration
  {
    INameFilter AppliesTo { get; }
    INameFilter InProject { get; }
    IReadOnlyCollection<IRuleConfiguration> Rules { get; }
  }

  public class ProjectItemRuleGroupConfigurationElement : ConfigurationElement, IProjectItemRuleGroupConfiguration
  {
    [ConfigurationValue]
    public NameFilter AppliesTo
    {
      get { return GetConfigurationProperty<NameFilter>(); }
      set { SetConfigurationProperty(value); }
    }

    [ConfigurationValue]
    public NameFilter InProject
    {
      get { return GetConfigurationProperty<NameFilter>(); }
      set { SetConfigurationProperty(value); }
    }

    [ConfigurationCollection (IsDefaultCollection = true, ElementName = "rule", IsOptional = true)]
    public ConfigurationElementCollection<RuleConfigurationElement> Rules => GetConfigurationCollection<RuleConfigurationElement>();

    INameFilter IProjectItemRuleGroupConfiguration.AppliesTo => AppliesTo;
    INameFilter IProjectItemRuleGroupConfiguration.InProject => InProject;
    IReadOnlyCollection<IRuleConfiguration> IProjectItemRuleGroupConfiguration.Rules => Rules;
  }
}