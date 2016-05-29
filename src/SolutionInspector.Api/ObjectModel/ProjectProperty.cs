using System;
using System.Diagnostics;
using Microsoft.Build.Construction;

namespace SolutionInspector.Api.ObjectModel
{
  /// <summary>
  /// Represents a property of a <see cref="IProject"/>.
  /// </summary>
  public interface IProjectProperty
  {
    /// <summary>
    /// The property's name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The property's value. Variables contained in the value are not expanded.
    /// </summary>
    string Value { get; }
  }

  [DebuggerDisplay ("{Name} = {Value}")]
  internal sealed class ProjectProperty : IProjectProperty, IEquatable<ProjectProperty>
  {
    public string Name { get; }
    public string Value { get; }

    public ProjectProperty (ProjectPropertyElement property)
        : this(property.Name, property.Value)
    {
      Name = property.Name;
      Value = property.Value;
    }

    private ProjectProperty (string name, string value)
    {
      Name = name;
      Value = value;
    }

    public bool Equals (ProjectProperty other)
    {
      if (ReferenceEquals(null, other))
        return false;
      if (ReferenceEquals(this, other))
        return true;
      return string.Equals(Name, other.Name) && string.Equals(Value, other.Value);
    }

    public override bool Equals (object obj)
    {
      if (ReferenceEquals(null, obj))
        return false;
      if (ReferenceEquals(this, obj))
        return true;
      if (obj.GetType() != typeof (ProjectProperty))
        return false;
      return Equals((ProjectProperty) obj);
    }

    public override int GetHashCode ()
    {
      unchecked
      {
        return (Name.GetHashCode() * 397) ^ Value.GetHashCode();
      }
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