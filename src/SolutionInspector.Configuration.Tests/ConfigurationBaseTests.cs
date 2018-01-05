using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using FakeItEasy;
using FluentAssertions;
using JetBrains.Annotations;
using NUnit.Framework;
using SolutionInspector.Commons.Extensions;
using SolutionInspector.Configuration.Attributes;
using SolutionInspector.Configuration.Collections;
using SolutionInspector.Configuration.Dynamic;
using SolutionInspector.TestInfrastructure;

namespace SolutionInspector.Configuration.Tests
{
  public class ConfigurationBaseTests
  {
    private XDocument _document;
    private XElement _element;
    private DummyConfigurationElement _sut;

    [SetUp]
    public void SetUp ()
    {
      var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;

      var dynamicConfigurationElementTypeResolver = A.Fake<IDynamicConfigurationElementTypeHelper>();
      A.CallTo(() => dynamicConfigurationElementTypeResolver.ResolveElementType(A<XElement>._)).Returns(typeof(DummyDynamicConfigurationElement));
      ConfigurationBase.DynamicConfigurationElementTypeHelper = dynamicConfigurationElementTypeResolver;

      _document = XDocument.Parse(
          $@"
<root xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
      xsi:schemaLocation=""{assemblyName} {assemblyName}.xsd""
      xmlns:x=""{assemblyName}"">
  <element 
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
    <dynamicCollection>
      <x:{typeof(DummyDynamicConfigurationElement).FullName} value=""dynamicValue"" />
    </dynamicCollection>
  </element>
</root>");

      _element = _document.Root.AssertNotNull().Elements().Single().AssertNotNull();
      _sut = ConfigurationElement.Load<DummyConfigurationElement>(_element);
    }

    [Test]
    public void Get_NonConfigurationProperty_Throws ()
    {
      // ACT
      Action act = () => Dev.Null = _sut.NotAConfigurationProperty;

      // ASSERT
      act.ShouldThrow<InvalidOperationException>()
          .WithMessage("Property 'NotAConfigurationProperty' is not properly marked as a configuration property.");
    }

    [Test]
    public void Get_SimpleValue ()
    {
      // ACT
      var result = _sut.Simple;

      // ASSERT
      result.Should().Be("simpleValue");
    }

    [Test]
    public void Get_AutoConvertibleValue ()
    {
      // ACT
      var result = _sut.AutoConvertible;

      // ASSERT
      result.Should().Be(7);
    }

    [Test]
    public void Get_ValueWithConverter ()
    {
      // ACT
      var result = _sut.WithConverter;

      // ASSERT
      result.Value.Should().Be("withConverterValue");
    }

    [Test]
    public void Get_ValueWithConverterFromTypeAttribute ()
    {
      // ACT
      var result = _sut.WithConverterFromAttribute;

      // ASSERT
      result.Value.Should().Be("withConverterFromAttributeValue");
    }

    [Test]
    public void Get_IConfigurationValue ()
    {
      // ACT
      var result = _sut.Configuration;

      // ASSERT
      result.Value.Should().Be("configurationValue");
    }

    [Test]
    public void Get_CollectionValue ()
    {
      // ACT
      var result = _sut.Collection;

      // ASSERT
      result.ShouldBeEquivalentTo(
          new[]
          {
              new { String = "item1" },
              new { String = "item2" }
          },
          o => o.ExcludingMissingMembers().WithStrictOrdering());
    }

    [Test]
    public void Get_DynamicCollectionValue ()
    {
      // ACT
      var result = _sut.DynamicCollection;

      // ASSERT
      result.Should().HaveCount(1);
      result[0].Should().BeOfType<DummyDynamicConfigurationElement>().Which.Value.Should().Be("dynamicValue");
    }

    [Test]
    public void Get_SubValue ()
    {
      // ACT
      var result = _sut.Sub;

      // ASSERT
      result.String.Should().Be("subValue");
    }

    [Test]
    public void Get_NonExistingValueWithDefault_ReturnsDefaultValueAndUpdatesXml ()
    {
      // ACT
      var result = _sut.WithDefault;

      // ASSERT
      result.Should().Be("defaultValue");

      _element.Attribute("withDefault")?.Value.Should().Be("defaultValue");
    }

    [Test]
    public void Get_NonExistingValueWithConverterAndDefault_ReturnsDefaultValueAndUpdatesXml ()
    {
      // ACT
      var result = _sut.WithConverterAndDefault;

      // ASSERT
      result.Value.Should().Be("defaultValue");

      _element.Attribute("withConverterAndDefault")?.Value.Should().Be("defaultValue");
    }

    [Test]
    public void Get_NonExistingValueWithConverterFromAttributeAndDefault_ReturnsDefaultValueAndUpdatesXml ()
    {
      // ACT
      var result = _sut.WithConverterFromAttributeAndDefault;

      // ASSERT
      result.Value.Should().Be("defaultValue");

      _element.Attribute("withConverterFromAttributeAndDefault")?.Value.Should().Be("defaultValue");
    }

    [Test]
    public void Get_NonExistingIConfigurationValueWithDefault_ReturnsDefaultValueAndUpdatesXml ()
    {
      // ACT
      var result = _sut.ConfigurationWithDefault;

      // ASSERT
      result.Should().NotBeNull();
      result?.Value.Should().Be("defaultValue");

      _element.Attribute("configurationWithDefault")?.Value.Should().Be("defaultValue");
    }

    [Test]
    public void Get_NonExistingCollectionValue_ReturnsEmptyCollectionAndUpdatesXml ()
    {
      // ACT
      var result = _sut.NonExistingCollection;

      // ASSERT
      result.Count.Should().Be(0);

      _element.Element("nonExistingCollection").Should().NotBeNull();
    }

    [Test]
    public void Get_NonExistingDynamicCollectionValue_ReturnsEmptyCollectionAndUpdatesXml ()
    {
      // ACT
      var result = _sut.NonExistingDynamicCollection;

      // ASSERT
      result.Count.Should().Be(0);

      _element.Element("nonExistingDynamicCollection").Should().NotBeNull();
    }

    [Test]
    public void Get_NonExistingSubValue_ReturnsEmptySubelementAndUpdatesXml ()
    {
      // ACT
      var result = _sut.NonExistingSub;

      // ASSERT
      result.Should().NotBeNull();

      _element.Element("nonExistingSub").Should().NotBeNull();
    }

    [Test]
    public void Get_CollectionValueWithoutAttribute_Throws ()
    {
      // ACT
      Action act = () => Dev.Null = _sut.CollectionWithoutAttribute;

      // ASSERT
      act.ShouldThrow<InvalidOperationException>()
          .WithMessage("Property 'CollectionWithoutAttribute' is not properly marked as a configuration collection.");
    }

    [Test]
    public void Get_DynamicCollectionValueWithoutAttribute_Throws ()
    {
      // ACT
      Action act = () => Dev.Null = _sut.DynamicCollectionWithoutAttribute;

      // ASSERT
      act.ShouldThrow<InvalidOperationException>()
          .WithMessage("Property 'DynamicCollectionWithoutAttribute' is not properly marked as a dynamic configuration collection.");
    }

    [Test]
    public void Get_SubValueWithoutAttribute_Throws ()
    {
      // ACT
      Action act = () => Dev.Null = _sut.SubWithoutAttribute;

      // ASSERT
      act.ShouldThrow<InvalidOperationException>()
          .WithMessage("Property 'SubWithoutAttribute' is not properly marked as a configuration subelement.");
    }

    [Test]
    public void Set_SimpleValue ()
    {
      // ACT
      _sut.Simple = "newValue";

      // ASSERT
      _element.Attribute("simpleValue")?.Value.Should().Be("newValue");
    }

    [Test]
    public void Set_NonConfigurationValue_Throws ()
    {
      // ACT
      Action act = () => _sut.NotAConfigurationProperty = "newValue";

      // ASSERT
      act.ShouldThrow<InvalidOperationException>()
          .WithMessage("Property 'NotAConfigurationProperty' is not properly marked as a configuration property.");
    }

    [Test]
    public void Set_AutoConvertibleValue ()
    {
      // ACT
      _sut.AutoConvertible = 1337;

      // ASSERT
      _element.Attribute("autoConvertible")?.Value.Should().Be("1337");
    }

    [Test]
    public void Set_ValueWithConverter ()
    {
      // ACT
      _sut.WithConverter = new DummyConvertedValue("newValue");

      // ASSERT
      _element.Attribute("withConverter")?.Value.Should().Be("newValue");
    }

    [Test]
    public void Set_ValueWithConverterFromAttribute ()
    {
      // ACT
      _sut.WithConverterFromAttribute = new DummyConvertedValueWithAttribute("newValue");

      // ASSERT
      _element.Attribute("withConverterFromAttribute")?.Value.Should().Be("newValue");
    }

    [Test]
    public void Set_IConfigurationValue ()
    {
      // ACT
      _sut.Configuration = new DummyConfigurationValue(s => { }, "newValue");

      // ASSERT
      _element.Attribute("configurationValue")?.Value.Should().Be("newValue");
    }

    [Test]
    public void Set_InnerValueFromIConfigurationValue ()
    {
      // ACT
      _sut.Configuration.Value = "newValue";

      // ASSERT
      _element.Attribute("configurationValue")?.Value.Should().Be("newValue");
    }

    private class DummyConfigurationElement : ConfigurationElement
    {
      public object NotAConfigurationProperty
      {
        get => GetConfigurationValue<object>();
        set => SetConfigurationValue(value);
      }

      [ConfigurationValue]
      public string Simple
      {
        get => GetConfigurationValue<string>();
        set => SetConfigurationValue(value);
      }

      [ConfigurationValue]
      public int AutoConvertible
      {
        get => GetConfigurationValue<int>();
        set => SetConfigurationValue(value);
      }

      [ConfigurationValue(IsOptional = true, DefaultValue = "defaultValue")]
      public string WithDefault
      {
        get => GetConfigurationValue<string>();
        // ReSharper disable once UnusedMember.Local
        set => SetConfigurationValue(value);
      }

      [ConfigurationValue(ConfigurationConverter = typeof(DummyConfigurationConverter))]
      public DummyConvertedValue WithConverter
      {
        get => GetConfigurationValue<DummyConvertedValue>();
        set => SetConfigurationValue(value);
      }

      [ConfigurationValue(IsOptional = true, ConfigurationConverter = typeof(DummyConfigurationConverter), DefaultValue = "defaultValue")]
      public DummyConvertedValue WithConverterAndDefault
      {
        get => GetConfigurationValue<DummyConvertedValue>();
        // ReSharper disable once UnusedMember.Local
        set => SetConfigurationValue(value);
      }

      [ConfigurationValue]
      public DummyConvertedValueWithAttribute WithConverterFromAttribute
      {
        get => GetConfigurationValue<DummyConvertedValueWithAttribute>();
        set => SetConfigurationValue(value);
      }

      [ConfigurationValue(IsOptional = true, DefaultValue = "defaultValue")]
      public DummyConvertedValueWithAttribute WithConverterFromAttributeAndDefault
      {
        get => GetConfigurationValue<DummyConvertedValueWithAttribute>();
        // ReSharper disable once UnusedMember.Local
        set => SetConfigurationValue(value);
      }

      [ConfigurationValue]
      public DummyConfigurationValue Configuration
      {
        get => GetConfigurationValue<DummyConfigurationValue>();
        set => SetConfigurationValue(value);
      }

      [CanBeNull]
      [ConfigurationValue(IsOptional = true, DefaultValue = "defaultValue")]
      public DummyConfigurationValue ConfigurationWithDefault => GetConfigurationValue<DummyConfigurationValue>();

      [ConfigurationCollection]
      public IConfigurationElementCollection<DummySubelement> Collection => GetConfigurationCollection<DummySubelement>();

      [ConfigurationCollection(IsOptional = true)]
      public IConfigurationElementCollection<DummySubelement> NonExistingCollection => GetConfigurationCollection<DummySubelement>();

      public IConfigurationElementCollection<DummySubelement> CollectionWithoutAttribute => GetConfigurationCollection<DummySubelement>();

      [DynamicConfigurationCollection]
      public IDynamicConfigurationElementCollection<DummyDynamicConfigurationElementBase> DynamicCollection =>
          GetDynamicConfigurationCollection<DummyDynamicConfigurationElementBase>();

      [DynamicConfigurationCollection(IsOptional = true)]
      public IDynamicConfigurationElementCollection<DummyDynamicConfigurationElementBase> NonExistingDynamicCollection =>
          GetDynamicConfigurationCollection<DummyDynamicConfigurationElementBase>();

      public IDynamicConfigurationElementCollection<DummyDynamicConfigurationElementBase> DynamicCollectionWithoutAttribute =>
          GetDynamicConfigurationCollection<DummyDynamicConfigurationElementBase>();

      [ConfigurationSubelement]
      public DummySubelement Sub => GetConfigurationSubelement<DummySubelement>();

      [ConfigurationSubelement(IsOptional = true)]
      public DummySubelement NonExistingSub => GetConfigurationSubelement<DummySubelement>();

      public DummySubelement SubWithoutAttribute => GetConfigurationSubelement<DummySubelement>();
    }

    private class DummySubelement : ConfigurationElement
    {
      [ConfigurationValue(IsOptional = true)]
      public string String
      {
        get => GetConfigurationValue<string>();
        // ReSharper disable once UnusedMember.Local
        set => SetConfigurationValue(value);
      }
    }

    private class DummyConvertedValue
    {
      public DummyConvertedValue ([CanBeNull] string value)
      {
        Value = value;
      }

      [CanBeNull]
      public string Value { get; }
    }

    [ConfigurationConverter(typeof(DummyConfigurationConverter2))]
    private class DummyConvertedValueWithAttribute : DummyConvertedValue
    {
      public DummyConvertedValueWithAttribute ([CanBeNull] string value)
          : base(value)
      {
      }
    }

    private class DummyConfigurationConverter : IConfigurationConverter<DummyConvertedValue>
    {
      [CanBeNull]
      public string ConvertTo ([CanBeNull] DummyConvertedValue value)
      {
        return value?.Value;
      }

      [CanBeNull]
      public DummyConvertedValue ConvertFrom ([CanBeNull] string value)
      {
        return new DummyConvertedValue(value);
      }
    }

    private class DummyConfigurationConverter2 : IConfigurationConverter<DummyConvertedValueWithAttribute>
    {
      [CanBeNull]
      public string ConvertTo ([CanBeNull] DummyConvertedValueWithAttribute value)
      {
        return value?.Value;
      }

      [CanBeNull]
      public DummyConvertedValueWithAttribute ConvertFrom ([CanBeNull] string value)
      {
        return new DummyConvertedValueWithAttribute(value);
      }
    }

    private class DummyConfigurationValue : ConfigurationValue<DummyConfigurationValue>
    {
      private string _value;

      // ReSharper disable once MemberCanBePrivate.Local
      public DummyConfigurationValue (Action<string> updateValue)
          : base(updateValue)
      {
      }

      public DummyConfigurationValue (Action<string> updateValue, string value)
          : this(updateValue)
      {
        Value = value;
      }

      public string Value
      {
        get => _value;
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

  internal abstract class DummyDynamicConfigurationElementBase : ConfigurationElement
  {
  }

  internal class DummyDynamicConfigurationElement : DummyDynamicConfigurationElementBase
  {
    [ConfigurationValue]
    public string Value => GetConfigurationValue<string>();
  }
}