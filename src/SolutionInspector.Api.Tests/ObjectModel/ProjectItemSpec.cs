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

namespace SolutionInspector.Api.Tests.ObjectModel
{
  [Subject (typeof (ProjectItem))]
  class ProjectItemSpec
  {
    static string SolutionPath;
    static IMsBuildParsingConfiguration MsBuildParsingConfiguration;

    Establish ctx = () =>
    {
      SolutionPath = Path.Combine(
          Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath).AssertNotNull(),
          @"ObjectModel\TestData\ProjectItem\TestSolution.sln");

      MsBuildParsingConfiguration = A.Fake<IMsBuildParsingConfiguration>();
      A.CallTo(() => MsBuildParsingConfiguration.IsValidProjectItemType(A<string>._)).Returns(true);
    };

    class when_loading
    {
      Establish ctx = () => { ProjectItemName = "Direct.cs"; };

      Because of = () => Result = LoadProjectItem(ProjectItemName);

      It parses_project_item = () =>
      {
        var projectItemPath = GetProjectItemPath(ProjectItemName);

        Result.Name.Should().Be(ProjectItemName);
        Result.OriginalInclude.Should().Be(ProjectItemName);
        Result.Include.Should().Be(ProjectItemName);
        Result.BuildAction.Should().Be(ProjectItemBuildAction.Compile);
        Result.File.FullName.Should().Be(projectItemPath);
        Result.Metadata["CustomMetadata"].Should().Be("SomeCustomMetadata");
        Result.CustomTool.Should().Be("SomeCustomTool");
        Result.CustomToolNamespace.Should().Be("SomeCustomToolNamespace");
        Result.Parent.Should().BeNull();
        Result.Children.Should().BeEmpty();
        Result.Identifier.Should().Be($"Project.csproj/{ProjectItemName}");
        Result.FullPath.Should().Be(projectItemPath);
      };

      static string ProjectItemName;
      static IProjectItem Result;
    }

    class when_loading_nested_project_item
    {
      Establish ctx = () => { ProjectItemName = "Nested.Designer.cs"; };

      Because of = () => Result = LoadProjectItem(ProjectItemName);

      It parses_project_item = () =>
      {
        var include = $"Folder\\{ProjectItemName}";
        var projectItemPath = GetProjectItemPath(include);

        Result.Name.Should().Be(ProjectItemName);
        Result.OriginalInclude.Should().Be(include);
        Result.Include.Should().Be(include);
        Result.BuildAction.Should().Be(ProjectItemBuildAction.Compile);
        Result.File.FullName.Should().Be(projectItemPath);
        Result.Parent.Name.Should().Be("Nested.resx");
        Result.Children.Should().BeEmpty();
        Result.Identifier.Should().Be($"Project.csproj/Folder/Nested.resx/{ProjectItemName}");
        Result.FullPath.Should().Be(projectItemPath);
      };

      static string ProjectItemName;
      static IProjectItem Result;
    }

    class when_loading_parent_project_item
    {
      Establish ctx = () => { ProjectItemName = "Nested.resx"; };

      Because of = () => Result = LoadProjectItem(ProjectItemName);

      It parses_project_item = () =>
      {
        var include = $"Folder\\{ProjectItemName}";
        var projectItemPath = GetProjectItemPath(include);

        Result.Name.Should().Be(ProjectItemName);
        Result.OriginalInclude.Should().Be(include);
        Result.Include.Should().Be(include);
        Result.BuildAction.Should().Be(ProjectItemBuildAction.EmbeddedResource);
        Result.File.FullName.Should().Be(projectItemPath);
        Result.Parent.Should().BeNull();
        Result.Children.Single().Name.Should().Be("Nested.Designer.cs");
        Result.Identifier.Should().Be($"Project.csproj/Folder/{ProjectItemName}");
        Result.FullPath.Should().Be(projectItemPath);
      };

      static string ProjectItemName;
      static IProjectItem Result;
    }

    class when_loading_linked_project_item
    {
      Establish ctx = () => { ProjectItemName = "Link.cs"; };

      Because of = () => Result = LoadProjectItem(ProjectItemName);

      It parses_project_item = () =>
      {
        var include = ProjectItemName;
        var projectItemPath = Path.Combine(Path.GetDirectoryName(SolutionPath).AssertNotNull(), ProjectItemName);

        Result.Name.Should().Be(ProjectItemName);
        Result.OriginalInclude.Should().Be("..\\Link.cs");
        Result.Include.Should().Be(include);
        Result.BuildAction.Should().Be(ProjectItemBuildAction.Compile);
        Result.File.FullName.Should().Be(projectItemPath);
        Result.Parent.Should().BeNull();
        Result.Children.Should().BeEmpty();
        Result.Identifier.Should().Be($"Project.csproj/{ProjectItemName}");
        Result.FullPath.Should().Be(projectItemPath);
      };

      static string ProjectItemName;
      static IProjectItem Result;
    }

    static IProjectItem LoadProjectItem (string itemName)
    {
      var solution = Solution.Load(SolutionPath, MsBuildParsingConfiguration);
      var project = solution.Projects.Single();

      return project.ProjectItems.Single(i => i.Name == itemName);
    }

    static string GetProjectItemPath (string itemName)
    {
      return Path.Combine(Path.GetDirectoryName(SolutionPath).AssertNotNull(), $"Project\\{itemName}");
    }
  }
}