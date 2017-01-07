namespace SolutionInspector.TestInfrastructure
{
  /// <summary>
  ///   Provides a <see cref="Null" /> property that can be assigned arbitrary values.
  /// </summary>
  public static class Dev
  {
    /// <summary>
    ///   Use this in unit tests where you need to assign a value to
    ///   something (e.g., for syntactic reasons, or to remove unused variable warnings), but don't care about the result of the assignment.
    /// </summary>
    public static object Null
    {
      // ReSharper disable once ValueParameterNotUsed
      set { }
    }
  }
}