using System;
using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;
using SolutionInspector.TestInfrastructure.AssertionExtensions;

#region R# preamble for Machine.Specifications files

// ReSharper disable ArrangeTypeModifiers
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable ClassNeverInstantiated.Local
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Local
// ReSharper disable NotAccessedField.Local
// ReSharper disable StaticMemberInGenericType
// ReSharper disable UnassignedField.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable UnassignedGetOnlyAutoProperty

#endregion

namespace SolutionInspector.DefaultRules.Tests
{
  [Subject (typeof (ProjectItemMustHaveCorrectBuildActionRule))]
  class ProjectItemMustHaveCorrectBuildActionRuleSpec
  {
    static IProjectItem ProjectItem;

    static ProjectItemMustHaveCorrectBuildActionRule SUT;

    Establish ctx = () =>
    {
      ProjectItem = A.Fake<IProjectItem>();

      SUT = new ProjectItemMustHaveCorrectBuildActionRule(
          new ProjectItemMustHaveCorrectBuildActionRuleConfiguration
          {
              ExpectedBuildAction = "Compile"
          });
    };

    class when_evaluating_project_item_with_correct_build_action
    {
      Establish ctx = () => { A.CallTo(() => ProjectItem.BuildAction).Returns(ProjectItemBuildAction.Compile); };

      Because of = () => Result = SUT.Evaluate(ProjectItem);

      It does_not_return_violations = () =>
          Result.Should().BeEmpty();

      static IEnumerable<IRuleViolation> Result;
    }

    class when_evaluating_project_item_with_incorrect_values
    {
      Establish ctx = () => { A.CallTo(() => ProjectItem.BuildAction).Returns(ProjectItemBuildAction.None); };

      Because of = () => Result = SUT.Evaluate(ProjectItem);

      It returns_violation = () =>
          Result.ShouldAllBeEquivalentTo(
              new RuleViolation(SUT, ProjectItem, $"Unexpected build action was 'None', but should be 'Compile'."));

      static IEnumerable<IRuleViolation> Result;
    }
  }
}