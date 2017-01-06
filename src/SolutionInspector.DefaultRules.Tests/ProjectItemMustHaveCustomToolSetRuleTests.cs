using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;

namespace SolutionInspector.DefaultRules.Tests
{
  public class ProjectItemMustHaveCustomToolSetRuleTests
  {
    private IProjectItem _projectItem;

    private ProjectItemMustHaveCustomToolSetRule _sut;

    [SetUp]
    public void SetUp ()
    {
      _projectItem = A.Fake<IProjectItem>();

      _sut = new ProjectItemMustHaveCustomToolSetRule(
               new ProjectItemMustHaveCustomToolSetRuleConfiguration
               {
                 ExpectedCustomTool = "CustomTool",
                 ExpectedCustomToolNamespace = "CustomToolNamespace"
               });
    }

    [Test]
    public void Evaluate_ItemWithExpectedValues_ReturnsNoViolations ()
    {
      A.CallTo(() => _projectItem.CustomTool).Returns("CustomTool");
      A.CallTo(() => _projectItem.CustomToolNamespace).Returns("CustomToolNamespace");

      // ACT
      var result = _sut.Evaluate(_projectItem);

      // ASSERT
      result.Should().BeEmpty();
    }

    [Test]
    public void Evaluate_ItemWithUnexpectedValues_ReturnsViolations ()
    {
      A.CallTo(() => _projectItem.CustomTool).Returns("DIFFERENT");
      A.CallTo(() => _projectItem.CustomToolNamespace).Returns("DIFFERENT_NS");

      // ACT
      var result = _sut.Evaluate(_projectItem);

      // ASSERT
      result.ShouldBeEquivalentTo(
        new[]
        {
          new RuleViolation(_sut, _projectItem, "Unexpected value for custom tool, was 'DIFFERENT' but should be 'CustomTool'."),
          new RuleViolation(
            _sut,
            _projectItem,
            "Unexpected value for custom tool namespace, was 'DIFFERENT_NS' but should be 'CustomToolNamespace'.")
        });
    }
  }
}