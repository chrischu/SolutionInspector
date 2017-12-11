using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using Fasterflect;
using SolutionInspector.Configuration;

namespace SolutionInspector.Api.Configuration
{
  /// <summary>
  ///   Base class for <see cref="ConfigurationElementCollection{T}" />s with elements of type <typeparamref name="TElement" />.
  /// </summary>
  public abstract class ConfigurationElementCollectionBase<TElement> : ConfigurationElementCollection, IConfigurationCollection<TElement>
    where TElement : System.Configuration.ConfigurationElement, new()

  {
    /// <inheritdoc />
    protected abstract override string ElementName { get; }

    /// <inheritdoc />
    public override ConfigurationElementCollectionType CollectionType => ConfigurationElementCollectionType.BasicMap;

    /// <summary>
    ///   Adds the given <paramref name="element" /> to the collection.
    /// </summary>
    public void Add (TElement element)
    {
      BaseAdd(element);
    }

    /// <inheritdoc />
    public void Clear ()
    {
      BaseClear();
    }

    /// <inheritdoc />
    public bool Contains (TElement item)
    {
      return BaseIndexOf(item) != -1;
    }

    /// <inheritdoc />
    public void Remove (TElement item)
    {
      BaseRemove(GetElementKey(item));
    }

    /// <inheritdoc />
    public new IEnumerator<TElement> GetEnumerator ()
    {
      var enumerator = base.GetEnumerator();

      while (enumerator.MoveNext())
        yield return (TElement) enumerator.Current;
    }

    /// <inheritdoc />
    protected sealed override System.Configuration.ConfigurationElement CreateNewElement ()
    {
      return new TElement();
    }

    /// <inheritdoc />
    protected override bool IsElementName (string elementName)
    {
      return !string.IsNullOrWhiteSpace(elementName) && elementName == ElementName;
    }

    /// <inheritdoc />
    protected override object GetElementKey (System.Configuration.ConfigurationElement element)
    {
      return element;
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