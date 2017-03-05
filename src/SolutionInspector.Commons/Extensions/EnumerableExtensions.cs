using System;
using System.Collections.Generic;
using System.Linq;

namespace SolutionInspector.Commons.Extensions
{
  /// <summary>
  ///   Extension methods for <see cref="IEnumerable{T}" />.
  /// </summary>
  public static class EnumerableExtensions
  {
    /// <summary>
    ///   Checks if the given <paramref name="enumerable" /> contains more than one elements.
    /// </summary>
    public static bool ContainsMoreThanOne<T> (this IEnumerable<T> enumerable)
    {
      return enumerable.ContainsMoreThan(1);
    }

    /// <summary>
    ///   Checks if the given <paramref name="enumerable" /> contains more than <paramref name="count" /> elements.
    /// </summary>
    private static bool ContainsMoreThan<T> (this IEnumerable<T> enumerable, int count)
    {
      return enumerable.Skip(count).Any();
    }

    public static IEnumerable<TSource> DistinctBy<TSource, TKey> (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
      var knownKeys = new HashSet<TKey>();

      foreach (var element in source)
        if (knownKeys.Add(keySelector(element)))
          yield return element;
    }

    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
      foreach (var item in source)
        action(item);
    }
  }
}