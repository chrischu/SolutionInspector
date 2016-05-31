using System;
using System.Diagnostics;
using Microsoft.Build.Construction;
using SolutionInspector.Api.Utilities;

namespace SolutionInspector.Api.ObjectModel
{
  /// <summary>
  ///   Represents an property of a <see cref="IProject" />.
  /// </summary>
  public interface IProjectProperty : IProjectPropertyBase
  {
    /// <summary>
    ///   The property's value. Variables contained in the value are not expanded.
    /// </summary>
    string Value { get; }
  }

  [DebuggerDisplay ("{Name} = {Value} {Location}")]
  internal sealed class ProjectProperty : ProjectPropertyBase, IEquatable<ProjectProperty>, IProjectProperty
  {
    public string Value { get; }

    public ProjectProperty (ProjectPropertyElement property)
        : base(property)
    {
      Value = property.Value;
    }

    public bool Equals (ProjectProperty other)
    {
      if (ReferenceEquals(null, other))
        return false;
      if (ReferenceEquals(this, other))
        return true;
      return base.Equals(other) && string.Equals(Value, other.Value);
    }

    public override bool Equals (object obj)
    {
      if (ReferenceEquals(null, obj))
        return false;
      if (ReferenceEquals(this, obj))
        return true;
      return obj is ProjectProperty && Equals((ProjectProperty) obj);
    }

    public override int GetHashCode ()
    {
      return HashCodeHelper.GetHashCode(base.GetHashCode(), Value.GetHashCode());
    }

    public static bool operator == (ProjectProperty left, ProjectProperty right)
    {
      return Equals(left, right);
    }

    public static bool operator != (ProjectProperty left, ProjectProperty right)
    {
      return !Equals(left, right);
    }
  }
}