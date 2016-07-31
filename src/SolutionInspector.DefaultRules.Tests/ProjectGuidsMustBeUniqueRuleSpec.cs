using System;
using System.Collections.Generic;
using System.Linq;
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
  [Subject (typeof(ProjectGuidsMustBeUniqueRule))]
  class ProjectGuidsMustBeUniqueRuleSpec
  {
    static ISolution Solution;
    static IProject Project1;
    static IProject Project2;
    static IProject Project3;

    static ProjectGuidsMustBeUniqueRule SUT;

    Establish ctx = () =>
    {
      Solution = A.Fake<ISolution>();

      Project1 = A.Fake<IProject>();
      A.CallTo(() => Project1.Name).Returns("Project1");
      A.CallTo(() => Project1.Guid).Returns(Guid.NewGuid());

      Project2 = A.Fake<IProject>();
      A.CallTo(() => Project2.Name).Returns("Project2");
      A.CallTo(() => Project2.Guid).Returns(Guid.NewGuid());

      Project3 = A.Fake<IProject>();
      A.CallTo(() => Project3.Name).Returns("Project3");
      A.CallTo(() => Project3.Guid).Returns(Guid.NewGuid());

      A.CallTo(() => Solution.Projects).Returns(new[] { Project1, Project2, Project3 });

      SUT = new ProjectGuidsMustBeUniqueRule();
    };

    class when_there_are_no_duplicate_project_guids
    {
      Because of = () => Result = SUT.Evaluate(Solution).ToArray();

      It returns_no_violations = () =>
          Result.Should().BeEmpty();

      static IEnumerable<IRuleViolation> Result;
    }

    class when_there_are_duplicate_project_guids
    {
      Establish ctx = () =>
      {
        DuplicateGuid = Guid.NewGuid();
        A.CallTo(() => Project1.Guid).Returns(DuplicateGuid);
        A.CallTo(() => Project2.Guid).Returns(DuplicateGuid);
      };

      Because of = () => Result = SUT.Evaluate(Solution).ToArray();

      It returns_violations = () =>
          Result.ShouldAllBeLike(
              new RuleViolation(
                  SUT,
                  Solution,
                  $"The project GUID '{DuplicateGuid}' is used in multiple projects ('Project1', 'Project2')."));

      static Guid DuplicateGuid;
      static IEnumerable<IRuleViolation> Result;
    }
  }
}