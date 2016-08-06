using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using Fasterflect;

namespace SolutionInspector.Api.Configuration
{
  /// <summary>
  ///   Base class for <see cref="ConfigurationElementCollection" />s with elements of type <typeparamref name="TElement" />.
  /// </summary>
  public abstract class ConfigurationElementCollectionBase<TElement> : ConfigurationElementCollection, IEnumerable<TElement>
      where TElement : ConfigurationElement, new()

  {
    /// <summary>
    ///   Adds the given <paramref name="element" /> to the collection.
    /// </summary>
    public void Add (TElement element)
    {
      BaseAdd(element);
    }

    /// <inheritdoc />
    protected sealed override ConfigurationElement CreateNewElement ()
    {
      return new TElement();
    }

    protected abstract override string ElementName { get; }

    public override ConfigurationElementCollectionType CollectionType => ConfigurationElementCollectionType.BasicMap;

    protected override bool IsElementName (string elementName)
    {
      return !string.IsNullOrWhiteSpace(elementName) && elementName == ElementName;
    }

    /// <inheritdoc />
    protected override object GetElementKey (ConfigurationElement element)
    {
      return element;
    }

    /// <inheritdoc />
    public new IEnumerator<TElement> GetEnumerator ()
    {
      var enumerator = base.GetEnumerator();

      while (enumerator.MoveNext())
        yield return (TElement) enumerator.Current;
    }

    /// <inheritdoc />
    protected override bool OnDeserializeUnrecognizedElement (string elementName, XmlReader reader)
    {
      if (!IsElementName(elementName))
        return false;

      var element = CreateNewElement();
      element.CallMethod("DeserializeElement", reader, false);
      BaseAdd(element);
      return true;
    }
  }
}