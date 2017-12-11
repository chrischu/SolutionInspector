using System;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;
using SolutionInspector.Commons.Extensions;
using SolutionInspector.Configuration;
using SolutionInspector.TestInfrastructure;

namespace SolutionInspector.DefaultRules.Tests
{
  public class RequiredNuGetPackagesRuleTests
  {
    private IProject _project;

    private RequiredNuGetPackagesRule _sut;

    [SetUp]
    public void SetUp ()
    {
      _project = A.Fake<IProject>();

      var configuration = ConfigurationElement.Create<RequiredNuGetPackagesRuleConfiguration>(
        initialize: c => { c.RequiredNuGetPackages.AssertNotNull().Add("Package"); });

      _sut = new RequiredNuGetPackagesRule(configuration);
    }

    [Test]
    public void Evaluate_AllNuGetPackagesAreThere_ReturnsNoViolations ()
    {
      var nuGetPackage = FakeHelper.CreateAndConfigure<INuGetPackage>(
        c =>
        {
          A.CallTo(() => c.Id).Returns("Package");
          A.CallTo(() => c.Version).Returns(new Version(0, 0, 1));
          A.CallTo(() => c.IsPreRelease).Returns(false);
          A.CallTo(() => c.PreReleaseTag).Returns(null);
          A.CallTo(() => c.TargetFramework).Returns("net461");
          A.CallTo(() => c.IsDevelopmentDependency).Returns(false);
        });

      A.CallTo(() => _project.NuGetPackages).Returns(new[] { nuGetPackage });

      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      result.Should().BeEmpty();
    }

    [Test]
    public void Evaluate_OneNuGetPackageIsMissing_ReturnsViolation ()
    {
      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      result.ShouldBeEquivalentTo(new[] { new RuleViolation(_sut, _project, "Required NuGet package 'Package' is missing.") });
    }
  }
}