using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using FakeItEasy;
using Fasterflect;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Api;
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
    private IRuleInstantiator _ruleInstantiator;

    private IRuleCollectionBuilder _sut;

    [SetUp]
    public void SetUp ()
    {
      _ruleInstantiator = A.Fake<IRuleInstantiator>();

      A.CallTo(() => _ruleInstantiator.Instantiate(A<string>._, A<XElement>._)).Returns(new Rule());

      _sut = new RuleCollectionBuilder(_ruleInstantiator);
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

      A.CallTo(() => _ruleInstantiator.Instantiate("Namespace.RuleName, Assembly", A<XElement>._)).MustHaveHappened(Repeated.Exactly.Times(3));
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

    private class Rule : RuleConfigurationElement, ISolutionRule, IProjectRule, IProjectItemRule
    {
      public IEnumerable<IRuleViolation> Evaluate (ISolution target)
      {
        return Enumerable.Empty<RuleViolation>();
      }

      public IEnumerable<IRuleViolation> Evaluate (IProject target)
      {
        return Enumerable.Empty<RuleViolation>();
      }

      public IEnumerable<IRuleViolation> Evaluate (IProjectItem target)
      {
        return Enumerable.Empty<RuleViolation>();
      }
    }
  }
}