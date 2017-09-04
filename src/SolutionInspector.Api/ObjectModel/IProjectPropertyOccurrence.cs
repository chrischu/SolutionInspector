using System;
using JetBrains.Annotations;
using SolutionInspector.Commons.Attributes;

namespace SolutionInspector.Api.ObjectModel
{
  /// <summary>
  ///   Represents a <see cref="IProjectProperty" />'s occurrence in the source MSBuild file.
  /// </summary>
  [PublicApi]
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
}