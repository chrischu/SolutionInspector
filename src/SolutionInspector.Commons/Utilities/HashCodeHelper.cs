using System;
using System.Linq;

namespace SolutionInspector.Commons.Utilities
{
  /// <summary>
  ///   Utility class to help generate hash codes.
  /// </summary>
  public static class HashCodeHelper
  {
    /// <summary>
    ///   Combines the given <paramref name="hashCodes" /> into one hash code.
    /// </summary>
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

    /// <summary>
    ///   Calculates the hash code from all the <paramref name="objects" /> and combines the results into one hash code.
    /// </summary>
    public static int GetHashCode (params object[] objects)
    {
      return GetHashCode(objects.Select(o => o?.GetHashCode() ?? 0).ToArray());
    }
  }
}