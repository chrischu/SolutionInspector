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
  [Subject (typeof(ProjectShouldNotContainProjectPropertiesWithDuplicateNameRule))]
  class ProjectShouldNotContainProjectPropertiesWithDuplicateNameRuleSpec
  {
    static IAdvancedProject AdvancedProject;
    static IProject Project;

    static ProjectShouldNotContainProjectPropertiesWithDuplicateNameRule SUT;

    Establish ctx = () =>
    {
      Project = A.Fake<IProject>();

      AdvancedProject = A.Fake<IAdvancedProject>();
      A.CallTo(() => Project.Advanced).Returns(AdvancedProject);

      SUT = new ProjectShouldNotContainProjectPropertiesWithDuplicateNameRule();
    };

    class when_evaluating_and_there_are_no_duplicates
    {
      Establish ctx = () => { SetupProperties(FakeProperty("One", FakeOccurrence("1")), FakeProperty("Two", FakeOccurrence("2"))); };

      Because of = () => Result = SUT.Evaluate(Project);

      It does_not_return_violations = () =>
          Result.Should().BeEmpty();

      static IEnumerable<IRuleViolation> Result;
    }

    class when_evaluating_and_there_are_duplicates
    {
      Establish ctx = () => { SetupProperties(FakeProperty("One", FakeOccurrence("1"), FakeOccurrence("2"))); };

      Because of = () => Result = SUT.Evaluate(Project);

      It returns_violation = () =>
          Result.ShouldAllBeLike(
              new RuleViolation(
                  SUT,
                  Project,
                  "There are multiple project properties with name 'One' and the same conditions in the following locations: 1, 2."));

      static IEnumerable<IRuleViolation> Result;
    }

    class when_evaluating_and_there_are_duplicates_with_different_conditions
    {
      Establish ctx =
          () =>
          {
            SetupProperties(
                FakeProperty(
                    "One",
                    FakeOccurrence("1", A.Dummy<IProjectPropertyCondition>()),
                    FakeOccurrence("2", A.Dummy<IProjectPropertyCondition>())));
          };

      Because of = () => Result = SUT.Evaluate(Project);

      It does_not_return_violations = () =>
          Result.Should().BeEmpty();

      static IEnumerable<IRuleViolation> Result;
    }

    static void SetupProperties (params IProjectProperty[] properties)
    {
      A.CallTo(() => AdvancedProject.Properties).Returns(properties.ToDictionary(p => p.Name));
    }

    static IProjectPropertyOccurrence FakeOccurrence (string location, IProjectPropertyCondition condition = null)
    {
      var occurrence = A.Fake<IProjectPropertyOccurrence>();

      var loc = A.Fake<IProjectLocation>();
      A.CallTo(() => loc.ToString()).Returns(location);

      A.CallTo(() => occurrence.Location).Returns(loc);

      A.CallTo(() => occurrence.Condition).Returns(condition);

      return occurrence;
    }

    static IProjectProperty FakeProperty (string name, params IProjectPropertyOccurrence[] occurrences)
    {
      var property = A.Fake<IProjectProperty>();

      A.CallTo(() => property.Name).Returns(name);
      A.CallTo(() => property.Occurrences).Returns(occurrences);

      return property;
    }
  }
}