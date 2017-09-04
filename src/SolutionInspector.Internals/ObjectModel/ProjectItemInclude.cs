using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Commons.Utilities;

namespace SolutionInspector.Internals.ObjectModel
{
  /// <inheritdoc cref="IProjectItemInclude" />
  public sealed class ProjectItemInclude : IProjectItemInclude, IEquatable<ProjectItemInclude>
  {
    internal ProjectItemInclude (string evaluated, string unevaluated)
    {
      Evaluated = evaluated;
      Unevaluated = unevaluated;
    }

    /// <summary>
    ///   Compares two <see cref="ProjectItemInclude" />s.
    /// </summary>
    public bool Equals ([CanBeNull] ProjectItemInclude other)
    {
      if (ReferenceEquals(null, other))
        return false;
      if (ReferenceEquals(this, other))
        return true;
      return string.Equals(Evaluated, other.Evaluated) && string.Equals(Unevaluated, other.Unevaluated);
    }

    /// <inheritdoc />
    public string Evaluated { get; }
    /// <inheritdoc />
    public string Unevaluated { get; }

    /// <inheritdoc />
    public override bool Equals([CanBeNull] object obj)
    {
      if (ReferenceEquals(null, obj))
        return false;
      if (ReferenceEquals(this, obj))
        return true;
      var projectItemInclude = obj as ProjectItemInclude;
      return projectItemInclude != null && Equals(projectItemInclude);
    }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override int GetHashCode ()
    {
      return HashCodeHelper.GetHashCode(Evaluated, Unevaluated);
    }

    /// <summary>
    ///   Compares two <see cref="ProjectItemInclude" />s by using <see cref="Equals(ProjectItemInclude)" /> and returns <see langword="true" /> if
    ///   they are equal, <see langword="false" /> otherwise.
    /// </summary>
    public static bool operator == ([CanBeNull] ProjectItemInclude left, [CanBeNull] ProjectItemInclude right)
    {
      return Equals(left, right);
    }

    /// <summary>
    ///   Compares two <see cref="ProjectItemInclude" />s by using <see cref="Equals(ProjectItemInclude)" /> and returns <see langword="false" /> if
    ///   they are equal, <see langword="true" /> otherwise.
    /// </summary>
    public static bool operator != ([CanBeNull] ProjectItemInclude left, [CanBeNull] ProjectItemInclude right)
    {
      return !Equals(left, right);
    }
  }
}