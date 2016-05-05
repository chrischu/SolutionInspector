using System;
using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;
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
  [Subject (typeof (NuGetPackagesShouldHaveOneVersionAcrossSolutionRule))]
  class NuGetPackagesShouldHaveOneVersionAcrossSolutionRuleSpec
  {
    static ISolution Solution;
    static IProject Project1;
    static IProject Project2;

    static NuGetPackagesShouldHaveOneVersionAcrossSolutionRule SUT;

    Establish ctx = () =>
    {
      Solution = A.Fake<ISolution>();

      Project1 = A.Fake<IProject>();
      Project2 = A.Fake<IProject>();
      A.CallTo(() => Solution.Projects).Returns(new[] { Project1, Project2 });

      var nuGetPackage = new NuGetPackage("Id", new Version(1, 0), false, null, "targetFramework", false);
      A.CallTo(() => Project1.NuGetPackages).Returns(new[] { nuGetPackage });
      A.CallTo(() => Project2.NuGetPackages).Returns(new[] { nuGetPackage });

      SUT = new NuGetPackagesShouldHaveOneVersionAcrossSolutionRule();
    };

    class when_no_NuGet_package_with_multiple_versions_is_there
    {
      Because of = () => Result = SUT.Evaluate(Solution);

      It returns_no_violations = () =>
          Result.Should().BeEmpty();

      static IEnumerable<IRuleViolation> Result;
    }

    class when_NuGet_package_with_multiple_versions_is_there
    {
      Establish ctx = () =>
      {
        var nuGetPackage = new NuGetPackage("Id", new Version(1, 1), false, null, "targetFramework", false);
        A.CallTo(() => Project2.NuGetPackages).Returns(new[] { nuGetPackage });
      };

      Because of = () => Result = SUT.Evaluate(Solution);

      It returns_violations = () =>
          Result.ShouldAllBeEquivalentTo(
              new RuleViolation(SUT, Solution, "The NuGet package 'Id' is referenced in more than one version ('1.0', '1.1')."));

      static IEnumerable<IRuleViolation> Result;
    }

    class when_NuGet_package_with_multiple_references_that_only_differ_in_the_prerelease_tag
    {
      Establish ctx = () =>
      {
        var nuGetPackage = new NuGetPackage("Id", new Version(1, 1), true, "-tag", "targetFramework", false);
        A.CallTo(() => Project2.NuGetPackages).Returns(new[] { nuGetPackage });
      };

      Because of = () => Result = SUT.Evaluate(Solution);

      It returns_violations = () =>
          Result.ShouldAllBeEquivalentTo(
              new RuleViolation(SUT, Solution, "The NuGet package 'Id' is referenced in more than one version ('1.0', '1.1-tag')."));

      static IEnumerable<IRuleViolation> Result;
    }
  }
}