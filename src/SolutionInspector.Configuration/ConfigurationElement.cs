using System;
using System.Xml.Linq;

namespace SolutionInspector.Configuration
{
  public abstract class ConfigurationElement : ConfigurationBase
  {
    public static T Create<T> (string elementName = "element", Action<T> initialize = null) where T : ConfigurationElement, new()
    {
      var configurationElement = (T) ConfigurationBase.Create(elementName, typeof(T));

      initialize?.Invoke(configurationElement);

      return configurationElement;
    }

    public new static T Load<T>(XElement element) where T : ConfigurationElement, new()
    {
      return ConfigurationBase.Load<T>(element);
    }

    public new static ConfigurationElement Load (Type configurationElementType, XElement element)
    {
      return (ConfigurationElement) ConfigurationBase.Load(configurationElementType, element);
    }
  }
}