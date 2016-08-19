using System.Xml.Linq;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.Api.Configuration.Ruleset;
using SolutionInspector.Commons.Extensions;

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

namespace SolutionInspector.Configuration.Tests.Rules
{
  [Subject (typeof(RuleConfigurationElement))]
  class RuleConfigurationElementSpec
  {
    static RuleConfigurationElement SUT;

    class when_deserializing_rule_with_complex_configuration_and_then_deserializing_configuration
    {
      Because of = () =>
      {
        var xElement = XElement.Parse(@"<rule type=""Namespace.RuleName, Assembly"" direct=""Direct"">
  <sub indirect=""Indirect"" />
</rule>");

        SUT = ConfigurationElement.Load<RuleConfigurationElement>(xElement);
        RuleConfiguration = ConfigurationElement.Load<RuleConfiguration>(xElement);
      };

      It deserializes_configuration = () =>
      {
        SUT.RuleType.Should().Be("Namespace.RuleName, Assembly");

        RuleConfiguration.Direct.Should().Be("Direct");
        RuleConfiguration.Sub.Indirect.Should().Be("Indirect");
      };

      static RuleConfiguration RuleConfiguration;
    }

    class when_changing_the_rule_type
    {
      Establish ctx = () =>
      {
       XElement = XElement.Parse(@"<rule type=""Namespace.RuleName, Assembly"" />");
        SUT = ConfigurationElement.Load<RuleConfigurationElement>(XElement);
      };

      Because of = () => SUT.RuleType = "Changed";

      It changes_rule_type = () =>
            XElement.Attribute("type").AssertNotNull().Value.Should().Be("Changed");

      static XElement XElement;
    }

    class SubConfiguration : ConfigurationElement
    {
      [ConfigurationValue]
      public string Indirect => GetConfigurationProperty<string>();
    }

    class RuleConfiguration : ConfigurationElement
    {
      [ConfigurationValue]
      public string Direct => GetConfigurationProperty<string>();

      [ConfigurationSubelement]
      public SubConfiguration Sub => GetConfigurationSubelement<SubConfiguration>();
    }
  }
}