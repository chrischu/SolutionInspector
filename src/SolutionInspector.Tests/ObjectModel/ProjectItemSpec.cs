using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FakeItEasy;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Commons.Extensions;
using SolutionInspector.Configuration.MsBuildParsing;
using SolutionInspector.ObjectModel;

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
  [Subject (typeof(ProjectItem))]
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

      Because of = () => Result = LoadProjectItems(ProjectItemName).Single();

      It parses_project_item = () =>
      {
        var projectItemPath = GetProjectItemPath(ProjectItemName);

        Result.Name.Should().Be(ProjectItemName);
        Result.Include.Should().Be(new ProjectItemInclude(ProjectItemName, ProjectItemName));
        Result.BuildAction.Should().Be(ProjectItemBuildAction.Compile);
        Result.File.FullName.Should().Be(projectItemPath);
        Result.Metadata["CustomMetadata"].Should().Be("SomeCustomMetadata");
        Result.CustomTool.Should().Be("SomeCustomTool");
        Result.CustomToolNamespace.Should().Be("SomeCustomToolNamespace");
        Result.Parent.Should().BeNull();
        Result.Children.Should().BeEmpty();
        Result.Identifier.Should().Be($"Project.csproj/{ProjectItemName}");
        Result.Location.Should().Be(new ProjectLocation(54, 5));
        Result.FullPath.Should().Be(projectItemPath);
      };

      static string ProjectItemName;
      static IProjectItem Result;
    }

    class when_loading_nested_project_item
    {
      Establish ctx = () => { ProjectItemName = "Nested.Designer.cs"; };

      Because of = () => Result = LoadProjectItems(ProjectItemName).Single();

      It parses_project_item = () =>
      {
        var include = $"Folder\\{ProjectItemName}";
        var projectItemPath = GetProjectItemPath(include);

        Result.Name.Should().Be(ProjectItemName);
        Result.Include.Should().Be(new ProjectItemInclude(include, include));
        Result.BuildAction.Should().Be(ProjectItemBuildAction.Compile);
        Result.File.FullName.Should().Be(projectItemPath);
        Result.Parent.Name.Should().Be("Nested.resx");
        Result.Children.Should().BeEmpty();
        Result.Identifier.Should().Be($"Project.csproj/Folder/Nested.resx/{ProjectItemName}");
        Result.Location.Should().Be(new ProjectLocation(59, 5));
        Result.FullPath.Should().Be(projectItemPath);
      };

      static string ProjectItemName;
      static IProjectItem Result;
    }

    class when_loading_parent_project_item
    {
      Establish ctx = () => { ProjectItemName = "Nested.resx"; };

      Because of = () => Result = LoadProjectItems(ProjectItemName).Single();

      It parses_project_item = () =>
      {
        var include = $"Folder\\{ProjectItemName}";
        var projectItemPath = GetProjectItemPath(include);

        Result.Name.Should().Be(ProjectItemName);
        Result.Include.Should().Be(new ProjectItemInclude(include, include));
        Result.BuildAction.Should().Be(ProjectItemBuildAction.EmbeddedResource);
        Result.File.FullName.Should().Be(projectItemPath);
        Result.Parent.Should().BeNull();
        Result.Children.Single().Name.Should().Be("Nested.Designer.cs");
        Result.Identifier.Should().Be($"Project.csproj/Folder/{ProjectItemName}");
        Result.Location.Should().Be(new ProjectLocation(45, 5));
        Result.FullPath.Should().Be(projectItemPath);
      };

      static string ProjectItemName;
      static IProjectItem Result;
    }

    class when_loading_linked_project_item
    {
      Establish ctx = () => { ProjectItemName = "Link.cs"; };

      Because of = () => Result = LoadProjectItems(ProjectItemName).Single();

      It parses_project_item = () =>
      {
        var projectItemPath = Path.Combine(Path.GetDirectoryName(SolutionPath).AssertNotNull(), ProjectItemName);

        Result.Name.Should().Be(ProjectItemName);
        Result.Include.Should().Be(new ProjectItemInclude("..\\Link.cs", "..\\Link.cs"));
        Result.BuildAction.Should().Be(ProjectItemBuildAction.Compile);
        Result.File.FullName.Should().Be(projectItemPath);
        Result.Parent.Should().BeNull();
        Result.Children.Should().BeEmpty();
        Result.Identifier.Should().Be($"Project.csproj/{ProjectItemName}");
        Result.Location.Should().Be(new ProjectLocation(51, 5));
        Result.FullPath.Should().Be(projectItemPath);
      };

      static string ProjectItemName;
      static IProjectItem Result;
    }

    class when_loading_a_duplicate_project_item
    {
      Establish ctx = () => { ProjectItemName = "Duplicate.cs"; };

      Because of = () => Result = LoadProjectItems(ProjectItemName).ToArray();

      It returns_both = () =>
          Result.Should().HaveCount(2);

      static string ProjectItemName;
      static IProjectItem[] Result;
    }

    class when_loading_a_project_item_with_duplicate_name_but_differing_somehow
    {
      Establish ctx = () => { ProjectItemName = "AlmostDuplicate.cs"; };

      Because of = () => Result = LoadProjectItems(ProjectItemName).ToArray();

      It parses_both_items = () =>
      {
        Result[0].Include.Evaluated.Should().Be("AlmostDuplicate.cs");
        Result[0].Metadata["Metadata"].Should().Be("3");

        Result[1].Include.Evaluated.Should().Be("AlmostDuplicate.cs");
        Result[1].Metadata["Metadata"].Should().Be("5");
      };

      static string ProjectItemName;
      static IProjectItem[] Result;
    }

    class when_loading_a_project_item_included_by_wildcard
    {
      Establish ctx = () => { ProjectItemName = "IncludedByWildcard.cs"; };

      Because of = () => Result = LoadProjectItems(ProjectItemName).Single();

      It parses_project_item = () =>
      {
        Result.Include.Evaluated.Should().Be("Wildcard\\IncludedByWildcard.cs");
        Result.Include.Unevaluated.Should().Be("Wildcard\\*.cs;Wildcard2\\*.cs");
        Result.IsIncludedByWildcard.Should().BeTrue();
        Result.WildcardInclude.Should().Be("Wildcard\\*.cs;Wildcard2\\*.cs");
        Result.WildcardExclude.Should().Be("Wildcard\\Excluded.cs;Wildcard2\\Excluded2.cs");
        Result.Metadata["Metadata"].Should().Be("SomeMetadata");
      };

      It does_not_load_excluded_items = () =>
      {
        LoadProjectItems("Exclude.cs").Should().BeEmpty();
        LoadProjectItems("Exclude2.cs").Should().BeEmpty();
      };

      static string ProjectItemName;
      static IProjectItem Result;
    }

    class when_loading_a_project_item_included_by_wildcard_and_normally
    {
      Establish ctx = () => { ProjectItemName = "IncludedByWildcardAndNormally.cs"; };

      Because of = () => Result = LoadProjectItems(ProjectItemName).ToArray();

      It parses_both_items = () =>
      {
        Result[0].Include.Evaluated.Should().Be("Wildcard\\IncludedByWildcardAndNormally.cs");
        Result[0].Include.Unevaluated.Should().Be("Wildcard\\*.cs;Wildcard2\\*.cs");
        Result[0].IsIncludedByWildcard.Should().BeTrue();
        Result[0].WildcardInclude.Should().Be("Wildcard\\*.cs;Wildcard2\\*.cs");
        Result[0].WildcardExclude.Should().Be("Wildcard\\Excluded.cs;Wildcard2\\Excluded2.cs");
        Result[0].Metadata["Metadata"].Should().Be("SomeMetadata");

        Result[1].Include.Evaluated.Should().Be("Wildcard\\IncludedByWildcardAndNormally.cs");
        Result[1].Include.Unevaluated.Should().Be("Wildcard\\IncludedByWildcardAndNormally.cs");
        Result[1].IsIncludedByWildcard.Should().BeFalse();
        Result[1].WildcardInclude.Should().BeNull();
        Result[1].WildcardExclude.Should().BeNull();
        Result[1].Metadata.GetValueOrDefault("Metadata").Should().BeNull();
      };

      static string ProjectItemName;
      static IProjectItem[] Result;
    }

    static IEnumerable<IProjectItem> LoadProjectItems (string itemName)
    {
      var solution = Solution.Load(SolutionPath, MsBuildParsingConfiguration);
      var project = solution.Projects.Single();

      return project.ProjectItems.Where(i => i.Name == itemName);
    }

    static string GetProjectItemPath (string itemName)
    {
      return Path.Combine(Path.GetDirectoryName(SolutionPath).AssertNotNull(), $"Project\\{itemName}");
    }
  }
}