using System;
using System.Collections.Generic;

namespace SolutionInspector.Commons.Extensions
{
  /// <summary>
  ///   Extension methods for collection types.
  /// </summary>
  public static class CollectionExtensions
  {
    /// <summary>
    ///   Add all the items from <paramref name="enumerable" /> to the <paramref name="collection" />.
    /// </summary>
    public static void AddRange<T> (this ICollection<T> collection, IEnumerable<T> enumerable)
    {
      foreach (var item in enumerable)
        collection.Add(item);
    }
  }
}