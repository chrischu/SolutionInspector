using System;
using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;
using SolutionInspector.Commons.Extensions;
using SolutionInspector.TestInfrastructure.Api;

namespace SolutionInspector.DefaultRules.Tests
{
  public class SolutionBuildConfigurationsRuleTests : RuleTestBase
  {
    private ISolution _solution;
    private IReadOnlyCollection<BuildConfiguration> _solutionConfigurations;

    private SolutionBuildConfigurationsRule _sut;

    [SetUp]
    public void SetUp ()
    {
      _solution = A.Fake<ISolution>();

      _solutionConfigurations = new BuildConfiguration[0];
      A.CallTo(() => _solution.BuildConfigurations).Returns(_solutionConfigurations);

      _sut = CreateRule<SolutionBuildConfigurationsRule>(
          r =>
          {
            r.Configurations.AssertNotNull().Add("Configuration");
            r.Platforms.AssertNotNull().Add("Platform");
          });
    }

    [Test]
    public void Evaluate_SolutionHasAllExpectedConfigurations_ReturnsNoViolations ()
    {
      A.CallTo(() => _solution.BuildConfigurations).Returns(new[] { new BuildConfiguration("Configuration", "Platform") });

      // ACT
      var result = _sut.Evaluate(_solution);

      // ASSERT
      result.Should().BeEmpty();
    }

    [Test]
    public void Evaluate_SolutionHasUnexpectedConfiguration_ReturnsViolation ()
    {
      A.CallTo(() => _solution.BuildConfigurations)
          .Returns(new[] { new BuildConfiguration("Configuration", "Platform"), new BuildConfiguration("Unex", "pected") });

      // ACT
      var result = _sut.Evaluate(_solution);

      // ASSERT
      result.ShouldBeEquivalentTo(
          new[]
          {
              new RuleViolation(_sut, _solution, "Unexpected build configuration 'Unex|pected' found.")
          });
    }

    [Test]
    public void Evaluate_SolutionDoesNotHaveExpectedConfiguration_ReturnsViolation ()
    {
      A.CallTo(() => _solution.BuildConfigurations).Returns(new BuildConfiguration[0]);

      // ACT
      var result = _sut.Evaluate(_solution);

      // ASSERT
      result.ShouldBeEquivalentTo(
          new[]
          {
              new RuleViolation(_sut, _solution, "Build configuration 'Configuration|Platform' could not be found.")
          });
    }
  }
}