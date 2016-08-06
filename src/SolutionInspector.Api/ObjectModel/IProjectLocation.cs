using System;
using JetBrains.Annotations;

namespace SolutionInspector.Api.ObjectModel
{
  /// <summary>
  ///   Represents the location (line/column) of an <see cref="IProjectItem" />/<see cref="IProjectProperty" /> in a project file.
  /// </summary>
  [PublicAPI]
  public interface IProjectLocation
  {
    /// <summary>
    ///   The line number.
    /// </summary>
    int Line { get; }

    /// <summary>
    ///   The column.
    /// </summary>
    int Column { get; }
  }
}