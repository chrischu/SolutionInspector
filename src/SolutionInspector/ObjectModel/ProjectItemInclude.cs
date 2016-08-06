using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Utilities;

namespace SolutionInspector.ObjectModel
{
  internal sealed class ProjectItemInclude : IProjectItemInclude, IEquatable<ProjectItemInclude>
  {
    public string Evaluated { get; }
    public string Unevaluated { get; }

    public ProjectItemInclude (string evaluated, string unevaluated)
    {
      Evaluated = evaluated;
      Unevaluated = unevaluated;
    }

    public bool Equals ([CanBeNull] ProjectItemInclude other)
    {
      if (ReferenceEquals(null, other))
        return false;
      if (ReferenceEquals(this, other))
        return true;
      return string.Equals(Evaluated, other.Evaluated) && string.Equals(Unevaluated, other.Unevaluated);
    }

    public override bool Equals ([CanBeNull] object obj)
    {
      if (ReferenceEquals(null, obj))
        return false;
      if (ReferenceEquals(this, obj))
        return true;
      return obj is ProjectItemInclude && Equals((ProjectItemInclude) obj);
    }

    [ExcludeFromCodeCoverage]
    public override int GetHashCode ()
    {
      return HashCodeHelper.GetHashCode(Evaluated, Unevaluated);
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