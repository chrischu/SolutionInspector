using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SolutionInspector.Configuration.Collections
{
  internal class ConfigurationElementCollection<TElement>
      : ConfigurationElementCollectionBase<TElement>, IConfigurationElementCollection<TElement>
      where TElement : ConfigurationElement, new()
  {
    private readonly string _elementName;

    public ConfigurationElementCollection (XElement collectionElement, string elementName, IEnumerable<TElement> items = null)
        : base(collectionElement, items ?? Enumerable.Empty<TElement>())
    {
      _elementName = elementName;
    }

    protected override void ValidateNewElement (TElement element)
    {
      if (element.Element.Name.LocalName != _elementName)
        throw new ArgumentException(
            $"The given element is not compatible with this collection since it has an invalid name '{element.Element.Name.LocalName}' " +
            $"and only elements named '{_elementName}' are allowed.",
            nameof(element));
    }

    public TElement AddNew ()
    {
      var configurationElement = ConfigurationElement.Create<TElement>(_elementName);
      Add(configurationElement);
      return configurationElement;
    }
  }
}