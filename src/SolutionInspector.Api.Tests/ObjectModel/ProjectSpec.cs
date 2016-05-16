using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
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
  [Subject (typeof (Project))]
  class ProjectSpec
  {
    static string SolutionPath;
    static IMsBuildParsingConfiguration MsBuildParsingConfiguration;

    Establish ctx = () =>
    {
      SolutionPath = Path.Combine(
          Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath).AssertNotNull(),
          @"ObjectModel\TestData\Project\TestSolution.sln");

      MsBuildParsingConfiguration = A.Fake<IMsBuildParsingConfiguration>();
      A.CallTo(() => MsBuildParsingConfiguration.IsValidProjectItemType(A<string>._)).Returns(true);
    };

    class when_loading_empty_project
    {
      Establish ctx = () => { ProjectName = "EmptyProject"; };

      Because of = () => Result = LoadProject(ProjectName);

      It parses_project = () =>
      {
        var projectPath = GetProjectPath(ProjectName);

        Result.Name.Should().Be(ProjectName);
        Result.AssemblyName.Should().Be(ProjectName);
        Result.DefaultNamespace.Should().Be(ProjectName);
        var expectedXml = XDocument.Load(projectPath).ToString();
        Result.ProjectXml.ToString().Should().Be(expectedXml);
        Result.FolderName.Should().Be(ProjectName);
        Result.ProjectFile.FullName.Should().Be(projectPath);
        Result.OutputType.Should().Be(ProjectOutputType.Library);
        Result.TargetFrameworkVersion.Should().Be(Version.Parse("4.6.1"));
        Result.Identifier.Should().Be($"{ProjectName}.csproj");
        Result.FullPath.Should().Be(projectPath);
        Result.BuildConfigurations.ShouldAllBeEquivalentTo(
            new[]
            {
                new BuildConfiguration("Debug", "AnyCPU"),
                new BuildConfiguration("Release", "AnyCPU")
            });
      };

      It parses_unconditional_properties = () =>
          Result.Advanced.Properties.GetPropertyValueOrNull("FileAlignment").Should().Be("512");

      It parses_build_configuration_dependent_properties = () =>
      {
        const string propertyName = "DependentOnConfiguration";

        Result.Advanced.ConditionalProperties.Single(p => p.Name == propertyName).ShouldBeEquivalentTo(
            new
            {
                Name = propertyName,
                Value = "LOL",
                Condition = " '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ",
                ParentCondition = (string) null
            });

        Result.Advanced.GetPropertiesBasedOnCondition(new BuildConfiguration("Debug", "AnyCPU"))[propertyName].Value.Should().Be("LOL");

        Result.Advanced.GetPropertiesBasedOnCondition(new BuildConfiguration("Release", "AnyCPU"))
            .GetValueOrDefault(propertyName)
            .Should()
            .BeNull();
      };

      It parses_property_dependent_properties = () =>
      {
        const string propertyName = "DependentOnProperty";

        Result.Advanced.ConditionalProperties.Single(p => p.Name == propertyName).ShouldBeEquivalentTo(
            new
            {
                Name = propertyName,
                Value = "ROFL",
                Condition = " '$(Property)' == 'true' ",
                ParentCondition = (string) null
            });

        var propertiesBasedOnTrueCondition =
            Result.Advanced.GetPropertiesBasedOnCondition(new Dictionary<string, string> { { "Property", "true" } });
        propertiesBasedOnTrueCondition[propertyName].Value.Should().Be("ROFL");

        var propertiesBasedOnFalseCondition =
            Result.Advanced.GetPropertiesBasedOnCondition(new Dictionary<string, string> { { "Property", "false" } });
        propertiesBasedOnFalseCondition.GetValueOrDefault(propertyName).Should().BeNull();
      };

      It parses_property_without_condition_but_with_conditional_parent = () =>
      {
        const string propertyName = "ConditionFromParent";

        Result.Advanced.ConditionalProperties.Single(p => p.Name == propertyName).ShouldBeEquivalentTo(
            new
            {
                Name = propertyName,
                Value = "QWER",
                Condition = (string) null,
                ParentCondition = " '$(Parent)' == 'true' "
            });

        var propertiesBasedOnTrueCondition =
            Result.Advanced.GetPropertiesBasedOnCondition(new Dictionary<string, string> { { "Parent", "true" } });
        propertiesBasedOnTrueCondition[propertyName].Value.Should().Be("QWER");

        var propertiesBasedOnFalseCondition =
            Result.Advanced.GetPropertiesBasedOnCondition(new Dictionary<string, string> { { "Parent", "false" } });
        propertiesBasedOnFalseCondition.GetValueOrDefault(propertyName).Should().BeNull();
      };

      It parses_property_with_condition_and_with_conditional_parent = () =>
      {
        const string propertyName = "ConditionFromSelf";

        Result.Advanced.ConditionalProperties.Single(p => p.Name == propertyName).ShouldBeEquivalentTo(
            new
            {
                Name = propertyName,
                Value = "ASDF",
                Condition = " '$(Self)' == 'true' ",
                ParentCondition = " '$(Parent)' == 'true' "
            });

        var propertiesBasedOnTrueCondition =
            Result.Advanced.GetPropertiesBasedOnCondition(new Dictionary<string, string> { { "Parent", "true" }, { "Self", "true" } });
        propertiesBasedOnTrueCondition[propertyName].Value.Should().Be("ASDF");

        var propertiesBasedOnFalseCondition1 =
            Result.Advanced.GetPropertiesBasedOnCondition(new Dictionary<string, string> { { "Parent", "false" } });
        propertiesBasedOnFalseCondition1.GetValueOrDefault(propertyName).Should().BeNull();

        var propertiesBasedOnFalseCondition2 =
            Result.Advanced.GetPropertiesBasedOnCondition(new Dictionary<string, string> { { "Parent", "true" }, { "Self", "false" } });
        propertiesBasedOnFalseCondition2.GetValueOrDefault(propertyName).Should().BeNull();
      };

      static string ProjectName;
      static IProject Result;
    }

    class when_loading_executable_project
    {
      Establish ctx = () => { ProjectName = "ExecutableProject"; };

      Because of = () => Result = LoadProject(ProjectName);

      It parses_project = () =>
      {
        var projectPath = GetProjectPath(ProjectName);

        Result.Name.Should().Be(ProjectName);
        Result.OutputType.Should().Be(ProjectOutputType.Executable);

        var appConfigPath = Path.Combine(Path.GetDirectoryName(projectPath).AssertNotNull(), "App.config");
        var expectedAppConfigContent = XDocument.Load(appConfigPath).ToString();
        Result.ConfigurationProjectItem.ConfigurationXml.ToString().Should().Be(expectedAppConfigContent);
      };

      static string ProjectName;
      static IProject Result;
    }

    class when_loading_project_with_references
    {
      Establish ctx = () =>
      {
        ProjectName = "ProjectWithReferences";
        ReferencedNuGetPackage1 = new NuGetPackage("Newtonsoft.Json", new Version(8, 0, 3), false, null, "net461", isDevelopmentDependency: false);
        ReferencedNuGetPackage2 = new NuGetPackage("Dapper", new Version(1, 50, 0), true, "-beta9", "net461", isDevelopmentDependency: true);
      };

      Because of = () => Result = LoadProject(ProjectName);

      It parses_gac_references = () =>
          Result.GacReferences.Single().ShouldBeEquivalentTo(new GacReference(new AssemblyName("System")));

      It parses_file_references = () =>
          Result.FileReferences.Single().ShouldBeEquivalentTo(new FileReference(new AssemblyName("Dummy"), ".\\Dummy.dll"));

      It parses_NuGet_references = () =>
      {
        var privateReference = Result.NuGetReferences.Single(r => r.IsPrivate);
        privateReference.ShouldBeEquivalentTo(
            new NuGetReference(
                ReferencedNuGetPackage1,
                new AssemblyName("Newtonsoft.Json, Version=8.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed, processorArchitecture=MSIL"),
                isPrivate: true,
                hintPath: "..\\packages\\Newtonsoft.Json.8.0.3\\lib\\net45\\Newtonsoft.Json.dll"));

        var publicReference = Result.NuGetReferences.Single(r => !r.IsPrivate);
        publicReference.ShouldBeEquivalentTo(new { IsPrivate = false }, c => c.ExcludingMissingMembers());
      };

      It parses_NuGet_packages = () =>
      {
        var projectPath = GetProjectPath(ProjectName);
        var packagesConfigPath = Path.Combine(Path.GetDirectoryName(projectPath).AssertNotNull(), "packages.config");
        Result.NuGetPackagesFile.FullName.Should().Be(packagesConfigPath);

        Result.NuGetPackages.ShouldAllBeEquivalentTo(new[] { ReferencedNuGetPackage1, ReferencedNuGetPackage2 });
      };

      It parses_project_references = () =>
          Result.ProjectReferences.Single().Project.Name.Should().Be("EmptyProject");

      static string ProjectName;
      static NuGetPackage ReferencedNuGetPackage1;
      static NuGetPackage ReferencedNuGetPackage2;
      static IProject Result;
    }

    static IProject LoadProject (string projectName)
    {
      var solution = Solution.Load(SolutionPath, MsBuildParsingConfiguration);

      return solution.Projects.Single(p => p.Name == projectName);
    }

    static string GetProjectPath (string projectName)
    {
      return Path.Combine(Path.GetDirectoryName(SolutionPath).AssertNotNull(), $"{projectName}\\{projectName}.csproj");
    }
  }
}