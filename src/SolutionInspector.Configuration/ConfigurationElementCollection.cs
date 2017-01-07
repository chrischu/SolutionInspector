using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Linq;

namespace SolutionInspector.Configuration
{
  /// <summary>
  ///   Collection for use in configuration classes.
  /// </summary>
  public class ConfigurationElementCollection<T> : IReadOnlyCollection<T>
    where T : ConfigurationElement, new()
  {
    private readonly List<T> _collection = new List<T>();
    private readonly XElement _collectionElement;
    private readonly string _elementName;

    /// <summary>
    ///   Creates a new <see cref="ConfigurationElementCollection{T}" />.
    /// </summary>
    public ConfigurationElementCollection (XElement collectionElement, string elementName)
    {
      _collectionElement = collectionElement;
      _collection.AddRange(_collectionElement.Elements().Select(ConfigurationElement.Load<T>));
      _elementName = elementName;
    }

    /// <summary>
    ///   Provides read access to the collection's items by <paramref name="index" />.
    /// </summary>
    public T this [int index] => _collection[index];

    public int Count => _collection.Count;

    public IEnumerator<T> GetEnumerator ()
    {
      return _collection.GetEnumerator();
    }

    [ExcludeFromCodeCoverage]
    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }

    /// <summary>
    ///   Adds a new elements to the collection and returns it.
    /// </summary>
    public T AddNew ()
    {
      var configurationElement = ConfigurationElement.Create<T>(_elementName);
      Add(configurationElement);
      return configurationElement;
    }

    /// <summary>
    ///   Adds the given <paramref name="element" /> to the collection.
    /// </summary>
    public void Add (T element)
    {
      _collection.Add(element);
      _collectionElement.Add(element.Element);
    }

    /// <summary>
    ///   Removes the given <paramref name="element" /> from the collection.
    /// </summary>
    public void Remove (T element)
    {
      _collection.Remove(element);
      element.Element.Remove();
    }

    /// <summary>
    ///   Completely clears the collection off all its elements.
    /// </summary>
    public void Clear ()
    {
      _collection.Clear();
      _collectionElement.RemoveNodes();
    }
  }
}