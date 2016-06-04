using System;
using System.Collections.Generic;
using System.Linq;
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
  [Subject (typeof (ProjectShouldNotContainProjectPropertiesWithDuplicateNameRule))]
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
      Establish ctx = () => { SetupProperties(FakeProperty("One", "1"), FakeProperty("Two", "2")); };

      Because of = () => Result = SUT.Evaluate(Project);

      It does_not_return_violations = () =>
          Result.Should().BeEmpty();

      static IEnumerable<IRuleViolation> Result;
    }

    class when_evaluating_and_there_are_duplicates
    {
      Establish ctx = () => { SetupProperties(FakeProperty("One", "1", "2", "3")); };

      Because of = () => Result = SUT.Evaluate(Project);

      It returns_violation = () =>
          Result.ShouldAllBeEquivalentTo(
              new RuleViolation(SUT, Project, "There are multiple project properties with name 'One' in the following locations: 1, 2, 3."));

      static IEnumerable<IRuleViolation> Result;
    }

    static void SetupProperties (params IProjectProperty[] properties)
    {
      A.CallTo(() => AdvancedProject.Properties).Returns(properties.ToDictionary(p => p.Name));
    }

    static IProjectProperty FakeProperty (string name, params string[] locationStrings)
    {
      var property = A.Fake<IProjectProperty>();

      A.CallTo(() => property.Name).Returns(name);

      var occurrences = locationStrings.Select(
          l =>
          {
            var location = A.Fake<IProjectLocation>();
            A.CallTo(() => location.ToString()).Returns(l);

            var occurrence = A.Fake<IProjectPropertyOccurrence>();
            A.CallTo(() => occurrence.Location).Returns(location);

            return occurrence;
          }).ToArray();

      A.CallTo(() => property.Occurrences).Returns(occurrences);

      return property;
    }
  }
}