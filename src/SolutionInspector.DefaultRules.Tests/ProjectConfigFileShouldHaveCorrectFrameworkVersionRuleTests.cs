using System.Xml.Linq;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;
using SolutionInspector.Configuration;

namespace SolutionInspector.DefaultRules.Tests
{
  public class ProjectConfigFileShouldHaveCorrectFrameworkVersionRuleTests
  {
    private IConfigurationProjectItem _configurationProjectItem;
    private IProject _project;

    private ProjectConfigFileShouldHaveCorrectFrameworkVersionRule _sut;

    [SetUp]
    public void SetUp ()
    {
      _project = A.Fake<IProject>();

      _configurationProjectItem = A.Fake<IConfigurationProjectItem>();
      A.CallTo(() => _project.ConfigurationProjectItem).Returns(_configurationProjectItem);

      var configuration = ConfigurationElement.Create<ProjectConfigurationFileShouldHaveCorrectFrameworkVersionRuleConfiguration>(
        initialize: c =>
        {
          c.ExpectedVersion = "Version";
          c.ExpectedSKU = "SKU";
        });

      _sut = new ProjectConfigFileShouldHaveCorrectFrameworkVersionRule(configuration);
    }

    [Test]
    public void Evaluate_ProjectWithoutFrameworkConfiguration_ReturnsViolation ()
    {
      var configurationXml = XDocument.Parse("<configuration />");

      A.CallTo(() => _configurationProjectItem.ConfigurationXml).Returns(configurationXml);

      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      result.ShouldBeEquivalentTo(
        new[]
        {
          new RuleViolation(
            _sut,
            _configurationProjectItem,
            "No explicit configuration for the supported runtime version/SKU could be found.")
        });
    }

    [Test]
    public void Evaluate_ProjectWithCorrectFrameworkConfiguration_ReturnsNoViolations ()
    {
      var configurationXml = XDocument.Parse(
        @"<configuration><startup><supportedRuntime version=""Version"" sku=""SKU"" /></startup></configuration>");

      A.CallTo(() => _configurationProjectItem.ConfigurationXml).Returns(configurationXml);

      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      result.Should().BeEmpty();
    }

    [Test]
    public void Evaluate_ProjectWithIncorrectFrameworkConfiguration_ReturnsViolations ()
    {
      var configurationXml =
          XDocument.Parse(
            @"<configuration><startup><supportedRuntime version=""DifferentVersion"" sku=""DifferentSKU"" /></startup></configuration>");

      A.CallTo(() => _configurationProjectItem.ConfigurationXml).Returns(configurationXml);

      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      result.ShouldBeEquivalentTo(
        new[]
        {
          new RuleViolation(
            _sut,
            _configurationProjectItem,
            "Unexpected value for supported runtime version, was 'DifferentVersion' but should be 'Version'."),
          new RuleViolation(
            _sut,
            _configurationProjectItem,
            "Unexpected value for supported runtime SKU, was 'DifferentSKU' but should be 'SKU'.")
        });
    }

    [Test]
    public void Evaluate_ProjectWithFrameworkConfigurationButWithoutSkuAndVersion_ReturnsViolations ()
    {
      var configurationXml = XDocument.Parse("<configuration><startup><supportedRuntime /></startup></configuration>");

      A.CallTo(() => _configurationProjectItem.ConfigurationXml).Returns(configurationXml);

      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      result.ShouldBeEquivalentTo(
        new[]
        {
          new RuleViolation(
            _sut,
            _configurationProjectItem,
            "Unexpected value for supported runtime version, was '<null>' but should be 'Version'."),
          new RuleViolation(
            _sut,
            _configurationProjectItem,
            "Unexpected value for supported runtime SKU, was '<null>' but should be 'SKU'.")
        });
    }
  }
}