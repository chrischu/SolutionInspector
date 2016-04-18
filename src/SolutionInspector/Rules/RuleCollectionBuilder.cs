using System.Collections.Generic;
using System.Linq;
using SolutionInspector.Configuration.Rules;
using SolutionInspector.ObjectModel;
using SolutionInspector.Utilities;

namespace SolutionInspector.Rules
{
  internal interface IRuleCollectionBuilder
  {
    IRuleCollection Build(IRulesConfiguration rulesConfiguration);
  }

  internal class RuleCollectionBuilder : IRuleCollectionBuilder
  {
    private readonly IRuleTypeResolver _ruleTypeResolver;
    private readonly IRuleConfigurationInstantiator _ruleConfigurationInstantiator;

    public RuleCollectionBuilder(
        IRuleTypeResolver ruleTypeResolver,
        IRuleConfigurationInstantiator ruleConfigurationInstantiator)
    {
      _ruleTypeResolver = ruleTypeResolver;
      _ruleConfigurationInstantiator = ruleConfigurationInstantiator;
    }

    public IRuleCollection Build(IRulesConfiguration rulesConfiguration)
    {
      var solutionRules = BuildSolutionRules(rulesConfiguration.SolutionRules);
      var projectRules = BuildProjectRules(rulesConfiguration.ProjectRuleGroups);
      var projectItemRules = BuildProjectItemRules(rulesConfiguration.ProjectItemRuleGroups);

      return new RuleCollection(solutionRules, projectRules, projectItemRules);
    }

    private IEnumerable<ISolutionRule> BuildSolutionRules(IReadOnlyCollection<IRuleConfiguration> solutionRulesConfiguration)
    {
      return InstantiateRules<ISolutionRule>(solutionRulesConfiguration);
    }

    private IEnumerable<IProjectRule> BuildProjectRules(IReadOnlyCollection<IProjectRuleGroupConfiguration> projectRuleGroups)
    {
      foreach (var projectRuleGroup in projectRuleGroups)
        foreach (var projectRule in InstantiateRules<IProjectRule>(projectRuleGroup.Rules))
          yield return new FilteringProjectRuleProxy(projectRuleGroup.AppliesTo, projectRule);
    }

    private IEnumerable<IProjectItemRule> BuildProjectItemRules(IReadOnlyCollection<IProjectItemRuleGroupConfiguration> projectItemRuleGrousp)
    {
      foreach (var projectItemRuleGroup in projectItemRuleGrousp)
        foreach (var projectItemRule in InstantiateRules<IProjectItemRule>(projectItemRuleGroup.Rules))
          yield return new FilteringProjectItemRuleProxy(projectItemRuleGroup.AppliesTo, projectItemRuleGroup.InProject, projectItemRule);
    }

    private IEnumerable<TRule> InstantiateRules<TRule>(IReadOnlyCollection<IRuleConfiguration> ruleConfigurations)
        where TRule : IRule
    {
      return from ruleConfiguration in ruleConfigurations
        let ruleTypeInfo = _ruleTypeResolver.Resolve(ruleConfiguration.RuleType)
        let config = _ruleConfigurationInstantiator.Instantiate(ruleTypeInfo.ConfigurationType, ruleConfiguration.Configuration)
        let constructorParameters = ruleTypeInfo.IsConfigurable ? new object[] { config } : new object[0]
        select (TRule) ruleTypeInfo.Constructor.Invoke(constructorParameters);
    }

    private class FilteringProjectRuleProxy : IProjectRule
    {
      private readonly INameFilter _filter;
      private readonly IProjectRule _rule;

      public FilteringProjectRuleProxy(INameFilter filter, IProjectRule rule)
      {
        _filter = filter;
        _rule = rule;
      }

      public IEnumerable<IRuleViolation> Evaluate(IProject target)
      {
        if (_filter.IsMatch(target.Name))
          return _rule.Evaluate(target);

        return Enumerable.Empty<IRuleViolation>();
      }
    }

    private class FilteringProjectItemRuleProxy : IProjectItemRule
    {
      private readonly INameFilter _appliesTo;
      private readonly INameFilter _inProject;
      private readonly IProjectItemRule _rule;

      public FilteringProjectItemRuleProxy(INameFilter appliesTo, INameFilter inProject, IProjectItemRule rule)
      {
        _appliesTo = appliesTo;
        _inProject = inProject;
        _rule = rule;
      }

      public IEnumerable<IRuleViolation> Evaluate(IProjectItem target)
      {
        if (_inProject.IsMatch(target.Project.Name) && _appliesTo.IsMatch(target.Name))
          return _rule.Evaluate(target);

        return Enumerable.Empty<IRuleViolation>();
      }
    }
  }
}