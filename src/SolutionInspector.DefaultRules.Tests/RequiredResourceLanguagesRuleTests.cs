using System.Configuration;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;

namespace SolutionInspector.DefaultRules.Tests
{
  public class RequiredResourceLanguagesRuleTests
  {
    private IProject _project;

    private RequiredResourceLanguagesRule _sut;

    [SetUp]
    public void SetUp ()
    {
      _project = A.Fake<IProject>();

      _sut = new RequiredResourceLanguagesRule(
        new RequiredResourceLanguagesRuleConfiguration
        {
          RequiredResources = new CommaDelimitedStringCollection { "Resources1", "Resources2" },
          RequiredLanguages = new CommaDelimitedStringCollection { "de", "cs" }
        });
    }

    [Test]
    public void Evaluate_ProjectContainsResourcesForAllRequiredLanguages_ReturnsNoViolations ()
    {
      A.CallTo(() => _project.ProjectItems).Returns(
        new[]
        {
          CreateProjectItem("Resources1.resx"),
          CreateProjectItem("Resources1.de.resx"),
          CreateProjectItem("Resources1.cs.resx"),
          CreateProjectItem("Resources2.resx"),
          CreateProjectItem("Resources2.de.resx"),
          CreateProjectItem("Resources2.cs.resx")
        });

      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      result.Should().BeEmpty();
    }

    [Test]
    public void Evaluate_ProjectDoesNotContainDefaultResourceFileForRequiredLanguages_ReturnsViolations ()
    {
      A.CallTo(() => _project.ProjectItems).Returns(
        new[]
        {
          CreateProjectItem("Resources1.de.resx"),
          CreateProjectItem("Resources1.cs.resx"),
          CreateProjectItem("Resources2.de.resx"),
          CreateProjectItem("Resources2.cs.resx")
        });

      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      result.ShouldBeEquivalentTo(
        new[]
        {
          new RuleViolation(_sut, _project, "For the required resource 'Resources1' no default resource file ('Resources1.resx') could be found."),
          new RuleViolation(_sut, _project, "For the required resource 'Resources2' no default resource file ('Resources2.resx') could be found.")
        });
    }

    [Test]
    public void Evaluate_ProjectDoesNotContainDResourceFileForARequiredLanguage_ReturnsViolations ()
    {
      A.CallTo(() => _project.ProjectItems).Returns(
        new[]
        {
          CreateProjectItem("Resources1.resx"),
          CreateProjectItem("Resources1.cs.resx"),
          CreateProjectItem("Resources2.resx"),
          CreateProjectItem("Resources2.cs.resx")
        });

      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      result.ShouldBeEquivalentTo(
        new[]
        {
          new RuleViolation(
            _sut,
            _project,
            "For the required resource 'Resources1' no resource file for language 'de' ('Resources1.de.resx') could be found."),
          new RuleViolation(
            _sut,
            _project,
            "For the required resource 'Resources2' no resource file for language 'de' ('Resources2.de.resx') could be found.")
        });
    }

    private IProjectItem CreateProjectItem (string name)
    {
      var projectItem = A.Fake<IProjectItem>();
      A.CallTo(() => projectItem.Name).Returns(name);
      return projectItem;
    }
  }
}