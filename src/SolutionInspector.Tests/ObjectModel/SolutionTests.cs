using System;
using System.IO;
using System.Linq;
using System.Reflection;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Commons.Extensions;
using SolutionInspector.Configuration.MsBuildParsing;
using SolutionInspector.ObjectModel;
using SolutionInspector.TestInfrastructure.AssertionExtensions;

namespace SolutionInspector.Tests.ObjectModel
{
  public class SolutionTests
  {
    static string _solutionPath;
    static IMsBuildParsingConfiguration _msBuildParsingConfiguration;

    [SetUp]
    public void SetUp ()
    {
      _solutionPath = Path.Combine(
        Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath).AssertNotNull(),
        @"ObjectModel\TestData\Solution\TestSolution.sln");

      _msBuildParsingConfiguration = A.Dummy<IMsBuildParsingConfiguration>();
    }

    [Test]
    public void Load ()
    {
      // ACT
      using (ISolution result = Solution.Load(_solutionPath, _msBuildParsingConfiguration))
      {
        // ASSERT
        result.Name.Should().Be("TestSolution");
        result.SolutionDirectory.FullName.Should().Be(Path.GetDirectoryName(_solutionPath));
        result.BuildConfigurations.ShouldAllBeLike(new BuildConfiguration("Debug", "Any CPU"), new BuildConfiguration("Release", "Any CPU"));
        result.Projects.Single().Name.Should().Be("EmptyProject");
        result.Identifier.Should().Be("TestSolution.sln");
        result.FullPath.Should().Be(_solutionPath);
      }
    }

    [Test]
    public void Dispose_UnloadsAllProjects ()
    {
      // ACT
      var solution = Solution.Load(_solutionPath, _msBuildParsingConfiguration);

      // ASSERT
      solution.Projects.All(p => p.Advanced.MsBuildProject.ProjectCollection.LoadedProjects.Count > 0).Should().BeTrue();

      // ACT
      solution.Dispose();

      // ASSERT
      solution.Projects.All(p => p.Advanced.MsBuildProject.ProjectCollection.LoadedProjects.Count == 0).Should().BeTrue();
    }

    [Test]
    public void GetProjectByProjectGuid_WithExistingGuid_ReturnsProject ()
    {
      var solution = Solution.Load(_solutionPath, _msBuildParsingConfiguration);

      var projectGuid = Guid.Parse("82E31753-8314-4C92-953C-0178F923C9C1");

      // ACT
      var project = solution.GetProjectByProjectGuid(projectGuid);

      // ASSERT
      project.Should().NotBeNull();
      project.AssertNotNull().Guid.Should().Be(projectGuid);
    }

    [Test]
    public void GetProjectByProjectGuid_WithNonExistingGuid_ReturnsNull ()
    {
      var solution = Solution.Load(_solutionPath, _msBuildParsingConfiguration);

      // ACT
      var project = solution.GetProjectByProjectGuid(Guid.Empty);

      // ASSERT
      project.Should().BeNull();
    }

    [Test]
    public void GetProjectByAbsoluteProjectFilePath_WithExistingProject_ReturnsProject ()
    {
      var solution = Solution.Load(_solutionPath, _msBuildParsingConfiguration);
      var projectPath = Path.Combine(solution.SolutionDirectory.FullName, "EmptyProject.csproj");

      // ACT
      var project = solution.GetProjectByAbsoluteProjectFilePath(projectPath);

      // ASSERT
      project.Should().NotBeNull();
      project.AssertNotNull().ProjectFile.FullName.Should().Be(projectPath);
    }

    [Test]
    public void GetProjectByAbsoluteProjectFilePath_WithNonExistingProject_ReturnsNull ()
    {
      var solution = Solution.Load(_solutionPath, _msBuildParsingConfiguration);
      var projectPath = Path.Combine(solution.SolutionDirectory.FullName, "DoesNotExist.csproj");

      // ACT
      var project = solution.GetProjectByAbsoluteProjectFilePath(projectPath);

      // ASSERT
      project.Should().BeNull();
    }
  }
}