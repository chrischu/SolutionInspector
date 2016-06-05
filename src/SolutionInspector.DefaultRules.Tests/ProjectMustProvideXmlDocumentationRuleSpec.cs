using System;
using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;
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
  [Subject (typeof(ProjectMustProvideXmlDocumentationRule))]
  class ProjectMustProvideXmlDocumentationRuleSpec
  {
    static string ProjectAssemblyName;
    static IProject Project;
    static IAdvancedProject AdvancedProject;

    static BuildConfiguration BuildConfiguration;


    static ProjectMustProvideXmlDocumentationRule SUT;

    Establish ctx = () =>
    {
      Project = A.Fake<IProject>();

      ProjectAssemblyName = "Project.dll";
      A.CallTo(() => Project.AssemblyName).Returns(ProjectAssemblyName);

      AdvancedProject = A.Fake<IAdvancedProject>();
      A.CallTo(() => Project.Advanced).Returns(AdvancedProject);

      BuildConfiguration = new BuildConfiguration("Configuration", "Platform");
      A.CallTo(() => Project.BuildConfigurations).Returns(new[] { BuildConfiguration });

      SUT = new ProjectMustProvideXmlDocumentationRule();
    };

    class when_xml_documentation_configuration_is_correct
    {
      Establish ctx = () =>
      {
        ProjectPropertyFakeUtility.SetupFakeBuildConfigurationDependentProperties(
            AdvancedProject,
            BuildConfiguration,
            new Dictionary<string, string>
            {
                { "OutputPath", "outputPath\\" },
                { "DocumentationFile", $"outputPath\\{ProjectAssemblyName}.XML" }
            });
      };

      Because of = () => Result = SUT.Evaluate(Project);

      It does_not_return_violations = () =>
          Result.Should().BeEmpty();

      static IEnumerable<IRuleViolation> Result;
    }

    class when_xml_documentation_differs_only_in_casing
    {
      Establish ctx = () =>
      {
        ProjectPropertyFakeUtility.SetupFakeBuildConfigurationDependentProperties(
            AdvancedProject,
            BuildConfiguration,
            new Dictionary<string, string>
            {
                { "OutputPath", "OUTPUTPATH\\" },
                { "DocumentationFile", $"OUTPUTPATH\\{ProjectAssemblyName}.xml" }
            });
      };

      Because of = () => Result = SUT.Evaluate(Project);

      It does_not_return_violations = () =>
          Result.Should().BeEmpty();

      static IEnumerable<IRuleViolation> Result;
    }

    class when_xml_documentation_configuration_is_missing
    {
      Establish ctx = () => { ProjectPropertyFakeUtility.SetupEmptyBuildConfigurationDependentProperties(AdvancedProject, BuildConfiguration); };

      Because of = () => Result = SUT.Evaluate(Project);

      It returns_violation = () =>
          Result.ShouldAllBeLike(
              new RuleViolation(
                  SUT,
                  Project,
                  $"In the build configuration '{BuildConfiguration}' the XML documentation configuration is missing."));

      static IEnumerable<IRuleViolation> Result;
    }

    class when_xml_documentation_configuration_is_there_but_has_an_unexpected_value
    {
      Establish ctx = () =>
      {
        ProjectPropertyFakeUtility.SetupFakeBuildConfigurationDependentProperties(
            AdvancedProject,
            BuildConfiguration,
            new Dictionary<string, string>
            {
                { "OutputPath", "outputPath\\" },
                { "DocumentationFile", "NOT_EXPECTED" }
            });
      };

      Because of = () => Result = SUT.Evaluate(Project);

      It returns_violation = () =>
          Result.ShouldAllBeLike(
              new RuleViolation(
                  SUT,
                  Project,
                  $"In the build configuration '{BuildConfiguration}' the XML documentation configuration " +
                  $"is invalid (was: \'NOT_EXPECTED\', expected: \'outputPath\\{Project.AssemblyName}.XML\')."));

      static IEnumerable<IRuleViolation> Result;
    }
  }
}