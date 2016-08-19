using JetBrains.Annotations;

namespace SolutionInspector.Commons.Extensions
{
  /// <summary>
  ///   Extension methods for <see cref="string"/>.
  /// </summary>
  public static class StringExtensions
  {
    /// <summary>
    /// Removes the given <paramref name="suffix"/> from the string.
    /// </summary>
    public static string RemoveSuffix ([CanBeNull] this string s, string suffix)
    {
      if (s == null)
        return null;

      return s.EndsWith(suffix) ? s.Substring(0, s.Length - suffix.Length) : s;
    }
  }
}