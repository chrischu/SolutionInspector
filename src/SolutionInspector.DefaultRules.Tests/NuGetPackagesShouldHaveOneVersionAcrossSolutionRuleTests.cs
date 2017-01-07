using System;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;
using SolutionInspector.TestInfrastructure;

namespace SolutionInspector.DefaultRules.Tests
{
  public class NuGetPackagesShouldHaveOneVersionAcrossSolutionRuleTests
  {
    private IProject _project1;
    private IProject _project2;
    private ISolution _solution;

    private NuGetPackagesShouldHaveOneVersionAcrossSolutionRule _sut;

    [SetUp]
    public void SetUp ()
    {
      _solution = A.Fake<ISolution>();

      _project1 = A.Fake<IProject>();
      _project2 = A.Fake<IProject>();
      A.CallTo(() => _solution.Projects).Returns(new[] { _project1, _project2 });

      _sut = new NuGetPackagesShouldHaveOneVersionAcrossSolutionRule();
    }

    [Test]
    public void Evaluate_NoNuGetPackageWithMultipleVersions_ReturnsNoViolations ()
    {
      A.CallTo(() => _project1.NuGetPackages).Returns(new[] { CreateNuGetPackage("Id", new Version(1, 0)) });
      A.CallTo(() => _project2.NuGetPackages).Returns(new[] { CreateNuGetPackage("Id", new Version(1, 0)) });

      // ACT
      var result = _sut.Evaluate(_solution);

      // ASSERT
      result.Should().BeEmpty();
    }

    [Test]
    public void Evaluate_NuGetPackageWithMultipleVersions_ReturnsViolation ()
    {
      A.CallTo(() => _project1.NuGetPackages).Returns(new[] { CreateNuGetPackage("Id", new Version(1, 0)) });
      A.CallTo(() => _project2.NuGetPackages).Returns(new[] { CreateNuGetPackage("Id", new Version(1, 1)) });

      // ACT
      var result = _sut.Evaluate(_solution);

      // ASSERT
      result.ShouldBeEquivalentTo(
        new[]
        {
          new RuleViolation(_sut, _solution, "The NuGet package 'Id' is referenced in more than one version ('1.0', '1.1').")
        });
    }

    [Test]
    public void Evaluate_NuGetPackageWithMultipleVersionsOnlyDifferingByPreReleaseTag_ReturnsViolations ()
    {
      A.CallTo(() => _project1.NuGetPackages).Returns(new[] { CreateNuGetPackage("Id", new Version(1, 0)) });
      A.CallTo(() => _project2.NuGetPackages).Returns(new[] { CreateNuGetPackage("Id", new Version(1, 0), "-tag") });

      // ACT
      var result = _sut.Evaluate(_solution);

      // ASSERT
      result.ShouldBeEquivalentTo(
        new[]
        {
          new RuleViolation(_sut, _solution, "The NuGet package 'Id' is referenced in more than one version ('1.0', '1.0-tag').")
        });
    }

    private INuGetPackage CreateNuGetPackage (string id, Version version, string tag = null)
    {
      return FakeHelper.CreateAndConfigure<INuGetPackage>(
        c =>
        {
          A.CallTo(() => c.Id).Returns(id);
          A.CallTo(() => c.Version).Returns(version);
          A.CallTo(() => c.IsPreRelease).Returns(true);
          A.CallTo(() => c.PreReleaseTag).Returns(tag);
          A.CallTo(() => c.TargetFramework).Returns("targetFramework");
          A.CallTo(() => c.IsDevelopmentDependency).Returns(false);
          A.CallTo(() => c.FullVersionString).Returns($"{version}{tag}");
        });
    }
  }
}