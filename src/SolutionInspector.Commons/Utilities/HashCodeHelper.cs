using System.Linq;

namespace SolutionInspector.Commons.Utilities
{
  /// <summary>
  ///   Utility class to help generate hash codes.
  /// </summary>
  public static class HashCodeHelper
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

    public static int GetHashCode (params object[] objects)
    {
      return GetHashCode(objects.Select(o => o?.GetHashCode() ?? 0).ToArray());
    }
  }
}