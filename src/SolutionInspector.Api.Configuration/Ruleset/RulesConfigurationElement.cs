using System.Collections.Generic;
using SolutionInspector.Configuration;

namespace SolutionInspector.Api.Configuration.Ruleset
{
  public interface IRulesConfiguration
  {
    IReadOnlyCollection<IRuleConfiguration> SolutionRules { get; }
    IReadOnlyCollection<IProjectRuleGroupConfiguration> ProjectRuleGroups { get; }
    IReadOnlyCollection<IProjectItemRuleGroupConfiguration> ProjectItemRuleGroups { get; }
  }

  public class RulesConfigurationElement : ConfigurationElement, IRulesConfiguration
  {
    [ConfigurationCollection (ElementName = "rule", IsOptional = true)]
    public ConfigurationElementCollection<RuleConfigurationElement> SolutionRules => GetConfigurationCollection<RuleConfigurationElement>();

    [ConfigurationCollection (CollectionName = "projectRules", ElementName = "projectRuleGroup", IsOptional = true)]
    public ConfigurationElementCollection<ProjectRuleGroupConfigurationElement> ProjectRuleGroups
        => GetConfigurationCollection<ProjectRuleGroupConfigurationElement>();

    [ConfigurationCollection(CollectionName = "projectItemRules", ElementName = "projectItemRuleGroup", IsOptional = true)]
    public ConfigurationElementCollection<ProjectItemRuleGroupConfigurationElement> ProjectItemRuleGroups
        => GetConfigurationCollection<ProjectItemRuleGroupConfigurationElement>();

    IReadOnlyCollection<IRuleConfiguration> IRulesConfiguration.SolutionRules => SolutionRules;
    IReadOnlyCollection<IProjectRuleGroupConfiguration> IRulesConfiguration.ProjectRuleGroups => ProjectRuleGroups;
    IReadOnlyCollection<IProjectItemRuleGroupConfiguration> IRulesConfiguration.ProjectItemRuleGroups => ProjectItemRuleGroups;
  }
}