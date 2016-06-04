using System;
using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;

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
  [Subject (typeof (RequiredNuGetPackagesRule))]
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
            A.CallTo(() => Project.NuGetPackages).Returns(new[] { new NuGetPackage("Package", Version.Parse("0.0.1"), false, "", "net461", false) });
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
          Result.ShouldAllBeEquivalentTo(
              new RuleViolation(SUT, Project, "Required NuGet package 'Package' is missing."));

      static IEnumerable<IRuleViolation> Result;
    }
  }
}