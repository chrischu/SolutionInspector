using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using SolutionInspector.Commons.Attributes;
using SolutionInspector.Configuration;

namespace SolutionInspector.Api.Configuration.Ruleset
{
  /// <summary>
  ///   Configuration for all (solution, project and project item) rules of a ruleset.
  /// </summary>
  [PublicApi]
  public interface IRulesConfiguration
  {
    /// <summary>
    ///   The collection of solution rules in the <see cref="IRulesConfiguration" />.
    /// </summary>
    IReadOnlyCollection<IRuleConfiguration> SolutionRules { get; }

    /// <summary>
    ///   The collection of project rules in the <see cref="IRulesConfiguration" />.
    /// </summary>
    IReadOnlyCollection<IProjectRuleGroupConfiguration> ProjectRuleGroups { get; }

    /// <summary>
    ///   The collection of project item rules in the <see cref="IRulesConfiguration" />.
    /// </summary>
    IReadOnlyCollection<IProjectItemRuleGroupConfiguration> ProjectItemRuleGroups { get; }
  }

  /// <inheritdoc cref="IRulesConfiguration" />
  [UsedImplicitly]
  public class RulesConfigurationElement : ConfigurationElement, IRulesConfiguration
  {
    /// <see cref="IRulesConfiguration.SolutionRules" />
    [ConfigurationCollection(ElementName = "rule", IsOptional = true)]
    public ConfigurationElementCollection<RuleConfigurationElement> SolutionRules => GetConfigurationCollection<RuleConfigurationElement>();

    /// <see cref="IRulesConfiguration.ProjectRuleGroups" />
    [ConfigurationCollection(CollectionName = "projectRules", ElementName = "projectRuleGroup", IsOptional = true)]
    public ConfigurationElementCollection<ProjectRuleGroupConfigurationElement> ProjectRuleGroups
      => GetConfigurationCollection<ProjectRuleGroupConfigurationElement>();

    /// <see cref="IRulesConfiguration.ProjectItemRuleGroups" />
    [ConfigurationCollection(CollectionName = "projectItemRules", ElementName = "projectItemRuleGroup", IsOptional = true)]
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