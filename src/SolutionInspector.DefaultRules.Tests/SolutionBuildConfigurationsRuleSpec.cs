using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using FakeItEasy;
using Fasterflect;
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
  [Subject (typeof(SolutionBuildConfigurationsRule))]
  class SolutionBuildConfigurationsRuleSpec
  {
    static ISolution Solution;
    static IReadOnlyCollection<BuildConfiguration> SolutionConfigurations;

    static SolutionBuildConfigurationsRule SUT;

    Establish ctx = () =>
    {
      Solution = A.Fake<ISolution>();

      SolutionConfigurations = new BuildConfiguration[0];
      A.CallTo(() => Solution.BuildConfigurations).Returns(SolutionConfigurations);

      SUT = new SolutionBuildConfigurationsRule(
          new SolutionBuildConfigurationsRuleConfiguration
          {
              Configurations = new CommaDelimitedStringCollection { "Configuration" },
              Platforms = new CommaDelimitedStringCollection { "Platform" }
          });
    };

    class when_getting_expected_configurations
    {
      Establish ctx = () =>
      {
        SUT = new SolutionBuildConfigurationsRule(
            new SolutionBuildConfigurationsRuleConfiguration
            {
                Configurations = new CommaDelimitedStringCollection { "C1", "C2" },
                Platforms = new CommaDelimitedStringCollection { "P1", "P2" }
            });
      };

      Because of = () => Result = (IReadOnlyCollection<BuildConfiguration>) SUT.GetPropertyValue("ExpectedConfigurations");

      It returns_expected_configurations = () =>
          Result.Should().Equal(
              new BuildConfiguration("C1", "P1"),
              new BuildConfiguration("C1", "P2"),
              new BuildConfiguration("C2", "P1"),
              new BuildConfiguration("C2", "P2"));

      static IReadOnlyCollection<BuildConfiguration> Result;
    }

    class when_all_configurations_are_as_expected
    {
      Establish ctx = () => { A.CallTo(() => Solution.BuildConfigurations).Returns(new[] { new BuildConfiguration("Configuration", "Platform") }); };

      Because of = () => Result = SUT.Evaluate(Solution).ToArray();

      It returns_no_violations = () =>
          Result.Should().BeEmpty();

      static IEnumerable<IRuleViolation> Result;
    }

    class when_there_are_unexpected_configurations
    {
      Establish ctx =
          () =>
          {
            A.CallTo(() => Solution.BuildConfigurations)
                .Returns(new[] { new BuildConfiguration("Configuration", "Platform"), new BuildConfiguration("Unex", "pected") });
          };

      Because of = () => Result = SUT.Evaluate(Solution);

      It returns_violations = () =>
          Result.ShouldAllBeLike(new RuleViolation(SUT, Solution, "Unexpected build configuration 'Unex|pected' found."));

      static IEnumerable<IRuleViolation> Result;
    }

    class when_there_are_missing_configurations
    {
      Establish ctx = () => { A.CallTo(() => Solution.BuildConfigurations).Returns(new BuildConfiguration[0]); };

      Because of = () => Result = SUT.Evaluate(Solution);

      It returns_violations = () =>
          Result.ShouldAllBeLike(new RuleViolation(SUT, Solution, "Build configuration 'Configuration|Platform' could not be found."));

      static IEnumerable<IRuleViolation> Result;
    }
  }
}