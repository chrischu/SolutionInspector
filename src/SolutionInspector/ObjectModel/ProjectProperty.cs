using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using SolutionInspector.Api.ObjectModel;

namespace SolutionInspector.ObjectModel
{
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