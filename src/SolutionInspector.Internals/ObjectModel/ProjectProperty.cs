using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using SolutionInspector.Api.ObjectModel;

namespace SolutionInspector.Internals.ObjectModel
{
  [DebuggerDisplay ("{" + nameof(Name) + "}")]
  internal class ProjectProperty : IProjectProperty, IEnumerable<IProjectPropertyOccurrence>
  {
    private readonly List<IProjectPropertyOccurrence> _occurrences = new List<IProjectPropertyOccurrence>();

    public ProjectProperty (string name, string defaultValue)
    {
      Name = name;
      DefaultValue = defaultValue;
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

    public string Name { get; }
    public string DefaultValue { get; }

    [ExcludeFromCodeCoverage]
    public IReadOnlyCollection<IProjectPropertyOccurrence> Occurrences => _occurrences;

    public void Add (IProjectPropertyOccurrence occurrence)
    {
      _occurrences.Add(occurrence);
    }
  }
}