using System;
using System.Collections.Generic;
using SolutionInspector.Commons.Attributes;

namespace SolutionInspector.Api.Configuration
{
  /// <summary>
  ///   A collection for use in configuration.
  /// </summary>
  [PublicApi]
  public interface IConfigurationCollection<T> : IEnumerable<T>
  {
    /// <summary>
    ///   The number of elements in the collection.
    /// </summary>
    int Count { get; }

    /// <summary>
    ///   Adds the <paramref name="item" /> to the collection.
    /// </summary>
    /// <param name="item"></param>
    void Add (T item);

    /// <summary>
    ///   Clears the collection.
    /// </summary>
    void Clear ();

    /// <summary>
    ///   Checks if the <paramref name="item" /> is contained in the collection and returns <see langword="true" /> if so, <see langword="false" />
    ///   otherwise.
    /// </summary>
    bool Contains (T item);

    /// <summary>
    ///   Removes the <paramref name="item" /> from the collection.
    /// </summary>
    void Remove (T item);
  }

  /// <summary>
  ///   A collection for use in configuration that provides keyed access to its elements.
  /// </summary>
  [PublicApi]
  public interface IKeyedConfigurationCollection<TElement, in TKey> : IConfigurationCollection<TElement>
  {
    /// <summary>
    ///   Checks if an item with the given <paramref name="key" /> is contained in the and returns <see langword="true" /> if so, <see langword="false" />
    ///   otherwise.
    /// </summary>
    bool Contains (TKey key);

    /// <summary>
    ///   Removes the item with the given <paramref name="key" /> from the collection.
    /// </summary>
    void Remove (TKey key);
  }
}