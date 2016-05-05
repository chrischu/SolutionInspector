using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace SolutionInspector.Api.Configuration.Rules
{
  /// <summary>
  /// Configuration for all the <see cref="SolutionRules"/>, <see cref="ProjectRuleGroups"/> and <see cref="ProjectItemRuleGroups"/>.
  /// </summary>
  public interface IRulesConfiguration
  {
    /// <summary>
    /// All configured solution rules.
    /// </summary>
    IReadOnlyCollection<IRuleConfiguration> SolutionRules { get; }

    /// <summary>
    /// All configured project rule groups.
    /// </summary>
    IReadOnlyCollection<IProjectRuleGroupConfiguration> ProjectRuleGroups { get; }

    /// <summary>
    /// All configured project item rule groups.
    /// </summary>
    IReadOnlyCollection<IProjectItemRuleGroupConfiguration> ProjectItemRuleGroups { get; }
  }

  internal sealed class RulesConfigurationSection : ConfigurationSection, IRulesConfiguration
  {
    public const string ExampleConfiguration = @"<rules>
  <solutionRules>
    <rule type=""Namespace.Rule, Assembly"" property=""Property"">
      <innerConfig property=""InnerProperty"" />
    </rule>
  </solutionRules>
  <projectRules>
    <projectRuleGroup appliesTo=""*"">
      <rule type=""Namespace.Rule, Assembly"" property=""Property"">
        <innerConfig property=""InnerProperty"" />
      </rule>
    </projectRuleGroup>
    <projectRuleGroup appliesTo=""+Inc*lude;-Exc*lude"">
      <rule type=""Namespace.Rule, Assembly"" property=""Property"">
        <innerConfig property=""InnerProperty"" />
      </rule>
    </projectRuleGroup>
    <projectRuleGroup appliesTo=""Project"">
      <rule type=""Namespace.Rule, Assembly"" property=""Property"">
        <innerConfig property=""InnerProperty"" />
      </rule>
    </projectRuleGroup>
  </projectRules>
  <projectItemRules>
    <projectItemRuleGroup appliesTo=""App.config"" inProject=""Project"">
      <rule type=""Namespace.Rule, Assembly"" property=""Property"">
        <innerConfig property=""InnerProperty"" />
      </rule>
    </projectItemRuleGroup>
  </projectItemRules>
</rules>";

    [ConfigurationProperty("solutionRules")]
    public RuleConfigurationCollection SolutionRules => (RuleConfigurationCollection)this["solutionRules"];

    [ConfigurationProperty("projectRules")]
    public ProjectRulesConfigurationCollection ProjectRules => (ProjectRulesConfigurationCollection)this["projectRules"];

    [ConfigurationProperty("projectItemRules")]
    public ProjectItemRulesConfigurationCollection ProjectItemRules => (ProjectItemRulesConfigurationCollection)this["projectItemRules"];

    IReadOnlyCollection<IRuleConfiguration> IRulesConfiguration.SolutionRules => SolutionRules.ToArray();
    IReadOnlyCollection<IProjectRuleGroupConfiguration> IRulesConfiguration.ProjectRuleGroups => ProjectRules.ToArray();
    IReadOnlyCollection<IProjectItemRuleGroupConfiguration> IRulesConfiguration.ProjectItemRuleGroups => ProjectItemRules.ToArray();
  }
}