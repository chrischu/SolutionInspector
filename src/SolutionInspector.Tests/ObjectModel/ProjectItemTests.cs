using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Api.Configuration.MsBuildParsing;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Commons.Extensions;
using SolutionInspector.ObjectModel;

namespace SolutionInspector.Tests.ObjectModel
{
  public class ProjectItemTests
  {
    private IMsBuildParsingConfiguration _msBuildParsingConfiguration;
    private string _solutionPath;

    [SetUp]
    public void SetUp ()
    {
      _solutionPath = Path.Combine(
        Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath).AssertNotNull(),
        @"ObjectModel\TestData\ProjectItem\TestSolution.sln");

      _msBuildParsingConfiguration = A.Fake<IMsBuildParsingConfiguration>();
      A.CallTo(() => _msBuildParsingConfiguration.IsValidProjectItemType(A<string>._)).Returns(true);
    }

    [Test]
    public void Load_DirectItem_Works ()
    {
      var projectItemName = "Direct.cs";
      var projectItemPath = GetProjectItemPath(projectItemName);

      // ACT
      var result = LoadProjectItems(projectItemName).Single();

      // ASSERT
      result.Name.Should().Be(projectItemName);
      result.Include.Should().Be(new ProjectItemInclude(projectItemName, projectItemName));
      result.BuildAction.Should().Be(ProjectItemBuildAction.Compile);
      result.File.FullName.Should().Be(projectItemPath);
      result.Metadata["CustomMetadata"].Should().Be("SomeCustomMetadata");
      result.CustomTool.Should().Be("SomeCustomTool");
      result.CustomToolNamespace.Should().Be("SomeCustomToolNamespace");
      result.Parent.Should().BeNull();
      result.Children.Should().BeEmpty();
      result.Identifier.Should().Be($"Project.csproj/{projectItemName}");
      result.Location.Should().Be(new ProjectLocation(54, 5));
      result.FullPath.Should().Be(projectItemPath);
    }

    [Test]
    public void Load_NestedItem_Works ()
    {
      var projectItemName = "Nested.Designer.cs";
      var include = $"Folder\\{projectItemName}";
      var projectItemPath = GetProjectItemPath(include);

      // ACT
      var result = LoadProjectItems(projectItemName).Single();

      // ASSERT
      result.Name.Should().Be(projectItemName);
      result.Include.Should().Be(new ProjectItemInclude(include, include));
      result.BuildAction.Should().Be(ProjectItemBuildAction.Compile);
      result.File.FullName.Should().Be(projectItemPath);
      result.Parent.Name.Should().Be("Nested.resx");
      result.Children.Should().BeEmpty();
      result.Identifier.Should().Be($"Project.csproj/Folder/Nested.resx/{projectItemName}");
      result.Location.Should().Be(new ProjectLocation(59, 5));
      result.FullPath.Should().Be(projectItemPath);
    }

    [Test]
    public void Load_ParentItem_Works ()
    {
      var projectItemName = "Nested.resx";
      var include = $"Folder\\{projectItemName}";
      var projectItemPath = GetProjectItemPath(include);

      // ACT
      var result = LoadProjectItems(projectItemName).Single();

      // ASSERT
      result.Name.Should().Be(projectItemName);
      result.Include.Should().Be(new ProjectItemInclude(include, include));
      result.BuildAction.Should().Be(ProjectItemBuildAction.EmbeddedResource);
      result.File.FullName.Should().Be(projectItemPath);
      result.Parent.Should().BeNull();
      result.Children.Single().Name.Should().Be("Nested.Designer.cs");
      result.Identifier.Should().Be($"Project.csproj/Folder/{projectItemName}");
      result.Location.Should().Be(new ProjectLocation(45, 5));
      result.FullPath.Should().Be(projectItemPath);
    }

    [Test]
    public void Load_LinkedItem_Works ()
    {
      var projectItemName = "Link.cs";
      var projectItemPath = Path.Combine(Path.GetDirectoryName(_solutionPath).AssertNotNull(), projectItemName);

      // ACT
      var result = LoadProjectItems(projectItemName).Single();

      // ASSERT
      result.Name.Should().Be(projectItemName);
      result.Include.Should().Be(new ProjectItemInclude("..\\Link.cs", "..\\Link.cs"));
      result.BuildAction.Should().Be(ProjectItemBuildAction.Compile);
      result.File.FullName.Should().Be(projectItemPath);
      result.Parent.Should().BeNull();
      result.Children.Should().BeEmpty();
      result.Identifier.Should().Be($"Project.csproj/{projectItemName}");
      result.Location.Should().Be(new ProjectLocation(51, 5));
      result.FullPath.Should().Be(projectItemPath);
    }

