using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;

namespace SolutionInspector.DefaultRules.Tests
{
  public class ProjectPropertyRuleTests
  {
    private IProject _project;
    private IAdvancedProject _advancedProject;

    private ProjectPropertyRule _sut;

    [SetUp]
    public void SetUp ()
    {
      _project = A.Fake<IProject>();

      _advancedProject = A.Fake<IAdvancedProject>();
      A.CallTo(() => _project.Advanced).Returns(_advancedProject);

      _sut = new ProjectPropertyRule(
               new ProjectPropertyRuleConfiguration
               {
                 Property = "Property",
                 ExpectedValue = "ExpectedValue"
               });
    }

    [Test]
    public void Evaluate_PropertyWithExpectedValue_ReturnsNoViolations ()
    {
      ProjectPropertyFakeUtility.SetupFakeProperty(_advancedProject, "Property", "ExpectedValue");

      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      result.Should().BeEmpty();
    }

    [Test]
    public void Evaluate_PropertyWithUnexpectedValue_ReturnsViolation ()
    {
      ProjectPropertyFakeUtility.SetupFakeProperty(_advancedProject, "Property", "ActualValue");

      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      result.ShouldBeEquivalentTo(
        new[]
        {
          new RuleViolation(_sut, _project, "Unexpected value for property 'Property', was 'ActualValue' but should be 'ExpectedValue'.")
        });
    }

    [Test]
    public void Evaluate_NonExistingProperty_ReturnsViolation ()
    {
      ProjectPropertyFakeUtility.SetupFakeProperty(_advancedProject, "Property", null);

      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      result.ShouldBeEquivalentTo(
        new[]
        {
          new RuleViolation(_sut, _project, "Unexpected value for property 'Property', was '<null>' but should be 'ExpectedValue'.")
        });
    }
  }
}