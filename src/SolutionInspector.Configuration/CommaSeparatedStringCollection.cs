using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace SolutionInspector.Configuration
{
  /// <summary>
  ///   A collection that can be used in configurations and that is serialized as a comma-separated string.
  /// </summary>
  public class CommaSeparatedStringCollection : ConfigurationValue<CommaSeparatedStringCollection>, IList<string>
  {
    private List<string> _collection = new List<string>();

    /// <summary>
    ///   Constructs a new <see cref="CommaSeparatedStringCollection" /> without any elements.
    /// </summary>
    public CommaSeparatedStringCollection (Action<string> updateAction)
      : base(updateAction)
    {
    }

    /// <summary>
    ///   Constructs a new <see cref="CommaSeparatedStringCollection" /> without the given <paramref name="elements" />.
    /// </summary>
    public CommaSeparatedStringCollection (Action<string> updateAction, IEnumerable<string> elements)
      : base(updateAction)
    {
      AddRange(elements);
    }

    /// <inheritdoc />
    public int Count => _collection.Count;

    /// <inheritdoc />
    public string this [int index]
    {
      get { return _collection[index]; }
      set
      {
        _collection[index] = value;
        Update();
      }
    }

    /// <summary>
    ///   Serializes the elements of the collection as comma-separated string.
    /// </summary>
    public override string Serialize ()
    {
      return string.Join(",", _collection);
    }

    /// <summary>
    ///   Deserializes the given <paramref name="serialized" /> string and replaces the elements in the collection with the ones from the string.
    /// </summary>
    public override void Deserialize (string serialized)
    {
      var collection = string.IsNullOrEmpty(serialized) ? Enumerable.Empty<string>() : serialized.Split(',');
      _collection = new List<string>(collection);
      Update();
    }

    /// <inheritdoc />
    public IEnumerator<string> GetEnumerator ()
    {
      return _collection.GetEnumerator();
    }

    /// <inheritdoc />
    public void Add ([CanBeNull] string item)
    {
      AddRange(new[] { item });
    }

    /// <inheritdoc />
    public void Insert (int index, [CanBeNull] string item)
    {
      _collection.Insert(index, item);
    }

    /// <summary>
    ///   Adds all given <paramref name="items" /> to the collection.
    /// </summary>
    public void Add (params string[] items)
    {
      AddRange(items);
    }

    /// <summary>
    ///   Adds all given <paramref name="items" /> to the collection.
    /// </summary>
    public void AddRange (IEnumerable<string> items)
    {
      _collection.AddRange(items);
      Update();
    }

    /// <inheritdoc />
    public void Clear ()
    {
      _collection.Clear();
      Update();
    }

    /// <inheritdoc />
    public bool Contains ([CanBeNull] string item)
    {
      return _collection.Contains(item);
    }

    /// <inheritdoc />
    public bool Remove ([CanBeNull] string item)
    {
      var removed = _collection.Remove(item);
      if (removed)
        Update();
      return removed;
    }

    /// <inheritdoc />
    public void RemoveAt (int index)
    {
      _collection.RemoveAt(index);
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }

    /// <inheritdoc />
    public void CopyTo (string[] array, int arrayIndex)
    {
      _collection.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc />
    public bool IsReadOnly => false;

    /// <inheritdoc />
    public int IndexOf ([CanBeNull] string item)
    {
      return _collection.IndexOf(item);
    }
  }
}