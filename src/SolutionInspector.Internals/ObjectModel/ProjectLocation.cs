using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Microsoft.Build.Construction;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Commons.Utilities;

namespace SolutionInspector.Internals.ObjectModel
{
  /// <inheritdoc cref="IProjectLocation" />
  public sealed class ProjectLocation : IProjectLocation, IEquatable<ProjectLocation>
  {
    internal ProjectLocation (int line, int column)
    {
      Column = column;
      Line = line;
    }

    internal ProjectLocation (ProjectElement projectElement)
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

    public int Line { get; }
    public int Column { get; }

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

    /// <inheritdoc />
    public static bool operator == (ProjectLocation left, ProjectLocation right)
    {
      return Equals(left, right);
    }

    /// <inheritdoc />
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