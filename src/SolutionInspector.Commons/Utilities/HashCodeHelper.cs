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
        return hashCodes.Aggregate(17, (current, hashCode) => current * 23 + hashCode);
      }
    }

    /// <summary>
    ///   Combines the given <paramref name="hashCodes" /> into one hash code in a way that the order of the given <paramref name="hashCodes" /> does not
    ///   matter.
    /// </summary>
    public static int GetOrderIndependentHashCode (params int[] hashCodes)
    {
      return hashCodes.Aggregate(0, (current, hashCode) => current ^ hashCode);
    }

    /// <summary>
    ///   Calculates the hash code from all the <paramref name="objects" /> and combines the results into one hash code.
    /// </summary>
    public static int GetHashCode (params object[] objects)
    {
      return GetHashCode(objects.Select(o => o?.GetHashCode() ?? 0).ToArray());
    }

    /// <summary>
    ///   Calculates the hash code from all the <paramref name="objects" /> and combines the results into one hash code in a way that the order of the given
    ///   <paramref name="objects" /> does not matter.
    /// </summary>
    public static int GetOrderIndependentHashCode (params object[] objects)
    {
      return GetOrderIndependentHashCode(objects.Select(o => o?.GetHashCode() ?? 0).ToArray());
    }
  }
}