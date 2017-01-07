using System;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;

namespace SolutionInspector.DefaultRules.Tests
{
  public class ProjectGuidsMustBeUniqueRuleTests
  {
    private IProject _project1;
    private IProject _project2;
    private IProject _project3;
    private ISolution _solution;

    private ProjectGuidsMustBeUniqueRule _sut;

    [SetUp]
    public void SetUp ()
    {
      _solution = A.Fake<ISolution>();

      _project1 = A.Fake<IProject>();
      A.CallTo(() => _project1.Name).Returns("Project1");
      A.CallTo(() => _project1.Guid).Returns(Guid.NewGuid());

      _project2 = A.Fake<IProject>();
      A.CallTo(() => _project2.Name).Returns("Project2");
      A.CallTo(() => _project2.Guid).Returns(Guid.NewGuid());

      _project3 = A.Fake<IProject>();
      A.CallTo(() => _project3.Name).Returns("Project3");
      A.CallTo(() => _project3.Guid).Returns(Guid.NewGuid());

      A.CallTo(() => _solution.Projects).Returns(new[] { _project1, _project2, _project3 });

      _sut = new ProjectGuidsMustBeUniqueRule();
    }

    [Test]
    public void Evaluate_NoDuplicateProjectGuids_ReturnsNoViolations ()
    {
      // ACT
      var result = _sut.Evaluate(_solution);

      // ASSERT
      result.Should().BeEmpty();
    }

    [Test]
    public void Evaluate_DuplicateProjectGuids_ReturnsViolation ()
    {
      var duplicateGuid = Guid.NewGuid();
      A.CallTo(() => _project1.Guid).Returns(duplicateGuid);
      A.CallTo(() => _project2.Guid).Returns(duplicateGuid);

      // ACT
      var result = _sut.Evaluate(_solution);

      // ASSERT
      result.ShouldBeEquivalentTo(
        new[]
        {
          new RuleViolation(
            _sut,
            _solution,
            $"The project GUID '{duplicateGuid}' is used in multiple projects ('Project1', 'Project2').")
        });
    }
  }
}