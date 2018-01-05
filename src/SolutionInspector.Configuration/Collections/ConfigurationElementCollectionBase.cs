using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace SolutionInspector.Configuration.Collections
{
  internal abstract class ConfigurationElementCollectionBase<TElement> : IConfigurationElementCollectionBase<TElement>
      where TElement : ConfigurationElement
  {
    private readonly XElement _collectionElement;
    private readonly List<TElement> _items;

    protected ConfigurationElementCollectionBase (XElement collectionElement, IEnumerable<TElement> elements)
    {
      _collectionElement = collectionElement;

      ContainingDocument = _collectionElement.Document;
      if (ContainingDocument == null)
        throw new ArgumentException("The collection element is not attached to a document.", nameof(collectionElement));

      _items = elements.ToList();
    }

    protected XDocument ContainingDocument { get; }

    public int Count => _items.Count;

    [ExcludeFromCodeCoverage]
    public bool IsReadOnly => false;

    public TElement this [int index]
    {
      get => _items[index];
      set
      {
        ValidateNewElement(value);
        _items[index] = value;
      }
    }

    public IEnumerator<TElement> GetEnumerator ()
    {
      return _items.GetEnumerator();
    }

    [ExcludeFromCodeCoverage]
    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }

    public void Add ([CanBeNull] TElement element)
    {
      Insert(_items.Count, element);
    }

    public void Insert (int index, [CanBeNull] TElement element)
    {
      if (element == null)
        throw new ArgumentNullException(nameof(element));

      ValidateNewElement(element);

      _items.Insert(index, element);
      _collectionElement.Add(element.Element);
    }

    public void Clear ()
    {
      _items.Clear();
      _collectionElement.RemoveNodes();
    }

    [ExcludeFromCodeCoverage]
    public bool Contains ([CanBeNull] TElement item)
    {
      return _items.Contains(item);
    }

    [ExcludeFromCodeCoverage]
    public void CopyTo (TElement[] array, int arrayIndex)
    {
      _items.CopyTo(array, arrayIndex);
    }

    public bool Remove ([CanBeNull] TElement element)
    {
      if (element == null)
        throw new ArgumentNullException(nameof(element));

      var removed = _items.Remove(element);
      if (removed)
        element.Element.Remove();

      return removed;
    }

    public void RemoveAt (int index)
    {
      var element = _items[index];
      _items.RemoveAt(index);
      element.Element.Remove();
    }

    [ExcludeFromCodeCoverage]
    public int IndexOf ([CanBeNull] TElement item)
    {
      return _items.IndexOf(item);
    }

    protected virtual void ValidateNewElement (TElement element)
    {
    }
  }
}