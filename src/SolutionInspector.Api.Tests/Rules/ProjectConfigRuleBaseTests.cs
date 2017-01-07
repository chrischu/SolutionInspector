using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;

namespace SolutionInspector.Api.Tests.Rules
{
  public class ProjectConfigRuleBaseTests
  {
    private DummyProjectConfigurationRuleConfiguration _configuration;

    private IProject _project;
    private IRuleViolation _ruleViolation;

    private DummyProjectConfigRule _sut;

    [SetUp]
    public void SetUp ()
    {
      _configuration = new DummyProjectConfigurationRuleConfiguration();
      _ruleViolation = A.Fake<IRuleViolation>();

      _sut = new DummyProjectConfigRule(_configuration, _ruleViolation);

      _project = A.Fake<IProject>();
      A.CallTo(() => _project.Name).Returns("Project");
    }

    [Test]
    public void Evaluate ()
    {
      // ACT
      var result = _sut.Evaluate(_project).ToArray();

      // ASSERT
      result.Length.Should().Be(1);
      result[0].Should().BeSameAs(_ruleViolation);
    }

    [Test]
    public void Evaluate_ProjectWithoutConfiguration_AndReportViolationOnMissingConfigurationFileSetToTrue_ReportsViolation ()
    {
      _configuration.ReportViolationOnMissingConfigurationFile = true;
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
      _configuration.ReportViolationOnMissingConfigurationFile = false;
      A.CallTo(() => _project.ConfigurationProjectItem).Returns(null);

      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      result.Should().BeEmpty();
    }

    private class DummyProjectConfigRule : ProjectConfigRuleBase<DummyProjectConfigurationRuleConfiguration>
    {
      private readonly IRuleViolation _violation;

      public DummyProjectConfigRule (DummyProjectConfigurationRuleConfiguration configuration, IRuleViolation violation)
        : base(configuration)
      {
        _violation = violation;
      }

      protected override IEnumerable<IRuleViolation> Evaluate (
        IConfigurationProjectItem target,
        XDocument configurationXml)
      {
        yield return _violation;
      }
    }

    private class DummyProjectConfigurationRuleConfiguration : ProjectConfigRuleConfigurationBase
    {
      public DummyProjectConfigurationRuleConfiguration ()
      {
        ReportViolationOnMissingConfigurationFile = true;
      }
    }
  }
}