using System;
using System.Collections.Generic;

namespace SolutionInspector.Api.Utilities
{
  internal class DictionaryEqualityComparer<TKey, TValue> : IEqualityComparer<IReadOnlyDictionary<TKey, TValue>>
  {
    public bool Equals (IReadOnlyDictionary<TKey, TValue> x, IReadOnlyDictionary<TKey, TValue> y)
    {
      if (x == y)
        return true;
      if ((x == null) || (y == null))
        return false;
      if (x.Count != y.Count)
        return false;

      foreach (var kvp in x)
      {
        TValue secondValue;
        if (!y.TryGetValue(kvp.Key, out secondValue))
          return false;
        if (!Equals(kvp.Value, secondValue))
          return false;
      }
      return true;
    }

    public int GetHashCode (IReadOnlyDictionary<TKey, TValue> obj)
    {
      unchecked
      {
        var hashCode = 17;
        foreach (var kvp in obj)
        {
          hashCode = hashCode * 23 + kvp.Key.GetHashCode();
          hashCode = hashCode * 23 + kvp.Value.GetHashCode();
        }

        return hashCode;
      }
    }
  }
}