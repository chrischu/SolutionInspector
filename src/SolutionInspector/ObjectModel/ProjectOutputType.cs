namespace SolutionInspector.ObjectModel
{
  /// <summary>
  /// A MSBuild project's output type.
  /// </summary>
  public enum ProjectOutputType
  {
    /// <summary>
    /// Building the project creates an executable file (.exe).
    /// </summary>
    Executable,
    /// <summary>
    /// Building the project creates a library file (.dll).
    /// </summary>
    Library
  }
}