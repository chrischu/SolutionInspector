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
  [Subject (typeof(AllProjectItemsMustBePresentRule))]
  class AllProjectItemsMustBePresentRuleSpec
  {
    static IProjectItemInclude ProjectItemInclude;
    static IProjectItem ProjectItem;
    static IProject Project;

    static AllProjectItemsMustBePresentRule SUT;

    Establish ctx = () =>
    {
      ProjectItem = A.Fake<IProjectItem>();

      ProjectItemInclude = A.Fake<IProjectItemInclude>();
      A.CallTo(() => ProjectItemInclude.Evaluated).Returns("ProjectItem");

      A.CallTo(() => ProjectItem.OriginalInclude).Returns(ProjectItemInclude);

      Project = A.Fake<IProject>();
      A.CallTo(() => Project.ProjectItems).Returns(new[] { ProjectItem });

      SUT = new AllProjectItemsMustBePresentRule();
    };

    class when_evaluating_and_all_files_are_existing
    {
      Establish ctx = () => { A.CallTo(() => ProjectItem.File.Exists).Returns(true); };

      Because of = () => Result = SUT.Evaluate(Project);

      It does_not_return_violations = () =>
          Result.Should().BeEmpty();

      static IEnumerable<IRuleViolation> Result;
    }

    class when_evaluating_and_at_least_one_file_is_missing
    {
      Establish ctx = () => { A.CallTo(() => ProjectItem.File.Exists).Returns(false); };

      Because of = () => Result = SUT.Evaluate(Project);

      It returns_violation = () =>
          Result.ShouldAllBeEquivalentTo(
              new RuleViolation(SUT, Project, "Could not find project item 'ProjectItem'."));

      static IEnumerable<IRuleViolation> Result;
    }
  }
}