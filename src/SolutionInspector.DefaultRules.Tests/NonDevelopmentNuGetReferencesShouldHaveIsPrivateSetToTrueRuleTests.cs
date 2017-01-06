using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;
using SolutionInspector.TestInfrastructure;

namespace SolutionInspector.DefaultRules.Tests
{
  public class NonDevelopmentNuGetReferencesShouldHaveIsPrivateSetToTrueRuleTests
  {
    private INuGetReference _nuGetReference;
    private IProject _project;

    private NonDevelopmentNuGetReferencesShouldHaveIsPrivateSetToTrueRule _sut;

    [SetUp]
    public void SetUp ()
    {
      _project = A.Fake<IProject>();

      _nuGetReference = A.Fake<INuGetReference>();
      A.CallTo(() => _nuGetReference.Package.Id).Returns("Id");

      A.CallTo(() => _project.NuGetReferences).Returns(new[] { _nuGetReference });

      _sut = new NonDevelopmentNuGetReferencesShouldHaveIsPrivateSetToTrueRule();
    }

    [Test]
    public void Evaluate_DevelopmentNuGetDependency_ReturnsNoViolations ()
    {
      A.CallTo(() => _nuGetReference.IsPrivate).Returns(Some.Boolean);
      A.CallTo(() => _nuGetReference.Package.IsDevelopmentDependency).Returns(true);

      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      result.Should().BeEmpty();
    }

    [Test]
    public void Evaluate_NonDevelopmentNuGetDependencyWithIsPrivateTrue_ReturnsNoViolations ()
    {
      A.CallTo(() => _nuGetReference.IsPrivate).Returns(true);
      A.CallTo(() => _nuGetReference.Package.IsDevelopmentDependency).Returns(false);

      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      result.Should().BeEmpty();
    }

    [Test]
    public void Evaluate_NonDevelopmentNuGetDependencyWithIsPrivateFalse_ReturnsViolation ()
    {
      A.CallTo(() => _nuGetReference.IsPrivate).Returns(false);
      A.CallTo(() => _nuGetReference.Package.IsDevelopmentDependency).Returns(false);

      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      result.ShouldBeEquivalentTo(
        new[]
        {
          new RuleViolation(
            _sut,
            _project,
            "The NuGet reference to package 'Id' is not a development dependency and therefore should has its 'IsPrivate' flag set to true.")
        });
    }
  }
}