    [Test]
    public void Load_DuplicateItem_ReturnsBoth ()
    {
      var projectItemName = "Duplicate.cs";

      // ACT
      var result = LoadProjectItems(projectItemName).ToArray();

      // ASSERT
      result.Should().HaveCount(2);
    }

    [Test]
    public void Load_ItemWithDuplicateNameButDifferences_ReturnsBoth ()
    {
      var projectItemName = "AlmostDuplicate.cs";

      // ACT
      var result = LoadProjectItems(projectItemName).ToArray();

      // ASSERT
      result.Should().HaveCount(2);
      result[0].Include.Evaluated.Should().Be("AlmostDuplicate.cs");
      result[0].Metadata["Metadata"].Should().Be("3");

      result[1].Include.Evaluated.Should().Be("AlmostDuplicate.cs");
      result[1].Metadata["Metadata"].Should().Be("5");
    }

    [Test]
    public void Load_ItemIncludedByWildcard_LoadsOnlyIncludedItems ()
    {
      var projectItemName = "IncludedByWildcard.cs";

      // ACT
      var result = LoadProjectItems(projectItemName).Single();

      // ASSERT
      result.Include.Evaluated.Should().Be("Wildcard\\IncludedByWildcard.cs");
      result.Include.Unevaluated.Should().Be("Wildcard\\*.cs;Wildcard2\\*.cs");
      result.IsIncludedByWildcard.Should().BeTrue();
      result.WildcardInclude.Should().Be("Wildcard\\*.cs;Wildcard2\\*.cs");
      result.WildcardExclude.Should().Be("Wildcard\\Excluded.cs;Wildcard2\\Excluded2.cs");
      result.Metadata["Metadata"].Should().Be("SomeMetadata");

      LoadProjectItems("Exclude.cs").Should().BeEmpty();
      LoadProjectItems("Exclude2.cs").Should().BeEmpty();
    }

    [Test]
    public void Load_ItemIncludedByWildcardAndNormally_LoadsBoth ()
    {
      var projectItemName = "IncludedByWildcardAndNormally.cs";

      // ACT
      var result = LoadProjectItems(projectItemName).ToArray();

      // ASSERT
      result[0].Include.Evaluated.Should().Be("Wildcard\\IncludedByWildcardAndNormally.cs");
      result[0].Include.Unevaluated.Should().Be("Wildcard\\*.cs;Wildcard2\\*.cs");
      result[0].IsIncludedByWildcard.Should().BeTrue();
      result[0].WildcardInclude.Should().Be("Wildcard\\*.cs;Wildcard2\\*.cs");
      result[0].WildcardExclude.Should().Be("Wildcard\\Excluded.cs;Wildcard2\\Excluded2.cs");
      result[0].Metadata["Metadata"].Should().Be("SomeMetadata");

      result[1].Include.Evaluated.Should().Be("Wildcard\\IncludedByWildcardAndNormally.cs");
      result[1].Include.Unevaluated.Should().Be("Wildcard\\IncludedByWildcardAndNormally.cs");
      result[1].IsIncludedByWildcard.Should().BeFalse();
      result[1].WildcardInclude.Should().BeNull();
      result[1].WildcardExclude.Should().BeNull();
      result[1].Metadata.GetValueOrDefault("Metadata").Should().BeNull();

      LoadProjectItems("Exclude.cs").Should().BeEmpty();
      LoadProjectItems("Exclude2.cs").Should().BeEmpty();
    }

    private IEnumerable<IProjectItem> LoadProjectItems (string itemName)
    {
      var solution = Solution.Load(_solutionPath, _msBuildParsingConfiguration);
      var project = solution.Projects.Single();

      return project.ProjectItems.Where(i => i.Name == itemName);
    }

    private string GetProjectItemPath (string itemName)
    {
      return Path.Combine(Path.GetDirectoryName(_solutionPath).AssertNotNull(), $"Project\\{itemName}");
    }
  }
}