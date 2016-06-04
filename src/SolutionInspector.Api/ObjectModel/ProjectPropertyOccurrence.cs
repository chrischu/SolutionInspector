using System;
using System.Diagnostics;
using JetBrains.Annotations;
using Microsoft.Build.Construction;

namespace SolutionInspector.Api.ObjectModel
{
  /// <summary>
  ///   Represents a <see cref="IProjectProperty" />'s occurrence in the source MSBuild file.
  /// </summary>
  [PublicAPI]
  public interface IProjectPropertyOccurrence
  {
    /// <summary>
    ///   The value.
    /// </summary>
    string Value { get; }

    /// <summary>
    ///   The condition that must be fulfilled for the occurrence to be active.
    /// </summary>
    [CanBeNull]
    IProjectPropertyCondition Condition { get; }

    /// <summary>
    ///   The location of the occurrence in the MSBuild file.
    /// </summary>
    IProjectLocation Location { get; }
  }

  [DebuggerDisplay ("{Value} at {Location} when ({Condition})")]
  internal class ProjectPropertyOccurrence : IProjectPropertyOccurrence
  {
    public string Value { get; }

    [CanBeNull]
    public IProjectPropertyCondition Condition { get; }

    public IProjectLocation Location { get; }

    public ProjectPropertyOccurrence (string value, [CanBeNull] IProjectPropertyCondition condition, IProjectLocation location)
    {
      Value = value;
      Condition = condition;
      Location = location;
    }

    public ProjectPropertyOccurrence (ProjectPropertyElement property)
        : this(property.Value, CreateCondition(property), new ProjectLocation(property))
    {
    }

    private static ProjectPropertyCondition CreateCondition (ProjectPropertyElement property)
    {
      var condition = new ProjectPropertyCondition(property);
      if(condition.Parent != null || condition.Self != null)
        return condition;

      return null;
    }
  }
}