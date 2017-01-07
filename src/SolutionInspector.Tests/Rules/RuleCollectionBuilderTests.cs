using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using Fasterflect;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Api.Configuration;
using SolutionInspector.Api.Configuration.Ruleset;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;
using SolutionInspector.Internals;
using SolutionInspector.Rules;
using SolutionInspector.TestInfrastructure;

namespace SolutionInspector.Tests.Rules
{
  public class RuleCollectionBuilderTests
  {
    private IRuleConfigurationInstantiator _ruleConfigurationInstantiator;
    private IRuleTypeResolver _ruleTypeResolver;

    private IRuleCollectionBuilder _sut;

    [SetUp]
    public void SetUp ()
    {
      _ruleTypeResolver = A.Fake<IRuleTypeResolver>();
      _ruleConfigurationInstantiator = A.Fake<IRuleConfigurationInstantiator>();

      A.CallTo(() => _ruleTypeResolver.Resolve(A<string>._))
          .Returns(new RuleTypeInfo(typeof(Rule), null, typeof(Rule).GetConstructors().Single()));

      _sut = new RuleCollectionBuilder(_ruleTypeResolver, _ruleConfigurationInstantiator);
    }

    [Test]
    public void Build_BuildsRuleProxies ()
    {
      var rulesConfiguration = A.Fake<IRulesConfiguration>();

      var appliesToNameFilter1 = A.Fake<INameFilter>();
      var appliesToNameFilter2 = A.Fake<INameFilter>();
      var inProjectFilter = A.Fake<INameFilter>();

      var rules =
          new[]
          {
            FakeHelper.CreateAndConfigure<IRuleConfiguration>(
              r => { A.CallTo(() => r.RuleType).Returns("Namespace.RuleName, Assembly"); })
          };

      A.CallTo(() => rulesConfiguration.SolutionRules).Returns(rules);

      A.CallTo(() => rulesConfiguration.ProjectRuleGroups).Returns(
        new[]
        {
          FakeHelper.CreateAndConfigure<IProjectRuleGroupConfiguration>(
            c =>
            {
              A.CallTo(() => c.AppliesTo).Returns(appliesToNameFilter1);
              A.CallTo(() => c.Rules).Returns(rules);
            })
        });

      A.CallTo(() => rulesConfiguration.ProjectItemRuleGroups).Returns(
        new[]
        {
          FakeHelper.CreateAndConfigure<IProjectItemRuleGroupConfiguration>(
            c =>
            {
              A.CallTo(() => c.AppliesTo).Returns(appliesToNameFilter2);
              A.CallTo(() => c.InProject).Returns(inProjectFilter);
              A.CallTo(() => c.Rules).Returns(rules);
            })
        });

      // ACT
      var result = _sut.Build(rulesConfiguration);

      // ASSERT
      result.SolutionRules.Single().Should().BeOfType<Rule>();
      AssertProjectRuleProxy<Rule>(result.ProjectRules.First(), appliesToNameFilter1);
      AssertProjectItemRuleProxy<Rule>(result.ProjectItemRules.Single(), appliesToNameFilter2, inProjectFilter);

      A.CallTo(() => _ruleTypeResolver.Resolve("Namespace.RuleName, Assembly")).MustHaveHappened(Repeated.Exactly.Times(3));
    }

    private void AssertProjectRuleProxy<TRule> (IRule rule, INameFilter expectedNameFilter)
    {
      var projectRuleProxyType = typeof(FilteringProjectRuleProxy);
      rule.Should().BeOfType(projectRuleProxyType);

      rule.GetFieldValue("_rule").Should().BeOfType<TRule>();

      var filter = rule.GetFieldValue("_filter");
      filter.Should().BeSameAs(expectedNameFilter);
    }

    private void AssertProjectItemRuleProxy<TRule> (IRule rule, INameFilter expectedAppliesToFilter, INameFilter expectedInProjectFilter)
    {
      var projectRuleProxyType = typeof(FilteringProjectItemRuleProxy);
      rule.Should().BeOfType(projectRuleProxyType);

      rule.GetFieldValue("_rule").Should().BeOfType<TRule>();

      var appliesToFilter = rule.GetFieldValue("_appliesTo");
      appliesToFilter.Should().BeSameAs(expectedAppliesToFilter);

      var inProjectFilter = rule.GetFieldValue("_inProject");
      inProjectFilter.Should().BeSameAs(expectedInProjectFilter);
    }

    private class Rule : IProjectRule, ISolutionRule, IProjectItemRule
    {
      public IEnumerable<IRuleViolation> Evaluate (IProjectItem target)
      {
        return Enumerable.Empty<RuleViolation>();
      }

      public IEnumerable<IRuleViolation> Evaluate (IProject target)
      {
        return Enumerable.Empty<RuleViolation>();
      }

      public IEnumerable<IRuleViolation> Evaluate (ISolution target)
      {
        return Enumerable.Empty<RuleViolation>();
      }
    }
  }
}