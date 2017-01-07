using System.Xml.Linq;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Api.Configuration.Ruleset;
using SolutionInspector.Commons.Extensions;
using SolutionInspector.Configuration;

namespace SolutionInspector.Api.Configuration.Tests.Ruleset
{
  public class RuleConfigurationElementTests
  {
    [Test]
    public void Loading_BothAsCommonRuleConfigurationElementAndSpecialRuleConfigurationElement_Works ()
    {
      var element = XElement.Parse(@"<rule type=""Namespace.RuleName, Assembly"" direct=""Direct"">
  <sub indirect=""Indirect"" />
</rule>");

      // ACT
      var commonRuleConfiguration = ConfigurationElement.Load<RuleConfigurationElement>(element);

      // ASSERT
      commonRuleConfiguration.RuleType.Should().Be("Namespace.RuleName, Assembly");

      // ACT
      var specialRuleConfiguration = ConfigurationElement.Load<SpecialRuleConfigurationElement>(element);

      // ASSERT
      specialRuleConfiguration.Direct.Should().Be("Direct");
      specialRuleConfiguration.Sub.Indirect.Should().Be("Indirect");
    }

    [Test]
    public void RuleTypeSet ()
    {
      var element = XElement.Parse(@"<rule type=""Original"" />");
      var ruleConfiguration = ConfigurationElement.Load<RuleConfigurationElement>(element);

      // ACT
      ruleConfiguration.RuleType = "Changed";

      // ASSERT
      ruleConfiguration.RuleType.Should().Be("Changed");
      element.Attribute("type").AssertNotNull().Value.Should().Be("Changed");
    }

    // ReSharper disable once ClassNeverInstantiated.Local
    private class SubConfiguration : ConfigurationElement
    {
      [ConfigurationValue]
      public string Indirect => GetConfigurationValue<string>();
    }

    private class SpecialRuleConfigurationElement : ConfigurationElement
    {
      [ConfigurationValue]
      public string Direct => GetConfigurationValue<string>();

      [ConfigurationSubelement]
      public SubConfiguration Sub => GetConfigurationSubelement<SubConfiguration>();
    }
  }
}