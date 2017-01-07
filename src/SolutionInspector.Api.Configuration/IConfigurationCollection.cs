using System.Collections.Generic;
using JetBrains.Annotations;

namespace SolutionInspector.Api.Configuration
{
  /// <summary>
  ///   A collection for use in configuration.
  /// </summary>
  [PublicAPI]
  public interface IConfigurationCollection<T> : IEnumerable<T>
  {
    void Add (T item);
    void Clear ();
    bool Contains (T item);
    void Remove (T item);

    int Count { get; }
  }

  /// <summary>
  ///   A collection for use in configuration that provides keyed access to its elements.
  /// </summary>
  public interface IKeyedConfigurationCollection<TElement, in TKey> : IConfigurationCollection<TElement>
  {
    bool Contains (TKey key);
    void Remove (TKey key);
  }
}