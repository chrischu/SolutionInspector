using System;
using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;
using SolutionInspector.TestInfrastructure;
using SolutionInspector.TestInfrastructure.AssertionExtensions;

#region R# preamble for Machine.Specifications files

// ReSharper disable ArrangeTypeModifiers
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable ClassNeverInstantiated.Local
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Local
// ReSharper disable NotAccessedField.Local
// ReSharper disable StaticMemberInGenericType
// ReSharper disable UnassignedField.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable UnassignedGetOnlyAutoProperty

#endregion

namespace SolutionInspector.DefaultRules.Tests
{
  [Subject (typeof(RequiredNuGetPackagesRule))]
  class RequiredNuGetPackagesRuleSpec
  {
    static IProject Project;

    static RequiredNuGetPackagesRule SUT;

    Establish ctx = () =>
    {
      Project = A.Fake<IProject>();

      SUT =
          new RequiredNuGetPackagesRule(
              new RequiredNuGetPackagesRuleConfiguration { new RequiredNuGetPackageConfigurationElement { Id = "Package" } });
    };

    class when_evaluating_and_all_NuGet_packages_are_there
    {
      Establish ctx =
          () =>
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

            A.CallTo(() => Project.NuGetPackages).Returns(new[] { nuGetPackage });
          };

      Because of = () => Result = SUT.Evaluate(Project);

      It does_not_return_violations = () =>
          Result.Should().BeEmpty();

      static IEnumerable<IRuleViolation> Result;
    }

    class when_evaluating_and_at_least_one_NuGet_package_is_missing
    {
      Because of = () => Result = SUT.Evaluate(Project);

      It returns_violation = () =>
          Result.ShouldAllBeLike(
              new RuleViolation(SUT, Project, "Required NuGet package 'Package' is missing."));

      static IEnumerable<IRuleViolation> Result;
    }
  }
}