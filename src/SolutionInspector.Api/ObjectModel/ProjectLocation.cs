using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Microsoft.Build.Construction;
using SolutionInspector.Api.Utilities;

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

  internal sealed class ProjectLocation : IProjectLocation, IEquatable<ProjectLocation>
  {
    public int Line { get; }
    public int Column { get; }

    public ProjectLocation (int line, int column)
    {
      Column = column;
      Line = line;
    }

    public ProjectLocation (ProjectElement projectElement)
        : this(projectElement.Location.Line, projectElement.Location.Column)
    {
    }

    public bool Equals ([CanBeNull] ProjectLocation other)
    {
      if (ReferenceEquals(null, other))
        return false;
      if (ReferenceEquals(this, other))
        return true;
      return Line == other.Line && Column == other.Column;
    }

    public override bool Equals ([CanBeNull] object obj)
    {
      if (ReferenceEquals(null, obj))
        return false;
      if (ReferenceEquals(this, obj))
        return true;
      return obj is ProjectLocation && Equals((ProjectLocation) obj);
    }

    [ExcludeFromCodeCoverage]
    public override int GetHashCode ()
    {
      return HashCodeHelper.GetHashCode(Line, Column);
    }

    public static bool operator == (ProjectLocation left, ProjectLocation right)
    {
      return Equals(left, right);
    }

    public static bool operator != (ProjectLocation left, ProjectLocation right)
    {
      return !Equals(left, right);
    }

    [ExcludeFromCodeCoverage]
    public override string ToString ()
    {
      return $"({Line},{Column})";
    }
  }
}