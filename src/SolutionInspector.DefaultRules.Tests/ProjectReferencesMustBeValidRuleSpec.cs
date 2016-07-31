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
  [Subject (typeof(ProjectReferencesMustBeValidRule))]
  class ProjectReferencesMustBeValidRuleSpec
  {
    static IProject Project;
    static IProjectReference ProjectReference;
    static Guid ProjectReferenceGuid;
    static ISolution Solution;

    static ProjectReferencesMustBeValidRule SUT;

    Establish ctx = () =>
    {
      Project = A.Fake<IProject>();

      ProjectReference = A.Fake<IProjectReference>();
      A.CallTo(() => Project.ProjectReferences).Returns(new[] { ProjectReference });

      ProjectReferenceGuid = Some.Guid;
      A.CallTo(() => ProjectReference.ReferencedProjectGuid).Returns(ProjectReferenceGuid);

      A.CallTo(() => ProjectReference.ReferencedProjectName).Returns("ReferencedProject");
      A.CallTo(() => ProjectReference.Include).Returns("ProjectReferenceInclude");

      Solution = A.Fake<ISolution>();
      A.CallTo(() => Project.Solution).Returns(Solution);

      SUT = new ProjectReferencesMustBeValidRule();
    };

    class when_evaluating_a_project_reference_with_invalid_guid
    {
      Establish ctx = () => { A.CallTo(() => ProjectReference.ReferencedProjectGuid).Returns(null); };

      Because of = () => Result = SUT.Evaluate(Project);

      It returns_violation = () =>
          Result.ShouldAllBeLike(
              new RuleViolation(
                  SUT,
                  Project,
                  "The reference to project 'ReferencedProject' ('ProjectReferenceInclude') is invalid because it does " +
                  "not specify the project GUID of the referenced project."));

      static IEnumerable<IRuleViolation> Result;
    }

    class when_evaluating_project_reference_with_non_existing_project_file_but_resolvable_guid
    {
      Establish ctx = () =>
      {
        A.CallTo(() => ProjectReference.Project).Returns(null);

        var projectMatchingReferencedProjectGuid = A.Fake<IProject>();
        A.CallTo(() => Solution.GetProjectByProjectGuid(ProjectReferenceGuid)).Returns(projectMatchingReferencedProjectGuid);
        A.CallTo(() => projectMatchingReferencedProjectGuid.Name).Returns("CorrectReferencedProject");
        A.CallTo(() => Project.GetIncludePathFor(projectMatchingReferencedProjectGuid)).Returns("CorrectProjectReferenceInclude");
      };

      Because of = () => Result = SUT.Evaluate(Project);

      It returns_violation = () =>
          Result.ShouldAllBeLike(
              new RuleViolation(
                  SUT,
                  Project,
                  "The reference to project 'ReferencedProject' ('ProjectReferenceInclude') is invalid because the " +
                  "referenced project file could not be found. However, there is a project that matches the given project guid: " +
                  "'CorrectReferencedProject' ('CorrectProjectReferenceInclude'). Did you mean to reference that one?"));

      static IEnumerable<IRuleViolation> Result;
    }

    class when_evaluating_project_reference_with_non_existing_project_file_and_nonresolvable_guid
    {
      Establish ctx = () =>
      {
        A.CallTo(() => ProjectReference.Project).Returns(null);
        A.CallTo(() => Solution.GetProjectByProjectGuid(ProjectReferenceGuid)).Returns(null);
      };

      Because of = () => Result = SUT.Evaluate(Project);

      It returns_violation = () =>
          Result.ShouldAllBeLike(
              new RuleViolation(
                  SUT,
                  Project,
                  "The reference to project 'ReferencedProject' ('ProjectReferenceInclude') is invalid because the " +
                  "referenced project file could not be found and the solution does not contain a project that at least matches the specified" +
                  $"referenced project GUID ({ProjectReferenceGuid})."));

      static IEnumerable<IRuleViolation> Result;
    }

    class when_evaluating_project_reference_with_existing_project_file_but_mismatching_guid
    {
      Establish ctx = () =>
      {
        ReferencedProject = A.Fake<IProject>();
        A.CallTo(() => ReferencedProject.Guid).Returns(Some.Guid);

        A.CallTo(() => ProjectReference.Project).Returns(ReferencedProject);
      };

      Because of = () => Result = SUT.Evaluate(Project);

      It returns_violation = () =>
          Result.ShouldAllBeLike(
              new RuleViolation(
                  SUT,
                  Project,
                  "The reference to project 'ReferencedProject' ('ProjectReferenceInclude') is invalid because the" +
                  $"specified project GUID for the referenced project ({ProjectReferenceGuid}) does not match the " +
                  $"project GUID of the actually referenced project ({ReferencedProject.Guid})."));

      static IProject ReferencedProject;
      static IEnumerable<IRuleViolation> Result;
    }

    class when_evaluating_valid_project_reference
    {
      Establish ctx = () =>
      {
        ReferencedProject = A.Fake<IProject>();
        A.CallTo(() => ReferencedProject.Guid).Returns(ProjectReferenceGuid);

        A.CallTo(() => ProjectReference.Project).Returns(ReferencedProject);
      };

      Because of = () => Result = SUT.Evaluate(Project);

      It does_not_return_violations = () =>
          Result.Should().BeEmpty();

      static IProject ReferencedProject;
      static IEnumerable<IRuleViolation> Result;
    }
  }
}