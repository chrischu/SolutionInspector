using System;
using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;

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
  [Subject (typeof(ProjectShouldNotContainProjectItemsWithDuplicateIncludesRule))]
  class ProjectShouldNotContainProjectItemsWithDuplicateIncludesRuleSpec
  {
    static IProject Project;

    static ProjectShouldNotContainProjectItemsWithDuplicateIncludesRule SUT;

    Establish ctx = () =>
    {
      Project = A.Fake<IProject>();

      SUT = new ProjectShouldNotContainProjectItemsWithDuplicateIncludesRule();
    };

    class when_evaluating_and_there_are_no_duplicates
    {
      Establish ctx = () => { A.CallTo(() => Project.ProjectItems).Returns(new[] { FakeProjectItem("One"), FakeProjectItem("Two") }); };

      Because of = () => Result = SUT.Evaluate(Project);

      It does_not_return_violations = () =>
          Result.Should().BeEmpty();

      static IEnumerable<IRuleViolation> Result;
    }

    class when_evaluating_and_there_are_duplicates
    {
      Establish ctx =
          () =>
          {
            A.CallTo(() => Project.ProjectItems)
                .Returns(new[] { FakeProjectItem("One", "1"), FakeProjectItem("One", "2"), FakeProjectItem("One", "3") });
          };

      Because of = () => Result = SUT.Evaluate(Project);

      It returns_violation = () =>
          Result.ShouldAllBeEquivalentTo(
              new RuleViolation(SUT, Project, "There are multiple project items with include 'One' in the following locations: 1, 2, 3."));

      static IEnumerable<IRuleViolation> Result;
    }

    static IProjectItem FakeProjectItem (string include, string location = null)
    {
      var projectItem = A.Fake<IProjectItem>();

      var projectItemInclude = A.Fake<IProjectItemInclude>();
      A.CallTo(() => projectItemInclude.Evaluated).Returns(include);
      A.CallTo(() => projectItem.OriginalInclude).Returns(projectItemInclude);

      if (location != null)
      {
        var projectItemLocation = A.Fake<IProjectLocation>();
        A.CallTo(() => projectItemLocation.ToString()).Returns(location);
        A.CallTo(() => projectItem.Location).Returns(projectItemLocation);
      }

      return projectItem;
    }
  }
}