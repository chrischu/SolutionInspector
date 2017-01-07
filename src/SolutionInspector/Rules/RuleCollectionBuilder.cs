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
    private readonly IRuleConfigurationInstantiator _ruleConfigurationInstantiator;
    private readonly IRuleTypeResolver _ruleTypeResolver;

    public RuleCollectionBuilder (
      IRuleTypeResolver ruleTypeResolver,
      IRuleConfigurationInstantiator ruleConfigurationInstantiator)
    {
      _ruleTypeResolver = ruleTypeResolver;
      _ruleConfigurationInstantiator = ruleConfigurationInstantiator;
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
          let ruleTypeInfo = _ruleTypeResolver.Resolve(ruleConfiguration.RuleType)
          let config = _ruleConfigurationInstantiator.Instantiate(ruleTypeInfo.ConfigurationType, ruleConfiguration.Element)
          let constructorParameters = ruleTypeInfo.IsConfigurable ? new object[] { config } : new object[0]
          select (TRule) ruleTypeInfo.Constructor.Invoke(constructorParameters);
    }
  }
}