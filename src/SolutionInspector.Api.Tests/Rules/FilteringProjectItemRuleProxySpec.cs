using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;
using SolutionInspector.Api.Utilities;
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

namespace SolutionInspector.Api.Tests.Rules
{
  [Subject (typeof(FilteringProjectItemRuleProxy))]
  class FilteringProjectItemRuleProxySpec
  {
    static INameFilter ProjectNameFilter;
    static INameFilter ProjectItemNameFilter;
    static IRuleViolation ProjectItemRuleViolation;
    static IProjectItemRule ProjectItemRule;

    static FilteringProjectItemRuleProxy SUT;

    static IProjectItem ProjectItem;
    static string ProjectItemName;
    static string ProjectName;

    Establish ctx = () =>
    {
      ProjectNameFilter = A.Fake<INameFilter>();
      ProjectItemNameFilter = A.Fake<INameFilter>();

      ProjectItemRule = A.Fake<IProjectItemRule>();
      ProjectItemRuleViolation = A.Fake<IRuleViolation>();
      A.CallTo(() => ProjectItemRule.Evaluate(A<IProjectItem>._)).Returns(new[] { ProjectItemRuleViolation });

      SUT = new FilteringProjectItemRuleProxy(ProjectItemNameFilter, ProjectNameFilter, ProjectItemRule);

      ProjectItem = A.Fake<IProjectItem>();
      ProjectItemName = Some.String();
      A.CallTo(() => ProjectItem.Name).Returns(ProjectItemName);

      var project = A.Fake<IProject>();
      ProjectName = Some.String();
      A.CallTo(() => project.Name).Returns(ProjectName);

      A.CallTo(() => ProjectItem.Project).Returns(project);
    };

    class when_evaluating_and_all_filters_match
    {
      Establish ctx = () =>
      {
        A.CallTo(() => ProjectNameFilter.IsMatch(A<string>._)).Returns(true);
        A.CallTo(() => ProjectItemNameFilter.IsMatch(A<string>._)).Returns(true);
      };

      Because of = () => Result = SUT.Evaluate(ProjectItem);

      It checks_project_name_filter = () =>
          A.CallTo(() => ProjectNameFilter.IsMatch(ProjectName)).MustHaveHappened();

      It checks_project_item_name_filter = () =>
          A.CallTo(() => ProjectItemNameFilter.IsMatch(ProjectItemName)).MustHaveHappened();

      It calls_inner_rule = () =>
          A.CallTo(() => ProjectItemRule.Evaluate(ProjectItem)).MustHaveHappened();

      It passes_on_violations_from_inner_rule = () =>
          Result.Single().Should().BeSameAs(ProjectItemRuleViolation);

      static IEnumerable<IRuleViolation> Result;
    }

    class when_evaluating_and_project_filter_does_not_match
    {
      Establish ctx = () => { A.CallTo(() => ProjectNameFilter.IsMatch(A<string>._)).Returns(false); };

      Because of = () => Result = SUT.Evaluate(ProjectItem);

      It checks_project_name_filter = () =>
          A.CallTo(() => ProjectNameFilter.IsMatch(ProjectName)).MustHaveHappened();

      It does_not_check_project_item_name_filter = () =>
          A.CallTo(() => ProjectItemNameFilter.IsMatch(ProjectItemName)).MustNotHaveHappened();


      It does_not_call_inner_rule = () =>
          A.CallTo(() => ProjectItemRule.Evaluate(ProjectItem)).MustNotHaveHappened();

      It returns_no_violations = () =>
          Result.Should().BeEmpty();

      static IEnumerable<IRuleViolation> Result;
    }

    class when_evaluating_and_project_item_filter_does_not_match
    {
      Establish ctx = () =>
      {
        A.CallTo(() => ProjectNameFilter.IsMatch(A<string>._)).Returns(true);
        A.CallTo(() => ProjectItemNameFilter.IsMatch(A<string>._)).Returns(false);
      };

      Because of = () => Result = SUT.Evaluate(ProjectItem);

      It checks_project_name_filter = () =>
          A.CallTo(() => ProjectNameFilter.IsMatch(ProjectName)).MustHaveHappened();

      It checks_project_item_name_filter = () =>
          A.CallTo(() => ProjectItemNameFilter.IsMatch(ProjectItemName)).MustHaveHappened();

      It does_not_call_inner_rule = () =>
          A.CallTo(() => ProjectItemRule.Evaluate(ProjectItem)).MustNotHaveHappened();

      It returns_no_violations = () =>
          Result.Should().BeEmpty();

      static IEnumerable<IRuleViolation> Result;
    }
  }
}