namespace SolutionInspector.Extensions
{
  /// <summary>
  /// Extension methods for <see cref="string"/>.
  /// </summary>
  public static class StringExtensions
  {
    /// <summary>
    /// Returns a new string that had its first character converted to lower case.
    /// </summary>
    public static string FirstToLower(this string s)
    {
      return char.ToLower(s[0]) + s.Substring(1);
    }
  }
}
