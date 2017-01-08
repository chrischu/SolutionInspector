using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SolutionInspector.Configuration
{
  /// <summary>
  ///   A collection that can be used in configurations and that is serialized as a comma-separated string.
  /// </summary>
  public class CommaSeparatedStringCollection : ConfigurationValue<CommaSeparatedStringCollection>, IEnumerable<string>
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

    /// <summary>
    ///   Gets the total count of elements in the collection.
    /// </summary>
    public int Count => _collection.Count;

    /// <summary>
    ///   Access an element in the collection by its <paramref name="index" />.
    /// </summary>
    public string this [int index]
    {
      get { return _collection[index]; }
      set { _collection[index] = value; }
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

    /// <summary>
    ///   Adds a new <paramref name="item" /> to the collection.
    /// </summary>
    public void Add (string item)
    {
      AddRange(new[] { item });
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

    /// <summary>
    ///   Clears the collection of all its items.
    /// </summary>
    public void Clear ()
    {
      _collection.Clear();
      Update();
    }

    /// <summary>
    ///   Checks whether the collection contains the given <paramref name="item" /> and returns <see langword="true" /> if so, or <see langword="false" />
    ///   otherwise.
    /// </summary>
    public bool Contains (string item)
    {
      return _collection.Contains(item);
    }

    /// <summary>
    ///   Removes the given <paramref name="item" /> from the collection if it exists.
    /// </summary>
    /// <returns><see langword="True" /> if the item was contained in the collection, <see langword="false" /> otherwise.</returns>
    public bool Remove (string item)
    {
      var removed = _collection.Remove(item);
      if (removed)
        Update();
      return removed;
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }
  }
}