using System;
using System.Linq;

namespace SolutionInspector.Api.Utilities
{
  internal static class HashCodeHelper
  {
    public static int GetHashCode (params int[] hashCodes)
    {
      unchecked
      {
        var totalHash = 17;
        foreach (var hashCode in hashCodes)
          totalHash = totalHash * 23 + hashCode;
        return totalHash;
      }
    }

    public static int GetHashCode(params object[] objects)
    {
      return GetHashCode(objects.Select(o => o.GetHashCode()).ToArray());
    }
  }
}