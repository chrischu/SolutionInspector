using System;
using System.Collections.Generic;

namespace SolutionInspector.Api.ObjectModel
{
  /// <summary>
  ///   Represents an property of a <see cref="IProject" /> and its (possibly multiple) occurrences in the MSBuild file.
  /// </summary>
  public interface IProjectProperty
  {
    /// <summary>
    ///   The property's name.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///   The property's value given all other properties have their default values.
    /// </summary>
    string DefaultValue { get; }

    /// <summary>
    ///   The property's (possibly multiple) occurrences.
    /// </summary>
    IReadOnlyCollection<IProjectPropertyOccurrence> Occurrences { get; }
  }
}