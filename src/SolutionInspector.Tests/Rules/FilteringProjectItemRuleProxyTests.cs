using System.Linq;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Api.Configuration;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;
using SolutionInspector.Rules;
using SolutionInspector.TestInfrastructure;

namespace SolutionInspector.Tests.Rules
{
  public class FilteringProjectItemRuleProxyTests
  {
    private INameFilter _projectNameFilter;
    private INameFilter _projectItemNameFilter;
    private IRuleViolation _projectItemRuleViolation;
    private IProjectItemRule _projectItemRule;

    private FilteringProjectItemRuleProxy _sut;

    private IProjectItem _projectItem;
    private string _projectItemName;
    private string _projectName;

    [SetUp]
    public void SetUp ()
    {
      _projectNameFilter = A.Fake<INameFilter>();
      _projectItemNameFilter = A.Fake<INameFilter>();

      _projectItemRule = A.Fake<IProjectItemRule>();
      _projectItemRuleViolation = A.Fake<IRuleViolation>();
      A.CallTo(() => _projectItemRule.Evaluate(A<IProjectItem>._)).Returns(new[] { _projectItemRuleViolation });

      _sut = new FilteringProjectItemRuleProxy(_projectItemNameFilter, _projectNameFilter, _projectItemRule);

      _projectItem = A.Fake<IProjectItem>();
      _projectItemName = Some.String();
      A.CallTo(() => _projectItem.Name).Returns(_projectItemName);

      var project = A.Fake<IProject>();
      _projectName = Some.String();
      A.CallTo(() => project.Name).Returns(_projectName);

      A.CallTo(() => _projectItem.Project).Returns(project);
    }

    [Test]
    public void Evaluate_AllFiltersMatch_CallsRuleAndPassesOnViolations ()
    {
      A.CallTo(() => _projectNameFilter.IsMatch(A<string>._)).Returns(true);
      A.CallTo(() => _projectItemNameFilter.IsMatch(A<string>._)).Returns(true);

      // ACT
      var result = _sut.Evaluate(_projectItem);

      // ASSERT
      A.CallTo(() => _projectItemRule.Evaluate(_projectItem)).MustHaveHappened();
      result.Single().Should().BeSameAs(_projectItemRuleViolation);

      A.CallTo(() => _projectNameFilter.IsMatch(_projectName)).MustHaveHappened();
      A.CallTo(() => _projectItemNameFilter.IsMatch(_projectItemName)).MustHaveHappened();
    }

    [Test]
    public void Evaluate_ProjectFilterDoesNotMatch_DoesNotCallRuleAndReturnsNoViolations ()
    {
      A.CallTo(() => _projectNameFilter.IsMatch(A<string>._)).Returns(false);

      // ACT
      var result = _sut.Evaluate(_projectItem);

      // ASSERT
      A.CallTo(() => _projectItemRule.Evaluate(_projectItem)).MustNotHaveHappened();
      result.Should().BeEmpty();

      A.CallTo(() => _projectNameFilter.IsMatch(_projectName)).MustHaveHappened();
      A.CallTo(() => _projectItemNameFilter.IsMatch(_projectItemName)).MustNotHaveHappened();
    }

    [Test]
    public void Evaluate_ProjectItemFilterDoesNotMatch_DoesNotCallRuleAndReturnsNoViolations ()
    {
      A.CallTo(() => _projectNameFilter.IsMatch(A<string>._)).Returns(true);
      A.CallTo(() => _projectItemNameFilter.IsMatch(A<string>._)).Returns(false);

      // ACT
      var result = _sut.Evaluate(_projectItem);

      // ASSERT
      A.CallTo(() => _projectItemRule.Evaluate(_projectItem)).MustNotHaveHappened();
      result.Should().BeEmpty();

      A.CallTo(() => _projectNameFilter.IsMatch(_projectName)).MustHaveHappened();
      A.CallTo(() => _projectItemNameFilter.IsMatch(_projectItemName)).MustHaveHappened();
    }
  }
}