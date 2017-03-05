using System.Collections.Generic;

namespace SolutionInspector.Api
{
  /// <summary>
  ///   Filter that checks a string against a set of include/exclude filters.
  /// </summary>
  public interface INameFilter
  {
    /// <summary>
    ///   Returns <c>true</c> if the given <paramref name="name" /> matches the filter, <c>false</c> otherwise.
    /// </summary>
    bool IsMatch (string name);

    /// <summary>
    ///   The list of include strings.
    /// </summary>
    IReadOnlyCollection<string> Includes { get; }

    /// <summary>
    ///   The list of exclude strings.
    /// </summary>
    IReadOnlyCollection<string> Excludes { get; }

    /// <summary>
    ///   Returns <see langword="true" /> when the includes do not have any filter and include everything, <see langword="false" /> otherwise.
    /// </summary>
    bool IncludesAll { get; }
  }
}