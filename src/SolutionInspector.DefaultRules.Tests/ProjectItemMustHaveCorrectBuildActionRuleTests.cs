using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;

namespace SolutionInspector.DefaultRules.Tests
{
  public class ProjectItemMustHaveCorrectBuildActionRuleTests
  {
    private IProjectItem _projectItem;

    private ProjectItemMustHaveCorrectBuildActionRule _sut;

    [SetUp]
    public void SetUp ()
    {
      _projectItem = A.Fake<IProjectItem>();

      _sut = new ProjectItemMustHaveCorrectBuildActionRule(
        new ProjectItemMustHaveCorrectBuildActionRuleConfiguration
        {
          ExpectedBuildAction = "Compile"
        });
    }

    [Test]
    public void Evaluate_ItemWithExpectedBuildAction_ReturnsNoViolations ()
    {
      A.CallTo(() => _projectItem.BuildAction).Returns(ProjectItemBuildAction.Compile);

      // ACT
      var result = _sut.Evaluate(_projectItem);

      // ASSERT
      result.Should().BeEmpty();
    }

    [Test]
    public void Evaluate_ItemWithUnexpectedBuildAction_ReturnsViolation ()
    {
      A.CallTo(() => _projectItem.BuildAction).Returns(ProjectItemBuildAction.None);

      // ACT
      var result = _sut.Evaluate(_projectItem);

      // ASSERT
      result.ShouldBeEquivalentTo(
        new[]
        {
          new RuleViolation(_sut, _projectItem, "Unexpected build action was \'None\', but should be \'Compile\'.")
        });
    }
  }
}