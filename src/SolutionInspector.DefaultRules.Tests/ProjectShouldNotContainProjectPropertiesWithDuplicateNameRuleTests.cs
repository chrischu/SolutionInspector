using System.Linq;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;

namespace SolutionInspector.DefaultRules.Tests
{
  public class ProjectShouldNotContainProjectPropertiesWithDuplicateNameRuleTests
  {
    private IAdvancedProject _advancedProject;
    private IProject _project;

    private ProjectShouldNotContainProjectPropertiesWithDuplicateNameRule _sut;

    [SetUp]
    public void SetUp ()
    {
      _project = A.Fake<IProject>();

      _advancedProject = A.Fake<IAdvancedProject>();
      A.CallTo(() => _project.Advanced).Returns(_advancedProject);

      _sut = new ProjectShouldNotContainProjectPropertiesWithDuplicateNameRule();
    }

    [Test]
    public void Evaluate_NoDuplicates_ReturnsNoViolations ()
    {
      SetupProperties(FakeProperty("One", FakeOccurrence("1")), FakeProperty("Two", FakeOccurrence("2")));

      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      result.Should().BeEmpty();
    }

    [Test]
    public void Evaluate_WithDuplicates_ReturnsViolation ()
    {
      SetupProperties(FakeProperty("One", FakeOccurrence("1"), FakeOccurrence("2")));

      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      result.ShouldBeEquivalentTo(
        new[]
        {
          new RuleViolation(
            _sut,
            _project,
            "There are multiple project properties with name 'One' and the same conditions in the following locations: 1, 2.")
        });
    }

    [Test]
    public void Evaluate_WithDuplicatesWithDifferentConditions_ReturnsNoViolations ()
    {
      SetupProperties(
        FakeProperty(
          "One",
          FakeOccurrence("1", A.Dummy<IProjectPropertyCondition>()),
          FakeOccurrence("2", A.Dummy<IProjectPropertyCondition>())));

      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      result.Should().BeEmpty();
    }

    private void SetupProperties (params IProjectProperty[] properties)
    {
      A.CallTo(() => _advancedProject.Properties).Returns(properties.ToDictionary(p => p.Name));
    }

    private IProjectProperty FakeProperty (string name, params IProjectPropertyOccurrence[] occurrences)
    {
      var property = A.Fake<IProjectProperty>();

      A.CallTo(() => property.Name).Returns(name);
      A.CallTo(() => property.Occurrences).Returns(occurrences);

      return property;
    }

    private IProjectPropertyOccurrence FakeOccurrence (string location, IProjectPropertyCondition condition = null)
    {
      var occurrence = A.Fake<IProjectPropertyOccurrence>();

      var loc = A.Fake<IProjectLocation>();
      A.CallTo(() => loc.ToString()).Returns(location);

      A.CallTo(() => occurrence.Location).Returns(loc);

      A.CallTo(() => occurrence.Condition).Returns(condition);

      return occurrence;
    }
  }
}