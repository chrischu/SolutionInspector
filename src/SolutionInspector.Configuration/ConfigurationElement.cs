using System;
using System.Xml.Linq;

namespace SolutionInspector.Configuration
{
  /// <summary>
  ///   Base class for configuration elements (configuration classes that map to a <see cref="XElement" />).
  /// </summary>
  public abstract class ConfigurationElement : ConfigurationBase
  {
    /// <summary>
    ///   Creates a new <see cref="ConfigurationElement" /> with an underlying <see cref="XElement" /> that uses <paramref name="elementName" />
    ///   as its name.
    /// </summary>
    public static T Create<T> (string elementName = "element", Action<T> initialize = null) where T : ConfigurationElement, new()
    {
      var configurationElement = (T) Create(elementName, typeof(T));

      initialize?.Invoke(configurationElement);

      return configurationElement;
    }

    /// <summary>
    ///   Loads the <see cref="ConfigurationElement" /> of type <typeparamref name="T" /> with the given <see cref="XElement" /> as input.
    /// </summary>
    public new static T Load<T> (XElement element) where T : ConfigurationElement, new()
    {
      return ConfigurationBase.Load<T>(element);
    }

    /// <summary>
    ///   Loads the <see cref="ConfigurationElement" /> of type <paramref name="configurationElementType" /> with the given <see cref="XElement" />
    ///   as input.
    /// </summary>
    public new static ConfigurationElement Load (Type configurationElementType, XElement element)
    {
      return (ConfigurationElement) ConfigurationBase.Load(configurationElementType, element);
    }
  }
}