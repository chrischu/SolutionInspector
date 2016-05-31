using System;
using Microsoft.Build.Construction;
using SolutionInspector.Api.Utilities;

namespace SolutionInspector.Api.ObjectModel
{
  /// <summary>
  /// Represents a property of a <see cref="IProject"/>.
  /// </summary>
  public interface IProjectPropertyBase
  {
    /// <summary>
    /// The property's name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The property's location in the project file.
    /// </summary>
    IProjectLocation Location { get; }
  }

  internal abstract class ProjectPropertyBase : IProjectPropertyBase
  {
    protected ProjectPropertyBase(ProjectPropertyElement property)
    {
      Name = property.Name;
      Location = new ProjectLocation(property.Location.Line, property.Location.Column);
    }

    public string Name { get; }
    public IProjectLocation Location { get; }

    public bool Equals(ProjectPropertyBase other)
    {
      if (ReferenceEquals(null, other))
        return false;
      if (ReferenceEquals(this, other))
        return true;
      return string.Equals(Name, other.Name) && Equals(Location, other.Location);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj))
        return false;
      if (ReferenceEquals(this, obj))
        return true;
      return obj.GetType() == typeof(ProjectPropertyBase) && Equals((ProjectPropertyBase)obj);
    }

    public override int GetHashCode()
    {
      return HashCodeHelper.GetHashCode(Name, Location);
    }

    public static bool operator ==(ProjectPropertyBase left, ProjectPropertyBase right)
    {
      return Equals(left, right);
    }

    public static bool operator !=(ProjectPropertyBase left, ProjectPropertyBase right)
    {
      return !Equals(left, right);
    }
  }
}