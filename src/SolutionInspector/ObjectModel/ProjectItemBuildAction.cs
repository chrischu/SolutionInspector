using System;
using JetBrains.Annotations;

namespace SolutionInspector.ObjectModel
{
  /// <summary>
  /// Represents a MSBuild project item build action (e.g. None, Compile).
  /// </summary>
  [PublicAPI]
  public class ProjectItemBuildAction : IEquatable<ProjectItemBuildAction>
  {
    private readonly string _value;

    /// <summary>
    /// Represents the <c>None</c> build action.
    /// </summary>
    public static readonly ProjectItemBuildAction None = new ProjectItemBuildAction("None");

    /// <summary>
    /// Represents the <c>Compile</c> build action.
    /// </summary>
    public static readonly ProjectItemBuildAction Compile = new ProjectItemBuildAction("Compile");

    /// <summary>
    /// Represents the <c>Content</c> build action.
    /// </summary>
    public static readonly ProjectItemBuildAction Content = new ProjectItemBuildAction("Content");

    /// <summary>
    /// Represents the <c>EmbeddedResource</c> build action.
    /// </summary>
    public static readonly ProjectItemBuildAction EmbeddedResource = new ProjectItemBuildAction("EmbeddedResource");

    /// <summary>
    /// Allows the creation of a custom <see cref="ProjectItemBuildAction"/>.
    /// </summary>
    public static ProjectItemBuildAction Custom(string value)
    {
      return new ProjectItemBuildAction(value);
    }

    private ProjectItemBuildAction(string value)
    {
      _value = value;
    }

    /// <inheritdoc />
    public bool Equals([CanBeNull] ProjectItemBuildAction other)
    {
      if (ReferenceEquals(null, other))
        return false;
      if (ReferenceEquals(this, other))
        return true;
      return string.Equals(_value, other._value);
    }

    /// <inheritdoc />
    public override bool Equals([CanBeNull] object obj)
    {
      if (ReferenceEquals(null, obj))
        return false;
      if (ReferenceEquals(this, obj))
        return true;
      if (obj.GetType() != GetType())
        return false;
      return Equals((ProjectItemBuildAction) obj);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
      return _value.GetHashCode();
    }

    /// <inheritdoc />
    public static bool operator ==([CanBeNull] ProjectItemBuildAction left, [CanBeNull] ProjectItemBuildAction right)
    {
      return Equals(left, right);
    }

    /// <inheritdoc />
    public static bool operator !=([CanBeNull] ProjectItemBuildAction left, [CanBeNull] ProjectItemBuildAction right)
    {
      return !(left == right);
    }

    /// <inheritdoc />
    public static bool operator ==([CanBeNull] ProjectItemBuildAction left, [CanBeNull] string right)
    {
      return Equals(left?._value, right);
    }

    /// <inheritdoc />
    public static bool operator !=([CanBeNull] ProjectItemBuildAction left, [CanBeNull] string right)
    {
      return !(left == right);
    }
  }
}