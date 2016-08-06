using System;
using System.Configuration;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.Configuration.Rules;
using SolutionInspector.TestInfrastructure.Configuration;

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

    Establish ctx = () => { SUT = new RuleConfigurationElement(); };

    class when_deserializing_rule_with_complex_configuration_and_then_deserializing_configuration
    {
      Because of = () =>
      {
        ConfigurationHelper.DeserializeElement(SUT, @"<rule type=""Namespace.RuleName, Assembly"" direct=""Direct"">
  <sub indirect=""Indirect"" />
</rule>");
        Result = new RuleConfiguration();
        ConfigurationHelper.DeserializeElement(Result, SUT.Configuration.OuterXml);
      };

      It deserializes_configuration = () =>
      {
        Result.Direct.Should().Be("Direct");
        Result.Sub.Indirect.Should().Be("Indirect");
      };

      static RuleConfiguration Result;
    }

    class SubConfiguration : ConfigurationElement
    {
      [ConfigurationProperty ("indirect", DefaultValue = "", IsRequired = true)]
      public string Indirect => (string) this["indirect"];
    }

    class RuleConfiguration : ConfigurationElement
    {
      [ConfigurationProperty ("direct", DefaultValue = "", IsRequired = true)]
      public string Direct => (string) this["direct"];

      [ConfigurationProperty ("sub")]
      public SubConfiguration Sub => (SubConfiguration) this["sub"];
    }
  }
}