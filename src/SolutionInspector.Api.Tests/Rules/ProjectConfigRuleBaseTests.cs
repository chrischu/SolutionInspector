using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;
using SolutionInspector.TestInfrastructure.Api;

namespace SolutionInspector.Api.Tests.Rules
{
  public class ProjectConfigRuleBaseTests : RuleTestBase
  {
    private IProject _project;

    private DummyProjectConfigRule _sut;

    [SetUp]
    public void SetUp ()
    {
      _sut = CreateRule<DummyProjectConfigRule>();

      _project = A.Fake<IProject>();
      A.CallTo(() => _project.Name).Returns("Project");
    }

    [Test]
    public void Evaluate ()
    {
      // ACT
      var result = _sut.Evaluate(_project).ToArray();

      // ASSERT
      result.ShouldBeEquivalentTo(
          new[]
          {
              new RuleViolation(_sut, _project, "VIOLATION")
          });
    }

    [Test]
    public void Evaluate_ProjectWithoutConfiguration_AndReportViolationOnMissingConfigurationFileSetToTrue_ReportsViolation ()
    {
      _sut.ReportViolationOnMissingConfigurationFile = true;
      A.CallTo(() => _project.ConfigurationProjectItem).Returns(null);

      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      result.ShouldBeEquivalentTo(
          new[]
          {
              new RuleViolation(_sut, _project, "For the project 'Project' no configuration file could be found.")
          });
    }

    [Test]
    public void Evaluate_ProjectWithoutConfiguration_AndReportViolationOnMissingConfigurationFileSetToFalse_ReportsNoViolation ()
    {
      _sut.ReportViolationOnMissingConfigurationFile = false;
      A.CallTo(() => _project.ConfigurationProjectItem).Returns(null);

      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      result.Should().BeEmpty();
    }

    private class DummyProjectConfigRule : ProjectConfigRuleBase
    {
      protected override IEnumerable<IRuleViolation> Evaluate (
          IConfigurationProjectItem target,
          XDocument configurationXml)
      {
        yield return new RuleViolation(this, target, "VIOLATION");
      }
    }
  }
}