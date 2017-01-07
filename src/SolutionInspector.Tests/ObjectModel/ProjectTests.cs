using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Api.Configuration.MsBuildParsing;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Commons.Extensions;
using SolutionInspector.ObjectModel;
using SolutionInspector.TestInfrastructure.AssertionExtensions;
using Wrapperator.Wrappers.IO;

namespace SolutionInspector.Tests.ObjectModel
{
  public class ProjectTests
  {
    private string _solutionPath;
    private IMsBuildParsingConfiguration _msBuildParsingConfiguration;
    private Guid _guidOfEmptyProject;

    [SetUp]
    public void SetUp ()
    {
      _solutionPath = Path.Combine(
        Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath).AssertNotNull(),
        @"ObjectModel\TestData\Project\TestSolution.sln");

      _msBuildParsingConfiguration = A.Fake<IMsBuildParsingConfiguration>();
      A.CallTo(() => _msBuildParsingConfiguration.IsValidProjectItemType(A<string>._)).Returns(true);

      _guidOfEmptyProject = Guid.Parse("{61EF1F50-EA80-4164-AFF3-974C76A8D1E2}");
    }

    [Test]
    public void Load_EmptyProject_Works ()
    {
      var projectName = "EmptyProject";
      var projectPath = GetProjectPath(projectName);

      // ACT
      using (var result = LoadProject(projectName))
      {
        // ASSERT
        result.Should(
          "have project properties filled correctly",
          p =>
          {
            p.Guid.Should().Be(_guidOfEmptyProject);
            p.Name.Should().Be(projectName);
            p.AssemblyName.Should().Be(projectName);
            p.DefaultNamespace.Should().Be(projectName);
            var expectedXml = XDocument.Load(projectPath).ToString();
            p.ProjectXml.ToString().Should().Be(expectedXml);
            p.FolderName.Should().Be(projectName);
            p.ProjectFile.FullName.Should().Be(projectPath);
            p.OutputType.Should().Be(ProjectOutputType.Library);
            p.TargetFrameworkVersion.Should().Be(Version.Parse("4.6.1"));
            p.Identifier.Should().Be($"{projectName}.csproj");
            p.FullPath.Should().Be(projectPath);
            p.BuildConfigurations.ShouldAllBeLike(new BuildConfiguration("Debug", "AnyCPU"), new BuildConfiguration("Release", "AnyCPU"));
          });

        result.Should(
          "have solution and MsBuildProjectInSolution filled correctly",
          p =>
          {
            p.Advanced.MsBuildProjectInSolution.ProjectName.Should().Be(projectName);
            p.Solution.Name.Should().Be("TestSolution");
          });

        result.Should(
          "have unconditional property filled correctly",
          p =>
          {
            // We only need to check one exemplary property.
            p.Advanced.Properties["FileAlignment"].Should().BeLike(
              new ProjectProperty("FileAlignment", "512") { new ProjectPropertyOccurrence("512", null, new ProjectLocation(13, 5)) });
          });

        result.Should(
          "have build configuration-dependent properties filled correctly",
          p =>
          {
            const string propertyName = "DependentOnConfiguration";

            var occurrence = new ProjectPropertyOccurrence(
                               "LOL",
                               new ProjectPropertyCondition(" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ", null),
                               new ProjectLocation(24, 5));
            p.Advanced.Properties[propertyName].Should().BeLike(
              new ProjectProperty(propertyName, "LOL")
              {
                occurrence
              });

            var debugProperties = p.Advanced.EvaluateProperties(new BuildConfiguration("Debug", "AnyCPU"));
            debugProperties[propertyName].Should().BeLike(new EvaluatedProjectPropertyValue("LOL", occurrence));

            var releaseProperties = p.Advanced.EvaluateProperties(new BuildConfiguration("Release", "AnyCPU"));
            releaseProperties.Should().NotContainKey(propertyName);
          });

        result.Should(
          "have property-dependent property filled correctly",
          p =>
          {
            const string propertyName = "DependentOnProperty";

            var occurrence = new ProjectPropertyOccurrence(
                               "ROFL",
                               new ProjectPropertyCondition(" '$(Property)' == 'true' ", null),
                               new ProjectLocation(25, 5));

            p.Advanced.Properties[propertyName].Should().BeLike(new ProjectProperty(propertyName, "") { occurrence });

            var propertiesBasedOnTrueCondition =
                p.Advanced.EvaluateProperties(new Dictionary<string, string> { { "Property", "true" } });
            propertiesBasedOnTrueCondition[propertyName].Should().BeLike(new EvaluatedProjectPropertyValue("ROFL", occurrence));

            var propertiesBasedOnFalseCondition =
                p.Advanced.EvaluateProperties(new Dictionary<string, string> { { "Property", "false" } });
            propertiesBasedOnFalseCondition.Should().NotContainKey(propertyName);
          });

        result.Should(
          "have property without condition but with conditional parent filled correctly",
          p =>
          {
            const string propertyName = "ConditionFromParent";

            var occurrence = new ProjectPropertyOccurrence(
                               "QWER",
                               new ProjectPropertyCondition(null, " '$(Parent)' == 'true' "),
                               new ProjectLocation(28, 5));

            p.Advanced.Properties[propertyName].Should().BeLike(
              new ProjectProperty(propertyName, "") { occurrence });

            var propertiesBasedOnTrueCondition =
                p.Advanced.EvaluateProperties(new Dictionary<string, string> { { "Parent", "true" } });
            propertiesBasedOnTrueCondition[propertyName].Should().BeLike(new EvaluatedProjectPropertyValue("QWER", occurrence));

            var propertiesBasedOnFalseCondition =
                p.Advanced.EvaluateProperties(new Dictionary<string, string> { { "Parent", "false" } });
            propertiesBasedOnFalseCondition.Should().NotContainKey(propertyName);
          });

        result.Should(
          "have property with condition and with conditional parent filled correctly",
          p =>
          {
            const string propertyName = "ConditionFromSelf";

            var occurrence = new ProjectPropertyOccurrence(
                               "ASDF",
                               new ProjectPropertyCondition(" '$(Self)' == 'true' ", " '$(Parent)' == 'true' "),
                               new ProjectLocation(29, 5));

            p.Advanced.Properties[propertyName].Should().BeLike(new ProjectProperty(propertyName, "") { occurrence });

            var propertiesBasedOnTrueCondition =
                p.Advanced.EvaluateProperties(new Dictionary<string, string> { { "Parent", "true" }, { "Self", "true" } });
            propertiesBasedOnTrueCondition[propertyName].Should().BeLike(new EvaluatedProjectPropertyValue("ASDF", occurrence));

            var propertiesBasedOnFalseCondition1 =
                p.Advanced.EvaluateProperties(new Dictionary<string, string> { { "Parent", "false" } });
            propertiesBasedOnFalseCondition1.Should().NotContainKey(propertyName);

            var propertiesBasedOnFalseCondition2 =
                p.Advanced.EvaluateProperties(new Dictionary<string, string> { { "Parent", "true" }, { "Self", "false" } });
            propertiesBasedOnFalseCondition2.Should().NotContainKey(propertyName);
          });

        result.Should(
          "have property that is contained more than once with different conditions filled correctly",
          p =>
          {
            const string propertyName = "Multiple";

            var occurrenceWithThree = new ProjectPropertyOccurrence(
                                        "Three",
                                        new ProjectPropertyCondition(" '$(Value)' == '3'", null),
                                        new ProjectLocation(32, 5));

            var occurrenceWithFive = new ProjectPropertyOccurrence(
                                       "Five",
                                       new ProjectPropertyCondition(" '$(Value)' == '5'", null),
                                       new ProjectLocation(33, 5));

            p.Advanced.Properties[propertyName].Should().BeLike(
              new ProjectProperty(propertyName, "") { occurrenceWithThree, occurrenceWithFive });

            var propertiesBasedOnFirstValue =
                p.Advanced.EvaluateProperties(new Dictionary<string, string> { { "Value", "3" } });
            propertiesBasedOnFirstValue[propertyName].Should().BeLike(new EvaluatedProjectPropertyValue("Three", occurrenceWithThree));

            var propertiesBasedOnSecondValue =
                p.Advanced.EvaluateProperties(new Dictionary<string, string> { { "Value", "5" } });
            propertiesBasedOnSecondValue[propertyName].Should().BeLike(new EvaluatedProjectPropertyValue("Five", occurrenceWithFive));
          });

        result.Should(
          "have duplicate property filled correctly",
          p =>
          {
            const string propertyName = "Duplicate";

            p.Advanced.Properties[propertyName].Should().BeLike(
              new ProjectProperty(propertyName, "2")
              {
                new ProjectPropertyOccurrence("1", null, new ProjectLocation(20, 5)),
                new ProjectPropertyOccurrence("2", null, new ProjectLocation(21, 5))
              });
          });
      }
    }

