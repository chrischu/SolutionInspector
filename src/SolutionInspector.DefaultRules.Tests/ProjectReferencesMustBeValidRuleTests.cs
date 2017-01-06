using System;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;
using SolutionInspector.TestInfrastructure;

namespace SolutionInspector.DefaultRules.Tests
{
  public class ProjectReferencesMustBeValidRuleTests
  {
    private IProject _project;
    private IProjectReference _projectReference;
    private Guid _projectReferenceGuid;
    private ISolution _solution;

    private ProjectReferencesMustBeValidRule _sut;

    [SetUp]
    public void SetUp ()
    {
      _project = A.Fake<IProject>();

      _projectReference = A.Fake<IProjectReference>();
      A.CallTo(() => _project.ProjectReferences).Returns(new[] { _projectReference });

      _projectReferenceGuid = Some.Guid;
      A.CallTo(() => _projectReference.ReferencedProjectGuid).Returns(_projectReferenceGuid);

      A.CallTo(() => _projectReference.ReferencedProjectName).Returns("ReferencedProject");
      A.CallTo(() => _projectReference.Include).Returns("ProjectReferenceInclude");

      _solution = A.Fake<ISolution>();
      A.CallTo(() => _project.Solution).Returns(_solution);

      _sut = new ProjectReferencesMustBeValidRule();
    }

    [Test]
    public void Evaluate_ValidProjectReference_ReturnsNoViolations ()
    {
      var referencedProject = A.Fake<IProject>();
      A.CallTo(() => referencedProject.Guid).Returns(_projectReferenceGuid);

      A.CallTo(() => _projectReference.Project).Returns(referencedProject);

      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      result.Should().BeEmpty();
    }

    [Test]
    public void Evaluate_ProjectReferenceWithInvalidGuid_ReturnsViolation ()
    {
      A.CallTo(() => _projectReference.ReferencedProjectGuid).Returns(null);

      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      result.ShouldBeEquivalentTo(
        new[]
        {
          new RuleViolation(
            _sut,
            _project,
            "The reference to project 'ReferencedProject' ('ProjectReferenceInclude') is invalid because it does " +
            "not specify the project GUID of the referenced project.")
        });
    }

    [Test]
    public void Evaluate_ProjectReferenceWithNonExistingProjectFileButResolvableGuid_ReturnsViolation ()
    {
      A.CallTo(() => _projectReference.Project).Returns(null);

      var projectMatchingReferencedProjectGuid = A.Fake<IProject>();
      A.CallTo(() => _solution.GetProjectByProjectGuid(_projectReferenceGuid)).Returns(projectMatchingReferencedProjectGuid);
      A.CallTo(() => projectMatchingReferencedProjectGuid.Name).Returns("CorrectReferencedProject");
      A.CallTo(() => _project.GetIncludePathFor(projectMatchingReferencedProjectGuid)).Returns("CorrectProjectReferenceInclude");

      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      result.ShouldBeEquivalentTo(
        new[]
        {
          new RuleViolation(
            _sut,
            _project,
            "The reference to project 'ReferencedProject' ('ProjectReferenceInclude') is invalid because the " +
            "referenced project file could not be found. However, there is a project that matches the given project guid: " +
            "'CorrectReferencedProject' ('CorrectProjectReferenceInclude'). Did you mean to reference that one?")
        });
    }

    [Test]
    public void Evaluate_ProjectReferenceWithNonExistingProjectFileAndNonresolvableGuid_ReturnsViolation ()
    {
      A.CallTo(() => _projectReference.Project).Returns(null);
      A.CallTo(() => _solution.GetProjectByProjectGuid(_projectReferenceGuid)).Returns(null);

      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      result.ShouldBeEquivalentTo(
        new[]
        {
          new RuleViolation(
            _sut,
            _project,
            "The reference to project 'ReferencedProject' ('ProjectReferenceInclude') is invalid because the " +
            "referenced project file could not be found and the solution does not contain a project that at least matches the specified" +
            $"referenced project GUID ({_projectReferenceGuid}).")
        });
    }

    [Test]
    public void Evaluate_ProjectReferenceWithExistingProjectFileButMismatchingGuid_ReturnsViolation ()
    {
      var referencedProject = A.Fake<IProject>();
      A.CallTo(() => referencedProject.Guid).Returns(Some.Guid);

      A.CallTo(() => _projectReference.Project).Returns(referencedProject);

      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      result.ShouldBeEquivalentTo(
        new[]
        {
          new RuleViolation(
            _sut,
            _project,
            "The reference to project 'ReferencedProject' ('ProjectReferenceInclude') is invalid because the" +
            $"specified project GUID for the referenced project ({_projectReferenceGuid}) does not match the " +
            $"project GUID of the actually referenced project ({referencedProject.Guid}).")
        });
    }
  }
}