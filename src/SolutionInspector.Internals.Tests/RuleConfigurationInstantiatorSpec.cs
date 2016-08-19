using System.Xml;
using System.Xml.Linq;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.Configuration;
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

namespace SolutionInspector.Internals.Tests
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
      Because of = () => Result = SUT.Instantiate(typeof(RuleConfiguration), XElement.Parse(@"<rule property=""value"" />"));

      It returns_correct_type = () =>
          Result.Should().BeOfType(typeof(RuleConfiguration));

      It sets_property = () =>
          Result.As<RuleConfiguration>().Property.Should().Be("value");

      static ConfigurationElement Result;
    }

    class when_instantiating_with_null
    {
      Because of = () => Result = SUT.Instantiate(null, Some.XElement);

      It returns_null = () =>
          Result.Should().BeNull();

      static XmlElement Configuration;
      static ConfigurationElement Result;
    }

    class RuleConfiguration : ConfigurationElement
    {
      [ConfigurationValue]
      public string Property => GetConfigurationProperty<string>();
    }
  }
}