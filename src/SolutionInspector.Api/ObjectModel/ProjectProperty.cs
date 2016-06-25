using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace SolutionInspector.Api.ObjectModel
{
  ///// <summary>
  /////   Represents an property of a <see cref="IProject" />.
  ///// </summary>
  //public interface IProjectProperty : IProjectPropertyBase
  //{
  //  /// <summary>
  //  ///   The property's value. Variables contained in the value are not expanded.
  //  /// </summary>
  //  string Value { get; }
  //}

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

  [DebuggerDisplay ("{Name}")]
  internal class ProjectProperty : IProjectProperty, IEnumerable<IProjectPropertyOccurrence>
  {
    private readonly List<IProjectPropertyOccurrence> _occurrences = new List<IProjectPropertyOccurrence>();

    public string Name { get; }
    public string DefaultValue { get; }

    [ExcludeFromCodeCoverage]
    public IReadOnlyCollection<IProjectPropertyOccurrence> Occurrences => _occurrences;

    public ProjectProperty (string name, string defaultValue)
    {
      Name = name;
      DefaultValue = defaultValue;
    }

    public void Add (IProjectPropertyOccurrence occurrence)
    {
      _occurrences.Add(occurrence);
    }

    [ExcludeFromCodeCoverage]
    public IEnumerator<IProjectPropertyOccurrence> GetEnumerator ()
    {
      return _occurrences.GetEnumerator();
    }

    [ExcludeFromCodeCoverage]
    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }
  }
}