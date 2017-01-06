using System.Xml.Linq;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Configuration;
using SolutionInspector.TestInfrastructure;

namespace SolutionInspector.Internals.Tests
{
  public class RuleConfigurationInstantiatorTests
  {
    private IRuleConfigurationInstantiator _sut;

    [SetUp]
    public void SetUp ()
    {
      _sut = new RuleConfigurationInstantiator();
    }

    [Test]
    public void Instantiate_DefaultCase_InstantiateRuleConfigurationAndSetsProperties ()
    {
      // ACT
      var result = _sut.Instantiate(typeof(RuleConfiguration), XElement.Parse(@"<rule property=""value"" />"));

      // ASSERT
      result.Should().BeOfType<RuleConfiguration>();
      result.As<RuleConfiguration>().Property.Should().Be("value");
    }

    [Test]
    public void Instantiate_WithNull_ReturnsNull ()
    {
      // ACT
      var result = _sut.Instantiate(null, Some.XElement);

      // ASSERT
      result.Should().BeNull();
    }

    class RuleConfiguration : ConfigurationElement
    {
      [ConfigurationValue]
      public string Property => GetConfigurationProperty<string>();
    }
  }
}