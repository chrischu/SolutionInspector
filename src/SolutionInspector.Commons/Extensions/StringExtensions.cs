using System;
using JetBrains.Annotations;

namespace SolutionInspector.Commons.Extensions
{
  /// <summary>
  ///   Extension methods for <see cref="string" />.
  /// </summary>
  public static class StringExtensions
  {
    /// <summary>
    ///   Removes the given <paramref name="suffix" /> from the string.
    /// </summary>
    [CanBeNull]
    public static string RemoveSuffix ([CanBeNull] this string s, string suffix)
    {
      if (s == null)
        return null;

      return s.EndsWith(suffix) ? s.Substring(0, s.Length - suffix.Length) : s;
    }

    [CanBeNull]
    public static string Prefix([CanBeNull] this string s, int length)
    {
      return s?.Substring(0, Math.Min(s.Length, length));
    }

    [CanBeNull]
    public static string Suffix([CanBeNull] this string s, int length)
    {
      return s?.Substring(Math.Max(0, s.Length - length));
    }
  }
}