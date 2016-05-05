using System;
using System.Collections.Generic;

namespace SolutionInspector.Api.Extensions
{
  /// <summary>
  /// Extension methods for dictionary types.
  /// </summary>
  public static class DictionaryExtensions
  {
    /// <summary>
    /// Tries to get the value with the given <paramref name="key"/> from the <paramref name="dictionary"/>. Returns <paramref name="default"/> 
    /// if no value can be found under the <paramref name="key"/>.
    /// </summary>
    /// <returns></returns>
    public static TValue GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, TValue @default = default(TValue))
    {
      TValue value;
      return dictionary.TryGetValue(key, out value) ? value : @default;
    }
  }
}