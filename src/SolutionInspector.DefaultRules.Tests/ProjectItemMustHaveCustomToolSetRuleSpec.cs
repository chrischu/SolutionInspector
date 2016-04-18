using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.ObjectModel;
using SolutionInspector.Rules;
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
  [Subject(typeof (ProjectItemMustHaveCustomToolSetRule))]
  class ProjectItemMustHaveCustomToolSetRuleSpec
  {
    static IProjectItem ProjectItem;

    static ProjectItemMustHaveCustomToolSetRule SUT;

    Establish ctx = () =>
    {
      ProjectItem = A.Fake<IProjectItem>();

      SUT = new ProjectItemMustHaveCustomToolSetRule(
          new ProjectItemMustHaveCustomToolSetRuleConfiguration
          {
              ExpectedCustomTool = "CustomTool",
              ExpectedCustomToolNamespace= "CustomToolNamespace"
          });
    };

    class when_evaluating_project_item_with_correct_values
    {
      Establish ctx = () =>
      {
        A.CallTo(() => ProjectItem.CustomTool).Returns("CustomTool");
        A.CallTo(() => ProjectItem.CustomToolNamespace).Returns("CustomToolNamespace");
      };

      Because of = () => Result = SUT.Evaluate(ProjectItem);

      It does_not_return_violations = () =>
          Result.Should().BeEmpty();

      static IEnumerable<IRuleViolation> Result;
    }

    class when_evaluating_project_item_with_incorrect_values
    {
      Establish ctx = () =>
      {
        A.CallTo(() => ProjectItem.CustomTool).Returns("DIFFERENT");
        A.CallTo(() => ProjectItem.CustomToolNamespace).Returns("DIFFERENT_NS");
      };

      Because of = () => Result = SUT.Evaluate(ProjectItem);

      It returns_violation = () =>
          Result.ShouldAllBeEquivalentTo(
              new RuleViolation(SUT, ProjectItem, "Unexpected value for custom tool, was 'DIFFERENT' but should be 'CustomTool'."),
              new RuleViolation(SUT, ProjectItem, "Unexpected value for custom tool namespace, was 'DIFFERENT_NS' but should be 'CustomToolNamespace'."));

      static IEnumerable<IRuleViolation> Result;
    }
  }
}