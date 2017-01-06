using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;

namespace SolutionInspector.DefaultRules.Tests
{
  public class ProjectShouldNotContainProjectItemsWithDuplicateIncludesRuleTests
  {
    private IProject _project;

    private ProjectShouldNotContainProjectItemsWithDuplicateIncludesRule _sut;

    [SetUp]
    public void SetUp ()
    {
      _project = A.Fake<IProject>();

      _sut = new ProjectShouldNotContainProjectItemsWithDuplicateIncludesRule();
    }

    [Test]
    public void Evaluate_NoDuplicates_ReturnsNoViolations ()
    {
      A.CallTo(() => _project.ProjectItems).Returns(new[] { FakeProjectItem("One"), FakeProjectItem("Two") });

      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      result.Should().BeEmpty();
    }

    [Test]
    public void Evaluate_WithDuplicates_ReturnsViolation ()
    {
      A.CallTo(() => _project.ProjectItems).Returns(new[] { FakeProjectItem("One", "1"), FakeProjectItem("One", "2"), FakeProjectItem("One", "3") });

      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      result.ShouldBeEquivalentTo(
        new[]
        {
          new RuleViolation(_sut, _project, "There are multiple project items with include 'One' in the following locations: 1, 2, 3.")
        });
    }

    private IProjectItem FakeProjectItem (string include, string location = null)
    {
      var projectItem = A.Fake<IProjectItem>();

      var projectItemInclude = A.Fake<IProjectItemInclude>();
      A.CallTo(() => projectItemInclude.Evaluated).Returns(include);
      A.CallTo(() => projectItem.Include).Returns(projectItemInclude);

      if (location != null)
      {
        var projectItemLocation = A.Fake<IProjectLocation>();
        A.CallTo(() => projectItemLocation.ToString()).Returns(location);
        A.CallTo(() => projectItem.Location).Returns(projectItemLocation);
      }

      return projectItem;
    }
  }
}