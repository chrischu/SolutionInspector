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
  internal class ProjectProperty : IProjectProperty
  {
    public string Name { get; }
    public string Value { get; }

    public ProjectProperty (ProjectPropertyElement property)
        : this(property.Name, property.Value)
    {
      Name = property.Name;
      Value = property.Value;
    }

    protected ProjectProperty (string name, string value)
    {
      Name = name;
      Value = value;
    }
  }
}