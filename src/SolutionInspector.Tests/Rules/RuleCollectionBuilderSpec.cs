using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using Fasterflect;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.Api.Configuration;
using SolutionInspector.Api.Configuration.Ruleset;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;
using SolutionInspector.Internals;
using SolutionInspector.Rules;
using SolutionInspector.TestInfrastructure;

#region R# preamble for Machine.Specifications files

// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable ClassNeverInstantiated.Local
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Local
// ReSharper disable NotAccessedField.Local
// ReSharper disable StaticMemberInGenericType
// ReSharper disable UnassignedField.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable UnassignedGetOnlyAutoProperty

#endregion

namespace SolutionInspector.Tests.Rules
{
  [Subject (typeof(RuleCollectionBuilder))]
  class RuleCollectionBuilderSpec
  {
    static IRuleTypeResolver RuleTypeResolver;
    static IRuleConfigurationInstantiator RuleConfigurationInstantiator;

    static IRuleCollectionBuilder SUT;

    Establish ctx = () =>
    {
      RuleTypeResolver = A.Fake<IRuleTypeResolver>();
      RuleConfigurationInstantiator = A.Fake<IRuleConfigurationInstantiator>();

      A.CallTo(() => RuleTypeResolver.Resolve(A<string>._))
          .Returns(new RuleTypeInfo(typeof(Rule), null, typeof(Rule).GetConstructors().Single()));

      SUT = new RuleCollectionBuilder(RuleTypeResolver, RuleConfigurationInstantiator);
    };

    class when_building
    {
      Establish ctx = () =>
      {
        RulesConfiguration = A.Fake<IRulesConfiguration>();

        AppliesToNameFilter1 = A.Fake<INameFilter>();
        AppliesToNameFilter2 = A.Fake<INameFilter>();
        InProjectFilter = A.Fake<INameFilter>();

        var rules =
            new[]
            {
                FakeHelper.CreateAndConfigure<IRuleConfiguration>(
                    r => { A.CallTo(() => r.RuleType).Returns("Namespace.RuleName, Assembly"); })
            };

        A.CallTo(() => RulesConfiguration.SolutionRules).Returns(rules);

        A.CallTo(() => RulesConfiguration.ProjectRuleGroups).Returns(
            new[]
            {
                FakeHelper.CreateAndConfigure<IProjectRuleGroupConfiguration>(
                    c =>
                    {
                      A.CallTo(() => c.AppliesTo).Returns(AppliesToNameFilter1);
                      A.CallTo(() => c.Rules).Returns(rules);
                    })
            });

        A.CallTo(() => RulesConfiguration.ProjectItemRuleGroups).Returns(
            new[]
            {
                FakeHelper.CreateAndConfigure<IProjectItemRuleGroupConfiguration>(
                    c =>
                    {
                      A.CallTo(() => c.AppliesTo).Returns(AppliesToNameFilter2);
                      A.CallTo(() => c.InProject).Returns(InProjectFilter);
                      A.CallTo(() => c.Rules).Returns(rules);
                    })
            });
      };

      Because of = () => Result = SUT.Build(RulesConfiguration);

      It calls_RuleTypeResolver = () =>
          A.CallTo(() => RuleTypeResolver.Resolve("Namespace.RuleName, Assembly")).MustHaveHappened(Repeated.Exactly.Times(3));

      It builds_solution_rule = () =>
          Result.SolutionRules.Single().Should().BeOfType<Rule>();

      It builds_project_rule_that_applies_to_all = () =>
          AssertProjectRuleProxy<Rule>(Result.ProjectRules.First(), AppliesToNameFilter1);

      It builds_project_item_rule = () =>
          AssertProjectItemRuleProxy<Rule>(Result.ProjectItemRules.Single(), AppliesToNameFilter2, InProjectFilter);

      static IRulesConfiguration RulesConfiguration;
      static INameFilter AppliesToNameFilter1;
      static INameFilter AppliesToNameFilter2;
      static INameFilter InProjectFilter;
      static IRuleCollection Result;
    }

    static void AssertProjectRuleProxy<TRule> (IRule rule, INameFilter expectedNameFilter)
    {
      var projectRuleProxyType = typeof(FilteringProjectRuleProxy);
      rule.Should().BeOfType(projectRuleProxyType);

      rule.GetFieldValue("_rule").Should().BeOfType<TRule>();

      var filter = rule.GetFieldValue("_filter");
      filter.Should().BeSameAs(expectedNameFilter);
    }

    static void AssertProjectItemRuleProxy<TRule> (IRule rule, INameFilter expectedAppliesToFilter, INameFilter expectedInProjectFilter)
    {
      var projectRuleProxyType = typeof(FilteringProjectItemRuleProxy);
      rule.Should().BeOfType(projectRuleProxyType);

      rule.GetFieldValue("_rule").Should().BeOfType<TRule>();

      var appliesToFilter = rule.GetFieldValue("_appliesTo");
      appliesToFilter.Should().BeSameAs(expectedAppliesToFilter);

      var inProjectFilter = rule.GetFieldValue("_inProject");
      inProjectFilter.Should().BeSameAs(expectedInProjectFilter);
    }

    class Rule : IProjectRule, ISolutionRule, IProjectItemRule
    {
      public IEnumerable<IRuleViolation> Evaluate (IProject target)
      {
        return Enumerable.Empty<RuleViolation>();
      }

      public IEnumerable<IRuleViolation> Evaluate (ISolution target)
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