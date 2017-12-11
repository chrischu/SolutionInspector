using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace SolutionInspector.Configuration
{
  /// <summary>
  ///   Collection for use in configuration classes.
  /// </summary>
  public class ConfigurationElementCollection<T> : IReadOnlyCollection<T>, IList<T>
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
      _collection.AddRange(_collectionElement.Elements().Select(element => ConfigurationBase.Load<T>(element)));
      _elementName = elementName;
    }

    /// <summary>
    ///   Provides access to the collection's items by <paramref name="index" />.
    /// </summary>
    public T this [int index]
    {
      get => _collection[index];
      set => _collection[index] = value;
    }

    /// <summary>
    ///   Gets the number of elements contained in the <see cref="ConfigurationElementCollection{T}" />.
    /// </summary>
    public int Count => _collection.Count;

    /// <inheritdoc />
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
    public void Add ([CanBeNull] T element)
    {
      if (element == null)
        throw new ArgumentNullException(nameof(element));

      _collection.Add(element);
      _collectionElement.Add(element.Element);
    }

    /// <summary>
    ///   Adds the given <paramref name="element" /> to the collection at the given <paramref name="index" />.
    /// </summary>
    public void Insert (int index, [CanBeNull] T element)
    {
      if (element == null)
        throw new ArgumentNullException(nameof(element));

      _collection.Insert(index, element);
      _collectionElement.Add(element.Element);
    }

    /// <summary>
    ///   Removes the given <paramref name="element" /> from the collection.
    /// </summary>
    public bool Remove ([CanBeNull] T element)
    {
      if (element == null)
        throw new ArgumentNullException(nameof(element));

      var removed = _collection.Remove(element);
      if (removed)
        element.Element.Remove();

      return removed;
    }

    /// <summary>
    ///   Removes the element at the given <paramref name="index" /> from the collection.
    /// </summary>
    public void RemoveAt (int index)
    {
      var element = _collection[index];
      _collection.RemoveAt(index);
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

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public bool Contains ([CanBeNull] T item)
    {
      return _collection.Contains(item);
    }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public void CopyTo (T[] array, int arrayIndex)
    {
      _collection.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public bool IsReadOnly => false;

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public int IndexOf ([CanBeNull] T item)
    {
      return _collection.IndexOf(item);
    }
  }
}