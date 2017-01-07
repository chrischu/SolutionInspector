using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace SolutionInspector.Api.ObjectModel
{
  /// <summary>
  ///   Represents a MSBuild project item build action (e.g. None, Compile).
  /// </summary>
  [PublicAPI]
  public sealed class ProjectItemBuildAction : IEquatable<ProjectItemBuildAction>
  {
    /// <summary>
    ///   Represents the <c>None</c> build action.
    /// </summary>
    public static readonly ProjectItemBuildAction None = new ProjectItemBuildAction("None");

    /// <summary>
    ///   Represents the <c>Compile</c> build action.
    /// </summary>
    public static readonly ProjectItemBuildAction Compile = new ProjectItemBuildAction("Compile");

    /// <summary>
    ///   Represents the <c>Content</c> build action.
    /// </summary>
    public static readonly ProjectItemBuildAction Content = new ProjectItemBuildAction("Content");

    /// <summary>
    ///   Represents the <c>EmbeddedResource</c> build action.
    /// </summary>
    public static readonly ProjectItemBuildAction EmbeddedResource = new ProjectItemBuildAction("EmbeddedResource");

    private readonly string _value;

    private ProjectItemBuildAction (string value)
    {
      _value = value;
    }

    /// <inheritdoc />
    public bool Equals ([CanBeNull] ProjectItemBuildAction other)
    {
      if (ReferenceEquals(null, other))
        return false;
      if (ReferenceEquals(this, other))
        return true;
      return string.Equals(_value, other._value);
    }

    /// <summary>
    ///   Allows the creation of a custom <see cref="ProjectItemBuildAction" />.
    /// </summary>
    public static ProjectItemBuildAction Custom (string value)
    {
      return new ProjectItemBuildAction(value);
    }

    /// <inheritdoc />
    public override bool Equals ([CanBeNull] object obj)
    {
      if (ReferenceEquals(null, obj))
        return false;
      if (ReferenceEquals(this, obj))
        return true;
      return obj is ProjectItemBuildAction && Equals((ProjectItemBuildAction) obj);
    }

    [ExcludeFromCodeCoverage]
    public override int GetHashCode ()
    {
      return _value.GetHashCode();
    }

    /// <inheritdoc />
    public static bool operator == ([CanBeNull] ProjectItemBuildAction left, [CanBeNull] ProjectItemBuildAction right)
    {
      return Equals(left, right);
    }

    /// <inheritdoc />
    public static bool operator != ([CanBeNull] ProjectItemBuildAction left, [CanBeNull] ProjectItemBuildAction right)
    {
      return !(left == right);
    }

    /// <inheritdoc />
    public static bool operator == ([CanBeNull] ProjectItemBuildAction left, [CanBeNull] string right)
    {
      return Equals(left?._value, right);
    }

    /// <inheritdoc />
    public static bool operator != ([CanBeNull] ProjectItemBuildAction left, [CanBeNull] string right)
    {
      return !(left == right);
    }

    public override string ToString ()
    {
      return _value;
    }
  }
}