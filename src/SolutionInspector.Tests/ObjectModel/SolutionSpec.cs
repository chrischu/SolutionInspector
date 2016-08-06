using System;
using System.IO;
using System.Linq;
using System.Reflection;
using FakeItEasy;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.Api.Configuration.MsBuildParsing;
using SolutionInspector.Api.Extensions;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.ObjectModel;
using SolutionInspector.TestInfrastructure.AssertionExtensions;

#region R# preamble for Machine.Specifications files

// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable ClassNeverInstantiated.Local
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Local
// ReSharper disable NotAccessedField.Local
// ReSharper disable StaticMemberInGenericType
// ReSharper disable UnassignedField.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable UnassignedGetOnlyAutoProperty

#endregion

namespace SolutionInspector.Tests.ObjectModel
{
  [Subject (typeof(Solution))]
  class SolutionSpec
  {
    static string SolutionPath;
    static IMsBuildParsingConfiguration MsBuildParsingConfiguration;

    Establish ctx = () =>
    {
      SolutionPath = Path.Combine(
          Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath).AssertNotNull(),
          @"ObjectModel\TestData\Solution\TestSolution.sln");

      MsBuildParsingConfiguration = A.Dummy<IMsBuildParsingConfiguration>();
    };

    class when_loading
    {
      Because of = () => Result = Solution.Load(SolutionPath, MsBuildParsingConfiguration);

      It parses_solution = () =>
      {
        Result.Name.Should().Be("TestSolution");
        Result.SolutionDirectory.FullName.Should().Be(Path.GetDirectoryName(SolutionPath));
        Result.BuildConfigurations.ShouldAllBeLike(new BuildConfiguration("Debug", "Any CPU"), new BuildConfiguration("Release", "Any CPU"));
        Result.Projects.Single().Name.Should().Be("EmptyProject");
        Result.Identifier.Should().Be("TestSolution.sln");
        Result.FullPath.Should().Be(SolutionPath);
      };

      static ISolution Result;
    }

    class when_loading_and_disposing
    {
      Establish ctx = () => { Solution = global::SolutionInspector.ObjectModel.Solution.Load(SolutionPath, MsBuildParsingConfiguration); };

      Because of = () => Solution.Dispose();

      It disposes_all_projects = () =>
          Solution.Projects.All(p => p.Advanced.MsBuildProject.ProjectCollection.LoadedProjects.Count == 0).Should().BeTrue();

      static ISolution Solution;
    }

    class when_getting_project_by_project_guid
    {
      Establish ctx = () => { Solution = global::SolutionInspector.ObjectModel.Solution.Load(SolutionPath, MsBuildParsingConfiguration); };

      Because of = () =>
      {
        /* Tests are in the its */
      };

      It returns_project_for_existing_guid = () =>
          Solution.GetProjectByProjectGuid(Guid.Parse("82E31753-8314-4C92-953C-0178F923C9C1")).Should().NotBeNull();

      It returns_null_for_nonexisting_guid = () =>
          Solution.GetProjectByProjectGuid(Guid.Empty).Should().BeNull();

      static ISolution Solution;
    }

    class when_getting_project_by_absolute_file_path
    {
      Establish ctx = () => { Solution = global::SolutionInspector.ObjectModel.Solution.Load(SolutionPath, MsBuildParsingConfiguration); };

      Because of = () =>
      {
        /* Tests are in the its */
      };

      It returns_project_for_existing_path = () =>
          Solution.GetProjectByAbsoluteProjectFilePath(Path.Combine(Solution.SolutionDirectory.FullName, "EmptyProject.csproj"))
              .Should()
              .NotBeNull();

      It returns_null_for_nonexisting_path = () =>
          Solution.GetProjectByAbsoluteProjectFilePath(Path.Combine(Solution.SolutionDirectory.FullName, "DoesNotExist.csproj"))
              .Should()
              .BeNull();

      static ISolution Solution;
    }
  }
}