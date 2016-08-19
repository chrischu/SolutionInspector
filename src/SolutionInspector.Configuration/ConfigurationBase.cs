using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Fasterflect;
using JetBrains.Annotations;
using SolutionInspector.Configuration.Validation;

namespace SolutionInspector.Configuration
{
  public abstract class ConfigurationBase
  {
    public XElement Element { get; private set; }

    protected static T Load<T>(XElement element) where T : ConfigurationBase, new()
    {
      return (T) Load(typeof(T), element);
    }

    protected static ConfigurationBase Create (string elementName, Type configurationType)
    {
      var instance = (ConfigurationBase)Activator.CreateInstance(configurationType);
      instance.Element = new XElement(elementName);
      return instance;
    }

    protected static ConfigurationBase Load(Type configurationType, XElement element)
    {
      var instance = (ConfigurationBase)Activator.CreateInstance(configurationType);
      instance.Element = element;
      ConfigurationValidator.Validate(instance);
      return instance;
    }

    protected T GetConfigurationSubelement<T> ([CallerMemberName] string clrPropertyName = null) where T : ConfigurationElement
    {
      var configurationPropertyAttribute = GetConfigurationPropertyAttribute<ConfigurationSubelementAttribute>(clrPropertyName);

      if (configurationPropertyAttribute == null)
        throw new InvalidOperationException($"Property '{clrPropertyName}' is not properly marked as a configuration Subelement.");

      var elementName = configurationPropertyAttribute.GetXmlName(clrPropertyName);

      var element = Element.Element(elementName);
      if (element == null)
        Element.Add(element = new XElement(elementName));

      return (T) ConfigurationElement.Load(typeof(T), element);
    }

    protected T GetConfigurationProperty<T> ([CallerMemberName] string clrPropertyName = null)
    {
      var configurationPropertyType = typeof(T);
      var configurationPropertyAttribute = GetConfigurationPropertyAttribute<ConfigurationValueAttribute>(clrPropertyName);

      if (configurationPropertyAttribute == null)
        throw new InvalidOperationException($"Property '{clrPropertyName}' is not properly marked as a configuration property.");

      var attributeName = configurationPropertyAttribute.GetXmlName(clrPropertyName);
      var attribute = Element.Attribute(attributeName);

      var configurationConverterAttribute = configurationPropertyType.GetCustomAttribute<ConfigurationConverterAttribute>();
      if (configurationPropertyAttribute.ConfigurationConverter != null || configurationConverterAttribute != null)
      {
        if (attribute == null && configurationPropertyAttribute.DefaultValue != null)
          Element.Add(attribute = new XAttribute(attributeName, configurationPropertyAttribute.DefaultValue));

        var converterType = configurationPropertyAttribute.ConfigurationConverter ?? configurationConverterAttribute.ConfigurationConverterType;
        var converter = CreateConfigurationConverter<T>(converterType);

        return converter.ConvertFrom(attribute?.Value);
      }

      if (typeof(IConfigurationValue).IsAssignableFrom(configurationPropertyType))
      {
        if (attribute?.Value == null)
          Element.Add(attribute = new XAttribute(attributeName, configurationPropertyAttribute.DefaultValue ?? ""));

        var instance = (T) Activator.CreateInstance(
          configurationPropertyType,
          (Action<string>) (s => Element.SetAttributeValue(attributeName, s)));
        instance.CallMethod("Deserialize", attribute.Value);
        return instance;
      }

      if (attribute == null && configurationPropertyAttribute.DefaultValue != null)
        Element.Add(attribute = new XAttribute(attributeName, configurationPropertyAttribute.DefaultValue));

      return (T) Convert.ChangeType(attribute?.Value, configurationPropertyType);
    }

    private IConfigurationConverter<T> CreateConfigurationConverter<T> (Type configurationConverterType)
    {
      return (IConfigurationConverter<T>) Activator.CreateInstance(configurationConverterType);
    }

    protected void SetConfigurationProperty<T> (T value, [CallerMemberName] string clrPropertyName = null)
    {
      var configurationValue = value as IConfigurationValue;
      var configurationPropertyAttribute = GetConfigurationPropertyAttribute<ConfigurationValueAttribute>(clrPropertyName);
      var configurationConverterAttribute = typeof(T).GetCustomAttribute<ConfigurationConverterAttribute>();

      if (configurationPropertyAttribute == null)
        throw new InvalidOperationException($"Property '{clrPropertyName}' is not properly marked as a configuration property.");

      var attributeName = configurationPropertyAttribute.GetXmlName(clrPropertyName);

      if (configurationValue != null)
        Element.SetAttributeValue(attributeName, configurationValue.Serialize());
      else if (configurationPropertyAttribute.ConfigurationConverter != null || configurationConverterAttribute != null)
      {
        var converterType = configurationPropertyAttribute.ConfigurationConverter ?? configurationConverterAttribute.ConfigurationConverterType;
        var converter = CreateConfigurationConverter<T>(converterType);

        Element.SetAttributeValue(attributeName, converter.ConvertTo(value));
      }
      else
        Element.SetAttributeValue(attributeName, value);
    }

    protected ConfigurationElementCollection<T> GetConfigurationCollection<T> ([CallerMemberName] string clrPropertyName = null)
      where T : ConfigurationElement, new()
    {
      var configurationCollectionAttribute = GetConfigurationPropertyAttribute<ConfigurationCollectionAttribute>(clrPropertyName);

      if (configurationCollectionAttribute == null)
        throw new InvalidOperationException($"Property '{clrPropertyName}' is not properly marked as a configuration collection.");

      XElement collectionElement;
      if (configurationCollectionAttribute.IsDefaultCollection)
        collectionElement = Element;
      else
      {
        collectionElement = Element.Element(configurationCollectionAttribute.GetXmlName(clrPropertyName));

        if (collectionElement == null)
          Element.Add(collectionElement = new XElement(configurationCollectionAttribute.GetXmlName(clrPropertyName)));
      }

      return new ConfigurationElementCollection<T>(collectionElement, configurationCollectionAttribute.ElementName);
    }

    [CanBeNull]
    private T GetConfigurationPropertyAttribute<T> (string clrPropertyName) where T : Attribute
    {
      var type = GetType();
      var property = type.GetProperty(clrPropertyName);
      return property.GetCustomAttribute<T>();
    }
  }
}