using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;

namespace SolutionInspector.DefaultRules.Tests
{
  public class ProjectItemMustBeIncludedByWildcardRuleTests
  {
    private IProject _project;
    private IProjectItem _projectItem;

    private ProjectItemMustBeIncludedByWildcardRule _sut;

    [SetUp]
    public void SetUp ()
    {
      _projectItem = A.Fake<IProjectItem>();
      A.CallTo(() => _projectItem.Identifier).Returns("Item");

      _project = A.Fake<IProject>();
      A.CallTo(() => _project.ProjectItems).Returns(new[] { _projectItem });

      _sut = new ProjectItemMustBeIncludedByWildcardRule();
    }

    [Test]
    public void Evaluate_ItemIsIncludedByWildcard_ReturnsNoViolation ()
    {
      A.CallTo(() => _projectItem.IsIncludedByWildcard).Returns(true);

      // ACT
      var result = _sut.Evaluate(_projectItem);

      // ASSERT
      result.Should().BeEmpty();
    }

    [Test]
    public void Evaluate_ItemIsNotIncludedByWildcard_ReturnsViolation ()
    {
      A.CallTo(() => _projectItem.IsIncludedByWildcard).Returns(false);

      // ACT
      var result = _sut.Evaluate(_projectItem);

      // ASSERT
      result.ShouldBeEquivalentTo(
        new[]
        {
          new RuleViolation(_sut, _projectItem, "Project item 'Item' must be included via wildcard.")
        });
    }
  }
}