using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using SolutionInspector.Configuration;

namespace SolutionInspector.Api.Configuration.Ruleset
{
  /// <summary>
  ///   Configuration for all (solution, project and project item) rules of a ruleset.
  /// </summary>
  public interface IRulesConfiguration
  {
    IReadOnlyCollection<IRuleConfiguration> SolutionRules { get; }
    IReadOnlyCollection<IProjectRuleGroupConfiguration> ProjectRuleGroups { get; }
    IReadOnlyCollection<IProjectItemRuleGroupConfiguration> ProjectItemRuleGroups { get; }
  }

  /// <inheritdoc cref="IRulesConfiguration" />
  /// >
  public class RulesConfigurationElement : ConfigurationElement, IRulesConfiguration
  {
    [ConfigurationCollection (ElementName = "rule", IsOptional = true)]
    public ConfigurationElementCollection<RuleConfigurationElement> SolutionRules => GetConfigurationCollection<RuleConfigurationElement>();

    [ConfigurationCollection (CollectionName = "projectRules", ElementName = "projectRuleGroup", IsOptional = true)]
    public ConfigurationElementCollection<ProjectRuleGroupConfigurationElement> ProjectRuleGroups
      => GetConfigurationCollection<ProjectRuleGroupConfigurationElement>();

    [ConfigurationCollection (CollectionName = "projectItemRules", ElementName = "projectItemRuleGroup", IsOptional = true)]
    public ConfigurationElementCollection<ProjectItemRuleGroupConfigurationElement> ProjectItemRuleGroups
      => GetConfigurationCollection<ProjectItemRuleGroupConfigurationElement>();

    [ExcludeFromCodeCoverage]
    IReadOnlyCollection<IRuleConfiguration> IRulesConfiguration.SolutionRules => SolutionRules;

    [ExcludeFromCodeCoverage]
    IReadOnlyCollection<IProjectRuleGroupConfiguration> IRulesConfiguration.ProjectRuleGroups => ProjectRuleGroups;

    [ExcludeFromCodeCoverage]
    IReadOnlyCollection<IProjectItemRuleGroupConfiguration> IRulesConfiguration.ProjectItemRuleGroups => ProjectItemRuleGroups;
  }
}