    [Test]
    public void Load_ExecutableProject_Works ()
    {
      var projectName = "ExecutableProject";
      var projectPath = GetProjectPath(projectName);

      // ACT
      using (var result = LoadProject(projectName))
      {
        // ASSERT
        result.Name.Should().Be(projectName);
        result.OutputType.Should().Be(ProjectOutputType.Executable);

        var appConfigPath = Path.Combine(Path.GetDirectoryName(projectPath).AssertNotNull(), "App.config");
        var expectedAppConfigContent = XDocument.Load(appConfigPath).ToString();
        result.ConfigurationProjectItem.ConfigurationXml.ToString().Should().Be(expectedAppConfigContent);
      }
    }

    [Test]
    public void Load_ProjectWithReferences_Works ()
    {
      var projectName = "ProjectWithReferences";
      var projectPath = GetProjectPath(projectName);

      var referencedNuGetPackage1 = new NuGetPackage("Newtonsoft.Json", new Version(8, 0, 3), false, null, "net452", isDevelopmentDependency: false);
      var referencedNuGetPackage2 = new NuGetPackage("Dapper", new Version(1, 50, 0), true, "-beta9", "net452", isDevelopmentDependency: true);

      // ACT
      using (var result = LoadProject(projectName))
      {
        // ASSERT
        result.Should(
          "have GAC references",
          p => p.GacReferences.Single().Should().BeLike(new GacReference(new AssemblyName("System"))));

        result.Should(
          "have file references",
          p =>
          {
            p.FileReferences.ShouldBeEquivalentTo(
              new[]
              {
                new FileReference(new AssemblyName("Dummy"), ".\\Dummy.dll", p.ProjectDirectory.FullName)
              },
              options =>
                options.ExcludingMissingMembers()
                    .IgnoringCyclicReferences()
                    .RespectingRuntimeTypes()
                    .Using<FileInfoWrapper>(ctx => ctx.Subject.FullName.Should().Be(ctx.Expectation.FullName)).WhenTypeIs<FileInfoWrapper>()
                    .Using<DirectoryInfoWrapper>(ctx => ctx.Subject.FullName.Should().Be(ctx.Expectation.FullName)).WhenTypeIs<DirectoryInfoWrapper>());
          });

        result.Should(
          "have NuGet references",
          p =>
          {
            var privateReference = p.NuGetReferences.Single(r => r.IsPrivate);
            privateReference.Should().BeLike(
              new NuGetReference(
                referencedNuGetPackage1,
                new AssemblyName("Newtonsoft.Json, Version=8.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed, processorArchitecture=MSIL"),
                isPrivate: true,
                hintPath: "..\\packages\\Newtonsoft.Json.8.0.3\\lib\\net45\\Newtonsoft.Json.dll",
                projectDirectory: p.ProjectDirectory.FullName));

            var publicReference = p.NuGetReferences.Single(r => !r.IsPrivate);
            publicReference.Should().BeLike(new { IsPrivate = false });
          });

        result.Should(
          "have NuGet packages",
          p =>
          {
            var packagesConfigPath = Path.Combine(Path.GetDirectoryName(projectPath).AssertNotNull(), "packages.config");
            p.NuGetPackagesFile.FullName.Should().Be(packagesConfigPath);

            p.NuGetPackages.ShouldAllBeLike(referencedNuGetPackage1, referencedNuGetPackage2);
          });

        result.Should(
          "have valid project reference",
          p =>
          {
            var reference = p.ProjectReferences.Single(r => r.ReferencedProjectName == "EmptyProject");

            reference.Project.Name.Should().Be("EmptyProject");
            reference.ReferencedProjectName.Should().Be("EmptyProject");
            reference.ReferencedProjectGuid.Should().Be(_guidOfEmptyProject);
            reference.Include.Should().Be("..\\EmptyProject\\EmptyProject.csproj");
            reference.File.FullName.Should().Be(GetProjectPath("EmptyProject"));
            reference.OriginalProjectItem.Should().NotBeNull();
          });

        result.Should(
          "have project reference with invalid project GUID",
          p =>
          {
            var reference = p.ProjectReferences.Single(r => r.ReferencedProjectName == "InvalidGuid");

            reference.Project.Name.Should().Be("EmptyProject");
            reference.ReferencedProjectName.Should().Be("InvalidGuid");
            reference.ReferencedProjectGuid.Should().Be(Guid.Empty);
            reference.Include.Should().Be("..\\EmptyProject\\EmptyProject.csproj");
            reference.File.FullName.Should().Be(GetProjectPath("EmptyProject"));
            reference.OriginalProjectItem.Should().NotBeNull();
          });

        result.Should(
          "have reference with invalid include",
          p =>
          {
            var reference = p.ProjectReferences.Single(r => r.ReferencedProjectName == "InvalidInclude");

            reference.Project.Should().BeNull();
            reference.ReferencedProjectName.Should().Be("InvalidInclude");
            reference.ReferencedProjectGuid.Should().Be(_guidOfEmptyProject);
            reference.Include.Should().Be("..\\WrongInclude\\WrongInclude.csproj");
            reference.File.FullName.Should().Be(GetProjectPath("WrongInclude"));
            reference.OriginalProjectItem.Should().NotBeNull();
          });
      }
    }

    [Test]
    public void Dispose_UnloadsProjects ()
    {
      // ACT
      var project = LoadProject("EmptyProject");

      // ASSERT
      project.Advanced.MsBuildProject.ProjectCollection.LoadedProjects.Should().HaveCount(1);

      // ACT
      project.Dispose();

      // ASSERT
      project.Advanced.MsBuildProject.ProjectCollection.LoadedProjects.Should().BeEmpty();
    }

    [Test]
    public void GetIncludePathFor_ReturnsRelativePath ()
    {
      var referencingProject = LoadProject("EmptyProject");
      var projectToReference = LoadProject("ExecutableProject");

      // ACT
      var result = referencingProject.GetIncludePathFor(projectToReference);

      // ASSERT
      result.Should().Be("..\\ExecutableProject\\ExecutableProject.csproj");
    }

    private IProject LoadProject (string projectName)
    {
      var solution = Solution.Load(_solutionPath, _msBuildParsingConfiguration);

      return solution.Projects.Single(p => p.Name == projectName);
    }

    private string GetProjectPath (string projectName)
    {
      return Path.Combine(Path.GetDirectoryName(_solutionPath).AssertNotNull(), $"{projectName}\\{projectName}.csproj");
    }
  }
}