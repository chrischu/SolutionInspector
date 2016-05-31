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
      Establish ctx = () =>
      {
        SetupUnconditionalProperties(Tuple.Create("One", "1"));
        SetupConditionalProperties(Tuple.Create("Two", "2"));
      };

      Because of = () => Result = SUT.Evaluate(Project);

      It does_not_return_violations = () =>
          Result.Should().BeEmpty();

      static IEnumerable<IRuleViolation> Result;
    }

    static void SetupUnconditionalProperties (params Tuple<string, string>[] nameAndLocation)
    {
      var properties = nameAndLocation.ToDictionary(
          x => x.Item1,
          x => SetupPropertyBase<IProjectProperty>(x.Item1, x.Item2));

      A.CallTo(() => AdvancedProject.Properties).Returns(properties);
    }

    static void SetupConditionalProperties (params Tuple<string, string>[] name)
    {
      var properties = name.Select(x => SetupPropertyBase<IConditionalProjectProperty>(x.Item1, x.Item2)).ToArray();
      A.CallTo(() => AdvancedProject.ConditionalProperties).Returns(properties);
    }

    class when_evaluating_and_there_are_duplicates
    {
      Establish ctx = () =>
      {
        SetupUnconditionalProperties(Tuple.Create("One", "1"));
        SetupConditionalProperties(Tuple.Create("One", "2"), Tuple.Create("One", "3"));
      };

      Because of = () => Result = SUT.Evaluate(Project);

      It returns_violation = () =>
          Result.ShouldAllBeEquivalentTo(
              new RuleViolation(SUT, Project, "There are multiple project properties with name 'One' in the following locations: 1, 2, 3."));

      static IEnumerable<IRuleViolation> Result;
    }

    static T SetupPropertyBase<T> (string name, string locationString)
        where T : IProjectPropertyBase
    {
      var property = A.Fake<T>();

      A.CallTo(() => property.Name).Returns(name);

      var location = A.Fake<IProjectLocation>();
      A.CallTo(() => location.ToString()).Returns(locationString);
      A.CallTo(() => property.Location).Returns(location);

      return property;
    }
  }
}