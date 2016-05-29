using System;

namespace SolutionInspector.Api.ObjectModel
{
  /// <summary>
  ///  Represents the include (path relative to the csproj file) of an <see cref="IProjectItem"/>.
  /// </summary>
  public interface IProjectItemInclude
  {
    /// <summary>
    /// The evaluated include (variables have been replaced).
    /// </summary>
    string Evaluated { get; }

    /// <summary>
    /// The evaluated include (variables have *not* been replaced).
    /// </summary>
    string Unevaluated { get; }
  }

  internal sealed class ProjectItemInclude : IProjectItemInclude, IEquatable<ProjectItemInclude>
  {
    public string Evaluated { get; }
    public string Unevaluated { get; }

    public ProjectItemInclude (string evaluated, string unevaluated)
    {
      Evaluated = evaluated;
      Unevaluated = unevaluated;
    }

    public bool Equals (ProjectItemInclude other)
    {
      if (ReferenceEquals(null, other))
        return false;
      if (ReferenceEquals(this, other))
        return true;
      return string.Equals(Evaluated, other.Evaluated) && string.Equals(Unevaluated, other.Unevaluated);
    }

    public override bool Equals (object obj)
    {
      if (ReferenceEquals(null, obj))
        return false;
      if (ReferenceEquals(this, obj))
        return true;
      return obj is ProjectItemInclude && Equals((ProjectItemInclude) obj);
    }

    public override int GetHashCode ()
    {
      unchecked
      {
        return (Evaluated.GetHashCode() * 397) ^ Unevaluated.GetHashCode();
      }
    }

    public static bool operator == (ProjectItemInclude left, ProjectItemInclude right)
    {
      return Equals(left, right);
    }

    public static bool operator != (ProjectItemInclude left, ProjectItemInclude right)
    {
      return !Equals(left, right);
    }
  }
}