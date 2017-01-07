using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;

namespace SolutionInspector.DefaultRules.Tests
{
  public class ProjectItemsMustBePresentRuleTests
  {
    private IProject _project;
    private IProjectItem _projectItem;
    private IProjectItemInclude _projectItemInclude;

    private ProjectItemsMustBePresentRule _sut;

    [SetUp]
    public void SetUp ()
    {
      _projectItem = A.Fake<IProjectItem>();

      _projectItemInclude = A.Fake<IProjectItemInclude>();
      A.CallTo(() => _projectItemInclude.Evaluated).Returns("ProjectItem");

      A.CallTo(() => _projectItem.Include).Returns(_projectItemInclude);

      _project = A.Fake<IProject>();
      A.CallTo(() => _project.ProjectItems).Returns(new[] { _projectItem });

      _sut = new ProjectItemsMustBePresentRule();
    }

    [Test]
    public void Evaluate_AllItemsExists_ReturnsNoViolations ()
    {
      A.CallTo(() => _projectItem.File.Exists).Returns(true);

      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      result.Should().BeEmpty();
    }

    [Test]
    public void Evaluate_ItemDoesNotExist_ReturnsViolation ()
    {
      A.CallTo(() => _projectItem.File.Exists).Returns(false);

      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      result.ShouldBeEquivalentTo(
        new[]
        {
          new RuleViolation(_sut, _project, "Could not find project item 'ProjectItem'.")
        });
    }
  }
}