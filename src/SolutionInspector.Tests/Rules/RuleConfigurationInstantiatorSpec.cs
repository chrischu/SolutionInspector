using System;
using System.Configuration;
using System.Xml;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.Rules;
using SolutionInspector.TestInfrastructure;
using Wrapperator.Interfaces.Reflection;

#region R# preamble for Machine.Specifications files

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

namespace SolutionInspector.Tests.Rules
{
  [Subject (typeof(RuleConfigurationInstantiator))]
  class RuleConfigurationInstantiatorSpec
  {
    static string AssemblyName;
    static IAssembly Assembly;

    static IRuleConfigurationInstantiator SUT;

    Establish ctx = () =>
    {
      AssemblyName = Some.String();

      SUT = new RuleConfigurationInstantiator();
    };

    class when_instantiating
    {
      Establish ctx = () =>
      {
        var doc = new XmlDocument();
        doc.LoadXml(@"<rule property=""value"" />");
        Configuration = (XmlElement) doc.FirstChild;
      };

      Because of = () => Result = SUT.Instantiate(typeof(RuleConfiguration), Configuration);

      It returns_correct_type = () =>
          Result.Should().BeOfType(typeof(RuleConfiguration));

      It sets_property = () =>
          Result.As<RuleConfiguration>().Property.Should().Be("value");

      static XmlElement Configuration;
      static ConfigurationElement Result;
    }

    class when_instantiating_with_null
    {
      Establish ctx = () =>
      {
        var doc = new XmlDocument();
        doc.LoadXml(@"<rule property=""value"" />");
        Configuration = (XmlElement) doc.FirstChild;
      };

      Because of = () => Result = SUT.Instantiate(null, Configuration);

      It returns_null = () =>
          Result.Should().BeNull();

      static XmlElement Configuration;
      static ConfigurationElement Result;
    }

    class RuleConfiguration : ConfigurationElement
    {
      [ConfigurationProperty ("property", DefaultValue = "", IsRequired = true)]
      public string Property => (string) this["property"];
    }
  }
}