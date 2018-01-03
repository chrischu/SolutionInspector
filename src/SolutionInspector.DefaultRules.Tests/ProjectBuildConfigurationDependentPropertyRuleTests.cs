using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Api.Configuration;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;
using SolutionInspector.TestInfrastructure.Api;

namespace SolutionInspector.DefaultRules.Tests
{
  public class ProjectBuildConfigurationDependentPropertyRuleTests : RuleTestBase
  {
    private IAdvancedProject _advancedProject;

    private BuildConfiguration _buildConfiguration1;
    private BuildConfiguration _buildConfiguration2;
    private BuildConfiguration _buildConfiguration3;
    private string _expectedValue;
    private BuildConfigurationFilter _filterIncludeOneAndTwo;

    private BuildConfigurationFilter _filterIncludeOneOnly;
    private IProject _project;

    private string _property;

    private ProjectBuildConfigurationDependentPropertyRule _sut;

    [SetUp]
    public void SetUp ()
    {
      _project = A.Fake<IProject>();

      _buildConfiguration1 = new BuildConfiguration("Configuration", "Platform");
      _buildConfiguration2 = new BuildConfiguration("Configuration", "Platform2");
      _buildConfiguration3 = new BuildConfiguration("Configuration2", "Platform");

      _filterIncludeOneOnly = new BuildConfigurationFilter(_buildConfiguration1);
      _filterIncludeOneAndTwo = new BuildConfigurationFilter(new BuildConfiguration("Configuration", "*"));

      _advancedProject = A.Fake<IAdvancedProject>();
      A.CallTo(() => _project.Advanced).Returns(_advancedProject);
      A.CallTo(() => _project.BuildConfigurations).Returns(new[] { _buildConfiguration1, _buildConfiguration2, _buildConfiguration3 });

      _property = "Property";
      _expectedValue = "ExpectedValue";

      _sut = CreateRule<ProjectBuildConfigurationDependentPropertyRule>(
          r =>
          {
            r.BuildConfigurationFilter = _filterIncludeOneOnly;
            r.Property = _property;
            r.ExpectedValue = _expectedValue;
          });
    }

    [Test]
    public void Evaluate_PropertyWithExpectedValue_ReturnsNoViolations ()
    {
      ProjectPropertyFakeUtility.SetupFakeBuildConfigurationDependentProperty(_advancedProject, _buildConfiguration1, _property, _expectedValue);

      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      result.Should().BeEmpty();
    }

    [Test]
    public void Evaluate_PropertyWithUnexpectedValue_ReturnsViolation ()
    {
      ProjectPropertyFakeUtility.SetupFakeBuildConfigurationDependentProperty(_advancedProject, _buildConfiguration1, _property, "ActualValue");

      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      result.ShouldBeEquivalentTo(
        new[]
        {
          new RuleViolation(
            _sut,
            _project,
            "Unexpected value for property 'Property' in build configuration " +
            $"'{_buildConfiguration1}', was 'ActualValue' but should be 'ExpectedValue'.")
        });
    }

    [Test]
    public void Evaluate_NonExistingProperty_ReturnsViolation ()
    {
      A.CallTo(() => _advancedProject.EvaluateProperties(_buildConfiguration1, null))
          .Returns(new Dictionary<string, IEvaluatedProjectPropertyValue>());

      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      result.ShouldBeEquivalentTo(
        new[]
        {
          new RuleViolation(
            _sut,
            _project,
            "Unexpected value for property 'Property' in build configuration " +
            $"'{_buildConfiguration1}', was '<null>' but should be 'ExpectedValue'.")
        });
    }

    [Test]
    public void Evaluate_PropertyThatHasUnexpectedValueInOtherConfiguration_ReturnsNoViolations ()
    {
      ProjectPropertyFakeUtility.SetupFakeBuildConfigurationDependentProperty(_advancedProject, _buildConfiguration1, _property, _expectedValue);
      ProjectPropertyFakeUtility.SetupFakeBuildConfigurationDependentProperty(_advancedProject, _buildConfiguration2, _property, "ActualValue");

      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      result.Should().BeEmpty();
    }

    [Test]
    public void Evaluate_PropertyWithUnexpectedValueInMultipleConfigurations_ReturnsViolations ()
    {
      _sut.BuildConfigurationFilter = _filterIncludeOneAndTwo;

      ProjectPropertyFakeUtility.SetupFakeBuildConfigurationDependentProperty(_advancedProject, _buildConfiguration1, _property, "ActualValue");
      ProjectPropertyFakeUtility.SetupFakeBuildConfigurationDependentProperty(_advancedProject, _buildConfiguration2, _property, "ActualValue");
      ProjectPropertyFakeUtility.SetupFakeBuildConfigurationDependentProperty(_advancedProject, _buildConfiguration3, _property, _expectedValue);

      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      result.ShouldBeEquivalentTo(
        new[]
        {
          new RuleViolation(
            _sut,
            _project,
            "Unexpected value for property 'Property' in build configuration " +
            $"'{_buildConfiguration1}', was 'ActualValue' but should be 'ExpectedValue'."),
          new RuleViolation(
            _sut,
            _project,
            "Unexpected value for property 'Property' in build configuration " +
            $"'{_buildConfiguration2}', was 'ActualValue' but should be 'ExpectedValue'.")
        });
    }
  }
}