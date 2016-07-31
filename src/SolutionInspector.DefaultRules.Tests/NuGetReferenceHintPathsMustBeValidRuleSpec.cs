using System;
using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;
using SolutionInspector.TestInfrastructure.AssertionExtensions;
using Wrapperator.Interfaces.IO;

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
  [Subject (typeof(NuGetReferenceHintPathsMustBeValidRule))]
  class NuGetReferenceHintPathsMustBeValidRuleSpec
  {
    static IFileInfo NuGetReferenceDllFile;
    static INuGetReference NuGetReference;
    static IProject Project;

    static NuGetReferenceHintPathsMustBeValidRule SUT;

    Establish ctx = () =>
    {
      Project = A.Fake<IProject>();

      NuGetReferenceDllFile = A.Fake<IFileInfo>();

      NuGetReference = A.Fake<INuGetReference>();
      A.CallTo(() => NuGetReference.DllFile).Returns(NuGetReferenceDllFile);
      A.CallTo(() => NuGetReference.HintPath).Returns("HintPath");
      A.CallTo(() => NuGetReference.Package.Id).Returns("Id");

      A.CallTo(() => Project.NuGetReferences).Returns(new[] { NuGetReference });

      SUT = new NuGetReferenceHintPathsMustBeValidRule();
    };

    class when_evaluating_and_all_NuGet_reference_hint_paths_are_valid
    {
      Establish ctx = () => { A.CallTo(() => NuGetReferenceDllFile.Exists).Returns(true); };

      Because of = () => Result = SUT.Evaluate(Project);

      It does_not_return_violations = () =>
          Result.Should().BeEmpty();

      static IEnumerable<IRuleViolation> Result;
    }

    class when_evaluating_and_a_NuGet_references_hint_path_is_invalid
    {
      Establish ctx = () => { A.CallTo(() => NuGetReferenceDllFile.Exists).Returns(false); };

      Because of = () => Result = SUT.Evaluate(Project);

      It returns_violation = () =>
          Result.ShouldAllBeLike(new RuleViolation(SUT, Project, "The NuGet reference to package 'Id' has an invalid hint path ('HintPath')."));

      static IEnumerable<IRuleViolation> Result;
    }
  }
}