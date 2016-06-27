using System;
using System.Collections.Generic;
using SystemInterface.IO;
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
  [Subject (typeof(NonDevelopmentNuGetReferencesShouldHaveIsPrivateSetToTrueRule))]
  class NonDevelopmentNuGetReferencesShouldHaveIsPrivateSetToTrueRuleSpec
  {
    static IFileInfo NuGetReferenceDllFile;
    static INuGetReference NuGetReference;
    static IProject Project;

    static NonDevelopmentNuGetReferencesShouldHaveIsPrivateSetToTrueRule SUT;

    Establish ctx = () =>
    {
      Project = A.Fake<IProject>();

      NuGetReferenceDllFile = A.Fake<IFileInfo>();

      NuGetReference = A.Fake<INuGetReference>();
      A.CallTo(() => NuGetReference.Package.Id).Returns("Id");

      A.CallTo(() => Project.NuGetReferences).Returns(new[] { NuGetReference });

      SUT = new NonDevelopmentNuGetReferencesShouldHaveIsPrivateSetToTrueRule();
    };

    class when_evaluating_and_NuGet_reference_is_development_dependency
    {
      Establish ctx = () =>
      {
        A.CallTo(() => NuGetReference.IsPrivate).Returns(Some.Boolean);
        A.CallTo(() => NuGetReference.Package.IsDevelopmentDependency).Returns(true);
      };

      Because of = () => Result = SUT.Evaluate(Project);

      It does_not_return_violations = () =>
          Result.Should().BeEmpty();

      static IEnumerable<IRuleViolation> Result;
    }

    class when_evaluating_and_NuGet_reference_is_non_development_dependency_but_has_IsPrivate_set_to_true
    {
      Establish ctx = () =>
      {
        A.CallTo(() => NuGetReference.IsPrivate).Returns(true);
        A.CallTo(() => NuGetReference.Package.IsDevelopmentDependency).Returns(false);
      };

      Because of = () => Result = SUT.Evaluate(Project);

      It does_not_return_violations = () =>
          Result.Should().BeEmpty();

      static IEnumerable<IRuleViolation> Result;
    }

    class when_evaluating_and_NuGet_reference_is_non_development_dependency_and_has_IsPrivate_set_to_false
    {
      Establish ctx = () =>
      {
        A.CallTo(() => NuGetReference.IsPrivate).Returns(false);
        A.CallTo(() => NuGetReference.Package.IsDevelopmentDependency).Returns(false);
      };

      Because of = () => Result = SUT.Evaluate(Project);

      It returns_violation = () =>
          Result.ShouldAllBeLike(
              new RuleViolation(
                  SUT,
                  Project,
                  "The NuGet reference to package 'Id' is not a development dependency and therefore should has its 'IsPrivate' flag set to true."));

      static IEnumerable<IRuleViolation> Result;
    }
  }
}