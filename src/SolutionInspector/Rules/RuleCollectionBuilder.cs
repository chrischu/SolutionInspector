using System;
using System.Collections.Generic;
using System.Linq;
using SolutionInspector.Api.Configuration.Ruleset;
using SolutionInspector.Api.Rules;
using SolutionInspector.Internals;

namespace SolutionInspector.Rules
{
  internal interface IRuleCollectionBuilder
  {
    IRuleCollection Build (IRulesConfiguration rulesConfiguration);
  }

  internal class RuleCollectionBuilder : IRuleCollectionBuilder
  {
    private readonly IRuleInstantiator _ruleInstantiator;

    public RuleCollectionBuilder (IRuleInstantiator ruleInstantiator)
    {
      _ruleInstantiator = ruleInstantiator;
    }

    public IRuleCollection Build (IRulesConfiguration rulesConfiguration)
    {
      var solutionRules = BuildSolutionRules(rulesConfiguration.SolutionRules);
      var projectRules = BuildProjectRules(rulesConfiguration.ProjectRuleGroups);
      var projectItemRules = BuildProjectItemRules(rulesConfiguration.ProjectItemRuleGroups);

      return new RuleCollection(solutionRules, projectRules, projectItemRules);
    }

    private IEnumerable<ISolutionRule> BuildSolutionRules (IReadOnlyCollection<IRuleConfiguration> solutionRulesConfiguration)
    {
      return InstantiateRules<ISolutionRule>(solutionRulesConfiguration);
    }

    private IEnumerable<IProjectRule> BuildProjectRules (IReadOnlyCollection<IProjectRuleGroupConfiguration> projectRuleGroups)
    {
      foreach (var projectRuleGroup in projectRuleGroups)
      foreach (var projectRule in InstantiateRules<IProjectRule>(projectRuleGroup.Rules))
        yield return new FilteringProjectRuleProxy(projectRuleGroup.AppliesTo, projectRule);
    }

    private IEnumerable<IProjectItemRule> BuildProjectItemRules (IReadOnlyCollection<IProjectItemRuleGroupConfiguration> projectItemRuleGrousp)
    {
      foreach (var projectItemRuleGroup in projectItemRuleGrousp)
      foreach (var projectItemRule in InstantiateRules<IProjectItemRule>(projectItemRuleGroup.Rules))
        yield return new FilteringProjectItemRuleProxy(projectItemRuleGroup.AppliesTo, projectItemRuleGroup.InProject, projectItemRule);
    }

    private IEnumerable<TRule> InstantiateRules<TRule> (IReadOnlyCollection<IRuleConfiguration> ruleConfigurations)
        where TRule : IRule
    {
      return from ruleConfiguration in ruleConfigurations
          select (TRule) (object) _ruleInstantiator.Instantiate(ruleConfiguration.RuleType, ruleConfiguration.Element);
    }
  }
}