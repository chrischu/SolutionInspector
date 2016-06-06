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
  [Subject (typeof(FilteringProjectRuleProxy))]
  class FilteringProjectRuleProxySpec
  {
    static INameFilter ProjectNameFilter;
    static IRuleViolation ProjectRuleViolation;
    static IProjectRule ProjectRule;

    static FilteringProjectRuleProxy SUT;

    static IProject Project;
    static string ProjectName;

    Establish ctx = () =>
    {
      ProjectNameFilter = A.Fake<INameFilter>();

      ProjectRule = A.Fake<IProjectRule>();
      ProjectRuleViolation = A.Fake<IRuleViolation>();
      A.CallTo(() => ProjectRule.Evaluate(A<IProject>._)).Returns(new[] { ProjectRuleViolation });

      SUT = new FilteringProjectRuleProxy(ProjectNameFilter, ProjectRule);

      Project = A.Fake<IProject>();
      ProjectName = Some.String();
      A.CallTo(() => Project.Name).Returns(ProjectName);
    };

    class when_evaluating_and_project_name_matches
    {
      Establish ctx = () => { A.CallTo(() => ProjectNameFilter.IsMatch(A<string>._)).Returns(true); };

      Because of = () => Result = SUT.Evaluate(Project);

      It checks_project_name_filter = () =>
          A.CallTo(() => ProjectNameFilter.IsMatch(ProjectName)).MustHaveHappened();

      It calls_inner_rule = () =>
          A.CallTo(() => ProjectRule.Evaluate(Project)).MustHaveHappened();

      It passes_on_violations_from_inner_rule = () =>
          Result.Single().Should().BeSameAs(ProjectRuleViolation);

      static IEnumerable<IRuleViolation> Result;
    }

    class when_evaluating_and_project_name_does_not_match
    {
      Establish ctx = () => { A.CallTo(() => ProjectNameFilter.IsMatch(A<string>._)).Returns(false); };

      Because of = () => Result = SUT.Evaluate(Project);

      It checks_project_name_filter = () =>
          A.CallTo(() => ProjectNameFilter.IsMatch(ProjectName)).MustHaveHappened();

      It does_not_call_inner_rule = () =>
          A.CallTo(() => ProjectRule.Evaluate(Project)).MustNotHaveHappened();

      It returns_no_violations = () =>
          Result.Should().BeEmpty();

      static IEnumerable<IRuleViolation> Result;
    }
  }
}