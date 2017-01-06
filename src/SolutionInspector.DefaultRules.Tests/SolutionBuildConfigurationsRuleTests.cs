using System.Collections.Generic;
using System.Configuration;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;

namespace SolutionInspector.DefaultRules.Tests
{
  public class SolutionBuildConfigurationsRuleTests
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

      _sut = new SolutionBuildConfigurationsRule(
               new SolutionBuildConfigurationsRuleConfiguration
               {
                 Configurations = new CommaDelimitedStringCollection { "Configuration" },
                 Platforms = new CommaDelimitedStringCollection { "Platform" }
               });
    }

    [Test]
    public void GetExpectedConfigurations_ReturnsCrossproductOfConfigurationsAndPlatforms ()
    {
      _sut = new SolutionBuildConfigurationsRule(
               new SolutionBuildConfigurationsRuleConfiguration
               {
                 Configurations = new CommaDelimitedStringCollection { "C1", "C2" },
                 Platforms = new CommaDelimitedStringCollection { "P1", "P2" }
               });

      // ACT
      var result = _sut.ExpectedConfigurations;

      // ASSERT
      result.Should().Equal(
        new BuildConfiguration("C1", "P1"),
        new BuildConfiguration("C1", "P2"),
        new BuildConfiguration("C2", "P1"),
        new BuildConfiguration("C2", "P2"));
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