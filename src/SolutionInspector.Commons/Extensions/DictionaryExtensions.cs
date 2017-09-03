using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace SolutionInspector.Commons.Extensions
{
  /// <summary>
  ///   Extension methods for dictionary types.
  /// </summary>
  public static class DictionaryExtensions
  {
    /// <summary>
    ///   Tries to get the value with the given <paramref name="key" /> from the <paramref name="dictionary" />. Returns <paramref name="default" />
    ///   if no value can be found under the <paramref name="key" />.
    /// </summary>
    [CanBeNull]
    public static TValue GetValueOrDefault<TKey, TValue> (
      this IReadOnlyDictionary<TKey, TValue> dictionary,
      TKey key,
      TValue @default = default(TValue))
    {
      TValue value;
      return dictionary.TryGetValue(key, out value) ? value : @default;
    }

    /// <summary>
    ///   Tries to get the value with the given <paramref name="key" /> from the <paramref name="dictionary" /> or add a new value (created by the
    ///   <paramref name="valueFactory" />) for the given <paramref name="key" />.
    /// </summary>
    public static TValue GetOrAdd<TKey, TValue> (this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> valueFactory)
    {
      TValue value;
      if (!dictionary.TryGetValue(key, out value))
        value = dictionary[key] = valueFactory(key);

      return value;
    }
  }
}