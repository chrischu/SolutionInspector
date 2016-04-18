using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using FakeItEasy;
using Fasterflect;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.Configuration.Rules;
using SolutionInspector.ObjectModel;
using SolutionInspector.Rules;
using SolutionInspector.TestInfrastructure;
using SolutionInspector.Utilities;

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
  [Subject(typeof (RuleCollectionBuilder))]
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
          .Returns(new RuleTypeInfo(typeof (Rule), null, typeof (Rule).GetConstructors().Single()));

      SUT = new RuleCollectionBuilder(RuleTypeResolver, RuleConfigurationInstantiator);
    };

    class when_building
    {
      Establish ctx = () =>
      {
        RulesConfiguration = A.Fake<IRulesConfiguration>();

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
                      A.CallTo(() => c.AppliesTo).Returns(new NameFilter(new[] { "*" }));
                      A.CallTo(() => c.Rules).Returns(rules);
                    }),
                FakeHelper.CreateAndConfigure<IProjectRuleGroupConfiguration>(
                    c =>
                    {
                      A.CallTo(() => c.AppliesTo).Returns(new NameFilter(new[] { "Include" }, new[] { "Exclude" }));
                      A.CallTo(() => c.Rules).Returns(rules);
                    }),
                FakeHelper.CreateAndConfigure<IProjectRuleGroupConfiguration>(
                    c =>
                    {
                      A.CallTo(() => c.AppliesTo).Returns(new NameFilter(new[] { "Project" }));
                      A.CallTo(() => c.Rules).Returns(rules);
                    }),
                FakeHelper.CreateAndConfigure<IProjectRuleGroupConfiguration>(
                    c =>
                    {
                      A.CallTo(() => c.AppliesTo).Returns(new NameFilter(new[] { "Filter*" }));
                      A.CallTo(() => c.Rules).Returns(rules);
                    })
            });

        A.CallTo(() => RulesConfiguration.ProjectItemRuleGroups).Returns(
            new[]
            {
                FakeHelper.CreateAndConfigure<IProjectItemRuleGroupConfiguration>(
                    c =>
                    {
                      A.CallTo(() => c.AppliesTo).Returns(new NameFilter(new[] { "*" }));
                      A.CallTo(() => c.InProject).Returns(new NameFilter(new[] { "*" }));
                      A.CallTo(() => c.Rules).Returns(rules);
                    })
            });
      };

      Because of = () => Result = SUT.Build(RulesConfiguration);

      It calls_RuleTypeResolver = () =>
          A.CallTo(() => RuleTypeResolver.Resolve("Namespace.RuleName, Assembly")).MustHaveHappened(Repeated.Exactly.Times(6));

      It builds_solution_rule = () =>
          Result.SolutionRules.Single().Should().BeOfType<Rule>();

      It builds_project_rule_that_applies_to_all = () =>
          AssertProjectRuleProxy<Rule>(Result.ProjectRules.First(), ".*");

      It builds_project_rule_that_applies_to_group = () =>
          AssertProjectRuleProxy<Rule>(Result.ProjectRules.Skip(1).First(), "Include", "Exclude");

      It builds_project_rule_that_applies_to_project = () =>
          AssertProjectRuleProxy<Rule>(Result.ProjectRules.Skip(2).First(), "Project");

      It builds_project_rule_that_applies_to_filter = () =>
          AssertProjectRuleProxy<Rule>(Result.ProjectRules.Skip(3).First(), "Filter.*");

      It builds_project_item_rule = () =>
          AssertProjectItemRuleProxy<Rule>(Result.ProjectItemRules.Single(), ".*", ".*");

      static IRulesConfiguration RulesConfiguration;
      static IRuleCollection Result;
    }

    static void AssertProjectRuleProxy<TRule>(IRule rule, string include, string exclude = null)
    {
      var projectRuleProxyType =
          typeof (RuleCollectionBuilder).GetNestedTypes(BindingFlags.NonPublic).Single(t => t.Name == "FilteringProjectRuleProxy");
      rule.Should().BeOfType(projectRuleProxyType);

      rule.GetFieldValue("_rule").Should().BeOfType<TRule>();

      var filter = rule.GetFieldValue("_filter");

      var includes = (Regex[]) filter.GetFieldValue("_includeFilters");
      includes.Select(r => (string) r.GetFieldValue("pattern")).Should().BeEquivalentTo($"^{include}$");

      if (exclude != null)
      {
        var excludes = (Regex[]) filter.GetFieldValue("_excludeFilters");
        excludes.Select(r => (string) r.GetFieldValue("pattern")).Should().BeEquivalentTo($"^{exclude}$");
      }
    }

    static void AssertProjectItemRuleProxy<TRule>(IRule rule, string appliesTo, string inProject)
    {
      var projectRuleProxyType =
          typeof (RuleCollectionBuilder).GetNestedTypes(BindingFlags.NonPublic).Single(t => t.Name == "FilteringProjectItemRuleProxy");
      rule.Should().BeOfType(projectRuleProxyType);

      rule.GetFieldValue("_rule").Should().BeOfType<TRule>();

      var appliesToFilter = rule.GetFieldValue("_appliesTo");
      var includes = (Regex[]) appliesToFilter.GetFieldValue("_includeFilters");
      includes.Select(r => (string) r.GetFieldValue("pattern")).Should().BeEquivalentTo($"^{appliesTo}$");

      var inProjectFilter = rule.GetFieldValue("_inProject");
      includes = (Regex[]) inProjectFilter.GetFieldValue("_includeFilters");
      includes.Select(r => (string) r.GetFieldValue("pattern")).Should().BeEquivalentTo($"^{inProject}$");
    }

    class Rule : IProjectRule, ISolutionRule, IProjectItemRule
    {
      public IEnumerable<IRuleViolation> Evaluate(IProject target)
      {
        return Enumerable.Empty<RuleViolation>();
      }

      public IEnumerable<IRuleViolation> Evaluate(ISolution target)
      {
        return Enumerable.Empty<RuleViolation>();
      }

      public IEnumerable<IRuleViolation> Evaluate(IProjectItem target)
      {
        return Enumerable.Empty<RuleViolation>();
      }
    }
  }
}