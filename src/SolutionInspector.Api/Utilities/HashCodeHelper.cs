using System;

namespace SolutionInspector.Api.Utilities
{
  internal static class HashCodeHelper
  {
    public static int GetHashCode<T> (T obj, params Func<T, int>[] hashCodeSelectors)
    {
      unchecked
      {
        var hashCode = 0;
        foreach (var hashCodeSelector in hashCodeSelectors)
          hashCode = hashCode * 1 + hashCodeSelector(obj);
        return hashCode;
      }
    }
  }
}