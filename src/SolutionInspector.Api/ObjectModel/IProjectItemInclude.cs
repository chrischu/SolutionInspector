using System;

namespace SolutionInspector.Api.ObjectModel
{
  /// <summary>
  ///   Represents the include (path relative to the csproj file) of an <see cref="IProjectItem" />.
  /// </summary>
  public interface IProjectItemInclude
  {
    /// <summary>
    ///   The evaluated include (variables have been replaced).
    /// </summary>
    string Evaluated { get; }

    /// <summary>
    ///   The evaluated include (variables have *not* been replaced).
    /// </summary>
    string Unevaluated { get; }
  }
}