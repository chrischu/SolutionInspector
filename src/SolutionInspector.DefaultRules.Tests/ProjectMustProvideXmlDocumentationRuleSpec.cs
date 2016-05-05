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
  [Subject (typeof (ProjectMustProvideXmlDocumentationRule))]
  class ProjectMustProvideXmlDocumentationRuleSpec
  {
    static string ProjectName;
    static IProject Project;
    static IAdvancedProject AdvancedProject;

    static BuildConfiguration BuildConfiguration;


    static ProjectMustProvideXmlDocumentationRule SUT;

    Establish ctx = () =>
    {
      Project = A.Fake<IProject>();

      ProjectName = "Project";
      A.CallTo(() => Project.Name).Returns(ProjectName);

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
        A.CallTo(() => AdvancedProject.ConfigurationDependentProperties[BuildConfiguration]).Returns(
            new Dictionary<string, string>
            {
                { "OutputPath", "outputPath\\" },
                { "DocumentationFile", $"outputPath\\{ProjectName}.XML" }
            });
      };

      Because of = () => Result = SUT.Evaluate(Project);

      It does_not_return_violations = () =>
          Result.Should().BeEmpty();

      static IEnumerable<IRuleViolation> Result;
    }

    class when_xml_documentation_configuration_is_missing
    {
      Establish ctx = () =>
      {
        A.CallTo(() => AdvancedProject.ConfigurationDependentProperties[BuildConfiguration]).Returns(
            new Dictionary<string, string>());
      };

      Because of = () => Result = SUT.Evaluate(Project);

      It returns_violation = () =>
          Result.ShouldAllBeEquivalentTo(
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
        A.CallTo(() => AdvancedProject.ConfigurationDependentProperties[BuildConfiguration]).Returns(
            new Dictionary<string, string>
            {
                { "OutputPath", "outputPath\\" },
                { "DocumentationFile", "NOT_EXPECTED" }
            });
      };

      Because of = () => Result = SUT.Evaluate(Project);

      It returns_violation = () =>
          Result.ShouldAllBeEquivalentTo(
              new RuleViolation(
                  SUT,
                  Project,
                  $"In the build configuration '{BuildConfiguration}' the XML documentation configuration " +
                  "is invalid (was: \'NOT_EXPECTED\', expected: \'outputPath\\Project.XML\')."));

      static IEnumerable<IRuleViolation> Result;
    }
  }
}