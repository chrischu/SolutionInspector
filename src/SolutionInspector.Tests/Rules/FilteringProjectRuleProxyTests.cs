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
  public class FilteringProjectRuleProxyTests
  {
    private INameFilter _projectNameFilter;
    private IRuleViolation _projectRuleViolation;
    private IProjectRule _projectRule;

    private FilteringProjectRuleProxy _sut;

    private IProject _project;
    private string _projectName;

    [SetUp]
    public void SetUp ()
    {
      _projectNameFilter = A.Fake<INameFilter>();

      _projectRule = A.Fake<IProjectRule>();
      _projectRuleViolation = A.Fake<IRuleViolation>();
      A.CallTo(() => _projectRule.Evaluate(A<IProject>._)).Returns(new[] { _projectRuleViolation });

      _sut = new FilteringProjectRuleProxy(_projectNameFilter, _projectRule);

      _project = A.Fake<IProject>();
      _projectName = Some.String();
      A.CallTo(() => _project.Name).Returns(_projectName);
    }

    [Test]
    public void Evaluate_ProjectNameMatches_EvaluatesRuleAndPassesOnViolations ()
    {
      A.CallTo(() => _projectNameFilter.IsMatch(A<string>._)).Returns(true);

      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      A.CallTo(() => _projectRule.Evaluate(_project)).MustHaveHappened();
      result.Single().Should().BeSameAs(_projectRuleViolation);

      A.CallTo(() => _projectNameFilter.IsMatch(_projectName)).MustHaveHappened();
    }

    [Test]
    public void Evaluate_ProjectNameDoesNotMatch_DoesNotEvaluateRuleAndReturnsNoViolations ()
    {
      A.CallTo(() => _projectNameFilter.IsMatch(A<string>._)).Returns(false);

      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      A.CallTo(() => _projectRule.Evaluate(_project)).MustNotHaveHappened();
      result.Should().BeEmpty();

      A.CallTo(() => _projectNameFilter.IsMatch(_projectName)).MustHaveHappened();
    }
  }
}