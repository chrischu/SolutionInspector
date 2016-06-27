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
  [Subject (typeof(ProjectItemMustNotBeIncludedByWildcardRule))]
  class ProjectItemMustNotBeIncludedByWildcardRuleSpec
  {
    static IProjectItem ProjectItem;
    static IProject Project;

    static ProjectItemMustNotBeIncludedByWildcardRule SUT;

    Establish ctx = () =>
    {
      ProjectItem = A.Fake<IProjectItem>();
      A.CallTo(() => ProjectItem.Identifier).Returns("Item");

      Project = A.Fake<IProject>();
      A.CallTo(() => Project.ProjectItems).Returns(new[] { ProjectItem });

      SUT = new ProjectItemMustNotBeIncludedByWildcardRule();
    };

    class when_evaluating_and_item_is_not_included_by_wildcard
    {
      Establish ctx = () => { A.CallTo(() => ProjectItem.IsIncludedByWildcard).Returns(false); };

      Because of = () => Result = SUT.Evaluate(ProjectItem);

      It does_not_return_violations = () =>
          Result.Should().BeEmpty();

      static IEnumerable<IRuleViolation> Result;
    }

    class when_evaluating_and_item_is_included_by_wildcard
    {
      Establish ctx = () => { A.CallTo(() => ProjectItem.IsIncludedByWildcard).Returns(true); };

      Because of = () => Result = SUT.Evaluate(ProjectItem);

      It returns_violation = () =>
          Result.ShouldAllBeLike(new RuleViolation(SUT, ProjectItem, "Project item 'Item' must NOT be included via wildcard."));

      static IEnumerable<IRuleViolation> Result;
    }

    
  }
}