using System;
using System.Linq;
using System.Xml.Linq;
using FluentAssertions;
using JetBrains.Annotations;
using Machine.Specifications;
using SolutionInspector.TestInfrastructure.AssertionExtensions;

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

namespace SolutionInspector.Configuration.Tests
{
  [Subject (typeof(ConfigurationBase))]
  class ConfigurationBaseSpec
  {
    static XElement Element;

    static DummyConfigurationElement SUT;

    Establish ctx = () =>
    {
      Element =
          XDocument.Parse(@"<element 
simple=""simpleValue"" 
autoConvertible=""7"" 
withConverter=""withConverterValue"" 
withConverterFromAttribute=""withConverterFromAttributeValue"" 
configuration=""configurationValue"">
  <sub string=""subValue"" />
  <collection>
    <item string=""item1"" />
    <item string=""item2"" />
  </collection>
</element>").Root;
      SUT = ConfigurationElement.Load<DummyConfigurationElement>(Element);
    };

    class when_getting_simple_value
    {
      Because of = () => Result = SUT.Simple;

      It works = () =>
            Result.Should().Be("simpleValue");

      static string Result;
    }

    class when_getting_non_configuration_property
    {
      Because of = () => Exception = Catch.Exception(() => SUT.NotAConfigurationProperty);

      It throws = () =>
        Exception.Should()
            .Be<InvalidOperationException>()
            .WithMessage("Property 'NotAConfigurationProperty' is not properly marked as a configuration property.");

      static Exception Exception;
    }

    class when_setting_simple_value
    {
      Because of = () => SUT.Simple = "newValue";

      It works = () =>
            Element.Attribute("simpleValue")?.Value.Should().Be("newValue");

      static string Result;
    }

    class when_setting_non_configuration_property
    {
      Because of = () => Exception = Catch.Exception(() => SUT.NotAConfigurationProperty = "newValue");

      It throws = () =>
        Exception.Should()
            .Be<InvalidOperationException>()
            .WithMessage("Property 'NotAConfigurationProperty' is not properly marked as a configuration property.");

      static Exception Exception;
    }

    class when_getting_auto_convertible_value
    {
      Because of = () => Result = SUT.AutoConvertible;

      It works = () =>
            Result.Should().Be(7);

      static int Result;
    }

    class when_setting_auto_convertible_value
    {
      Because of = () => SUT.AutoConvertible = 1337;

      It works = () =>
            Element.Attribute("autoConvertible")?.Value.Should().Be("1337");

      static string Result;
    }

    class when_getting_value_with_default
    {
      Because of = () => Result = SUT.WithDefault;

      It works = () =>
            Result.Should().Be("defaultValue");

      It adds_attribute_to_xml = () =>
            Element.Attribute("withDefault")?.Value.Should().Be("defaultValue");

      static string Result;
    }

    class when_setting_value_with_default
    {
      Because of = () => SUT.WithDefault = "newValue";

      It works = () =>
            Element.Attribute("withDefault")?.Value.Should().Be("newValue");

      static string Result;
    }

    class when_getting_value_with_converter
    {
      Because of = () => Result = SUT.WithConverter;

      It works = () =>
            Result.Value.Should().Be("withConverterValue");

      static DummyConvertedValue Result;
    }

    class when_getting_value_with_converter_and_default_value
    {
      Because of = () => Result = SUT.WithConverterAndDefault;

      It works = () =>
            Result.Value.Should().Be("defaultValue");

      It adds_attribute_to_xml = () =>
            Element.Attribute("withConverterAndDefault")?.Value.Should().Be("defaultValue");

      static DummyConvertedValue Result;
    }

    class when_getting_value_with_converter_from_attribute
    {
      Because of = () => Result = SUT.WithConverterFromAttribute;

      It works = () =>
            Result.Value.Should().Be("withConverterFromAttributeValue");
      
      static DummyConvertedValue Result;
    }

    class when_getting_value_with_converter_from_attribute_and_default_value
    {
      Because of = () => Result = SUT.WithConverterFromAttributeAndDefault;

      It works = () =>
            Result.Value.Should().Be("defaultValue");

      It adds_attribute_to_xml = () =>
            Element.Attribute("withConverterFromAttributeAndDefault")?.Value.Should().Be("defaultValue");

      static DummyConvertedValue Result;
    }

    class when_setting_value_with_converter
    {
      Because of = () => SUT.WithConverter = new DummyConvertedValue("newValue");

      It works = () =>
            Element.Attribute("withConverter")?.Value.Should().Be("newValue");

      static string Result;
    }

    class when_setting_value_with_converter_From_attribute
    {
      Because of = () => SUT.WithConverterFromAttribute = new DummyConvertedValueWithAttribute("newValue");

      It works = () =>
            Element.Attribute("withConverterFromAttribute")?.Value.Should().Be("newValue");

      static string Result;
    }

    class when_getting_configuration_value
    {
      Because of = () => Result = SUT.Configuration;

      It works = () =>
            Result.Value.Should().Be("configurationValue");

      static DummyConfigurationValue Result;
    }

    class when_updating_configuration_value
    {
      Because of = () => SUT.Configuration.Value = "newValue";

      It updates_value_in_xml = () =>
            Element.Attribute("configurationValue")?.Value.Should().Be("newValue");
    }

    class when_setting_configuration_value
    {
      Because of = () => SUT.Configuration = new DummyConfigurationValue(s => { }, "newValue");

      It updates_value_in_xml = () =>
            Element.Attribute("configurationValue")?.Value.Should().Be("newValue");
    }

    class when_getting_configuration_value_with_default_value
    {
      Because of = () => Result = SUT.ConfigurationWithDefault;

      It works = () =>
            Result.Value.Should().Be("defaultValue");

      It adds_attribute_to_xml = () =>
            Element.Attribute("configurationWithDefault")?.Value.Should().Be("defaultValue");

      static DummyConfigurationValue Result;
    }

    class when_getting_collection_value
    {
      Because of = () => Result = SUT.Collection;

      It works = () =>
            Result.Select(x => x.String).Should().BeEquivalentTo("item1", "item2");

      static ConfigurationElementCollection<DummySubelement> Result;
    }

    class when_getting_collection_value_that_does_not_yet_exist
    {
      Because of = () => Result = SUT.NonExistingCollection;

      It works = () =>
            Result.Count.Should().Be(0);

      It adds_element_to_xml = () =>
            Element.Element("nonExistingCollection").Should().NotBeNull();

      static ConfigurationElementCollection<DummySubelement> Result;
    }

    class when_getting_collection_value_without_attribute
    {
      Because of = () => Exception = Catch.Exception(() => SUT.CollectionWithoutAttribute);

      It throws = () =>
        Exception.Should()
            .Be<InvalidOperationException>()
            .WithMessage("Property 'CollectionWithoutAttribute' is not properly marked as a configuration collection.");

      static Exception Exception;
    }

    class when_getting_sub_value
    {
      Because of = () => Result = SUT.Sub;

      It works = () =>
            Result.String.Should().Be("subValue");

      static DummySubelement Result;
    }

    class when_getting_sub_value_that_does_not_yet_exist
    {
      Because of = () => Result = SUT.NonExistingSub;

      It works = () =>
            Result.Should().NotBeNull();

      It adds_element_to_xml = () =>
            Element.Element("nonExistingSub").Should().NotBeNull();

      static DummySubelement Result;
    }

    class when_getting_sub_value_without_attribute
    {
      Because of = () => Exception = Catch.Exception(() => SUT.SubWithoutAttribute);

      It throws = () =>
        Exception.Should()
            .Be<InvalidOperationException>()
            .WithMessage("Property 'SubWithoutAttribute' is not properly marked as a configuration Subelement.");

      static Exception Exception;
    }

    class DummyConfigurationElement : ConfigurationElement
    {
      public object NotAConfigurationProperty
      {
        get { return GetConfigurationProperty<object>(); }
        set { SetConfigurationProperty(value); }
      }

      [ConfigurationValue]
      public string Simple
      {
        get { return GetConfigurationProperty<string>(); }
        set { SetConfigurationProperty(value); }
      }

      [ConfigurationValue]
      public int AutoConvertible
      {
        get { return GetConfigurationProperty<int>(); }
        set { SetConfigurationProperty(value); }
      }

      [ConfigurationValue (IsOptional = true, DefaultValue = "defaultValue")]
      public string WithDefault
      {
        get { return GetConfigurationProperty<string>(); }
        set { SetConfigurationProperty(value); }
      }

      [ConfigurationValue (ConfigurationConverter = typeof(DummyConfigurationConverter))]
      public DummyConvertedValue WithConverter
      {
        get { return GetConfigurationProperty<DummyConvertedValue>(); }
        set { SetConfigurationProperty(value); }
      }

      [ConfigurationValue (IsOptional = true, ConfigurationConverter = typeof(DummyConfigurationConverter), DefaultValue = "defaultValue")]
      public DummyConvertedValue WithConverterAndDefault
      {
        get { return GetConfigurationProperty<DummyConvertedValue>(); }
        set { SetConfigurationProperty(value); }
      }

      [ConfigurationValue]
      public DummyConvertedValueWithAttribute WithConverterFromAttribute
      {
        get { return GetConfigurationProperty<DummyConvertedValueWithAttribute>(); }
        set { SetConfigurationProperty(value); }
      }

      [ConfigurationValue (IsOptional = true, DefaultValue = "defaultValue")]
      public DummyConvertedValueWithAttribute WithConverterFromAttributeAndDefault
      {
        get { return GetConfigurationProperty<DummyConvertedValueWithAttribute>(); }
        set { SetConfigurationProperty(value); }
      }

      [ConfigurationValue]
      public DummyConfigurationValue Configuration
      {
        get { return GetConfigurationProperty<DummyConfigurationValue>(); }
        set { SetConfigurationProperty(value); }
      }

      [ConfigurationValue (IsOptional = true, DefaultValue = "defaultValue")]
      public DummyConfigurationValue ConfigurationWithDefault => GetConfigurationProperty<DummyConfigurationValue>();

      [ConfigurationCollection]
      public ConfigurationElementCollection<DummySubelement> Collection => GetConfigurationCollection<DummySubelement>();

      [ConfigurationCollection (IsOptional = true)]
      public ConfigurationElementCollection<DummySubelement> NonExistingCollection => GetConfigurationCollection<DummySubelement>();

      public ConfigurationElementCollection<DummySubelement> CollectionWithoutAttribute => GetConfigurationCollection<DummySubelement>();

      [ConfigurationSubelement]
      public DummySubelement Sub => GetConfigurationSubelement<DummySubelement>();

      [ConfigurationSubelement (IsOptional = true)]
      public DummySubelement NonExistingSub => GetConfigurationSubelement<DummySubelement>();

      public DummySubelement SubWithoutAttribute => GetConfigurationSubelement<DummySubelement>();
    }

    class DummySubelement : ConfigurationElement
    {
      [ConfigurationValue (IsOptional = true)]
      public string String
      {
        get { return GetConfigurationProperty<string>(); }
        set { SetConfigurationProperty(value); }
      }
    }

    class DummyConvertedValue
    {
      public string Value { get; }

      public DummyConvertedValue (string value)
      {
        Value = value;
      }
    }

    [ConfigurationConverter(typeof(DummyConfigurationConverter2))]
    class DummyConvertedValueWithAttribute : DummyConvertedValue
    {
      public DummyConvertedValueWithAttribute (string value)
        : base(value)
      {
      }
    }

    class DummyConfigurationConverter : IConfigurationConverter<DummyConvertedValue>
    {
      public string ConvertTo ([CanBeNull] DummyConvertedValue value)
      {
        return value?.Value;
      }

      public DummyConvertedValue ConvertFrom ([CanBeNull] string value)
      {
        return new DummyConvertedValue(value);
      }
    }

    class DummyConfigurationConverter2 : IConfigurationConverter<DummyConvertedValueWithAttribute>
    {
      public string ConvertTo([CanBeNull] DummyConvertedValueWithAttribute value)
      {
        return value?.Value;
      }

      public DummyConvertedValueWithAttribute ConvertFrom([CanBeNull] string value)
      {
        return new DummyConvertedValueWithAttribute(value);
      }
    }

    class DummyConfigurationValue : ConfigurationValue<DummyConfigurationValue>
    {
      string _value;

      public DummyConfigurationValue (Action<string> updateValue)
        : base(updateValue)
      {
      }

      public DummyConfigurationValue (Action<string> updateValue, string value)
        : base(updateValue)
      {
        Value = value;
      }

      public string Value
      {
        get { return _value; }
        set
        {
          _value = value;
          Update();
        }
      }

      public override string Serialize ()
      {
        return Value;
      }

      public override void Deserialize (string serialized)
      {
        Value = serialized;
      }
    }
  }
}