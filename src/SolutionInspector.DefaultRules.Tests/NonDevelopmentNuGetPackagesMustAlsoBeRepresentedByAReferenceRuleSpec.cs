using System;
using System.Collections.Generic;
using System.Reflection;
using FakeItEasy;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.ObjectModel;
using SolutionInspector.Rules;
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
  [Subject(typeof (NonDevelopmentNuGetPackagesMustAlsoBeRepresentedByAReferenceRule))]
  class NonDevelopmentNuGetPackagesMustAlsoBeRepresentedByAReferenceRuleSpec
  {
    static IProject Project;

    static NonDevelopmentNuGetPackagesMustAlsoBeRepresentedByAReferenceRule SUT;

    Establish ctx = () =>
    {
      Project = A.Fake<IProject>();

      var nuGetPackage = new NuGetPackage("package", new Version(1, 0), false, null, "targetFramework", false);
      A.CallTo(() => Project.NuGetPackages).Returns(new[] { nuGetPackage });
      A.CallTo(() => Project.NuGetReferences).Returns(new[] { new NuGetReference(nuGetPackage, new AssemblyName(), false, "") });

      SUT = new NonDevelopmentNuGetPackagesMustAlsoBeRepresentedByAReferenceRule();
    };

    class when_all_NuGet_packages_have_a_corresponding_reference
    {
      Because of = () => Result = SUT.Evaluate(Project);

      It does_not_return_violations = () =>
          Result.Should().BeEmpty();

      static IEnumerable<IRuleViolation> Result;
    }

    class when_a_NuGet_package_does_not_have_a_corresponding_reference
    {
      Establish ctx = () =>
      {
        A.CallTo(() => Project.NuGetReferences).Returns(new NuGetReference[0]);
      };

      Because of = () => Result = SUT.Evaluate(Project);

      It returns_violation = () =>
          Result.ShouldAllBeEquivalentTo(
              new RuleViolation(SUT, Project, "For the NuGet package 'package.1.0', no DLL reference could be found."));

      static IEnumerable<IRuleViolation> Result;
    }

    class when_only_a_development_NuGet_package_does_not_have_a_corresponding_reference
    {
      Establish ctx = () =>
      {
        var nuGetPackage = new NuGetPackage("package", new Version(1, 0), false, null, "targetFramework", true);
        A.CallTo(() => Project.NuGetPackages).Returns(new[] { nuGetPackage });
        A.CallTo(() => Project.NuGetReferences).Returns(new NuGetReference[0]);
      };

      Because of = () => Result = SUT.Evaluate(Project);

      It does_not_return_violations = () =>
          Result.Should().BeEmpty();

      static IEnumerable<IRuleViolation> Result;
    }
  }
}