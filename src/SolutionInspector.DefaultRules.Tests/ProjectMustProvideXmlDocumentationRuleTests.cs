using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;

namespace SolutionInspector.DefaultRules.Tests
{
  public class ProjectMustProvideXmlDocumentationRuleTests
  {
    private IAdvancedProject _advancedProject;
    private BuildConfiguration _buildConfiguration;
    private IProject _project;
    private string _projectAssemblyName;

    private ProjectMustProvideXmlDocumentationRule _sut;

    [SetUp]
    public void SetUp ()
    {
      _project = A.Fake<IProject>();

      _projectAssemblyName = "Project.dll";
      A.CallTo(() => _project.AssemblyName).Returns(_projectAssemblyName);

      _advancedProject = A.Fake<IAdvancedProject>();
      A.CallTo(() => _project.Advanced).Returns(_advancedProject);

      _buildConfiguration = new BuildConfiguration("Configuration", "Platform");
      A.CallTo(() => _project.BuildConfigurations).Returns(new[] { _buildConfiguration });

      _sut = new ProjectMustProvideXmlDocumentationRule();
    }

    [Test]
    public void Evaluate_XmlDocumentationConfigurationIsAsExpected_ReturnsNoViolations ()
    {
      ProjectPropertyFakeUtility.SetupFakeBuildConfigurationDependentProperties(
        _advancedProject,
        _buildConfiguration,
        new Dictionary<string, string>
        {
          { "OutputPath", "outputPath\\" },
          { "DocumentationFile", $"outputPath\\{_projectAssemblyName}.XML" }
        });

      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      result.Should().BeEmpty();
    }

    [Test]
    public void Evaluate_XmlDocumentationConfigurationDiffersOnlyInCasing_ReturnsNoViolations ()
    {
      ProjectPropertyFakeUtility.SetupFakeBuildConfigurationDependentProperties(
        _advancedProject,
        _buildConfiguration,
        new Dictionary<string, string>
        {
          { "OutputPath", "OUTPUTPATH\\" },
          { "DocumentationFile", $"OUTPUTPATH\\{_projectAssemblyName}.xml" }
        });

      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      result.Should().BeEmpty();
    }

    [Test]
    public void Evaluate_XmlDocumentationConfigurationIsMissing_ReturnsViolation ()
    {
      ProjectPropertyFakeUtility.SetupEmptyBuildConfigurationDependentProperties(_advancedProject, _buildConfiguration);

      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      result.ShouldBeEquivalentTo(
        new[]
        {
          new RuleViolation(
            _sut,
            _project,
            $"In the build configuration '{_buildConfiguration}' the XML documentation configuration is missing.")
        });
    }

    [Test]
    public void Evaluate_XmlDocumentationConfigurationHasUnexpectedValues_ReturnsViolation ()
    {
      ProjectPropertyFakeUtility.SetupFakeBuildConfigurationDependentProperties(
        _advancedProject,
        _buildConfiguration,
        new Dictionary<string, string>
        {
          { "OutputPath", "outputPath\\" },
          { "DocumentationFile", "NOT_EXPECTED" }
        });

      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      result.ShouldBeEquivalentTo(
        new[]
        {
          new RuleViolation(
            _sut,
            _project,
            $"In the build configuration '{_buildConfiguration}' the XML documentation configuration " +
            $"is invalid (was: \'NOT_EXPECTED\', expected: \'outputPath\\{_project.AssemblyName}.XML\').")
        });
    }
  }
}