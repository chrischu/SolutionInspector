using System;

namespace SolutionInspector.Api.ObjectModel
{
  /// <summary>
  ///   Represents an evaluated <see cref="IProjectProperty" />.
  /// </summary>
  public interface IEvaluatedProjectPropertyValue
  {
    /// <summary>
    ///   The value.
    /// </summary>
    string Value { get; }

    /// <summary>
    ///   The occurrence of the property from which the <see cref="Value" /> originates.
    /// </summary>
    IProjectPropertyOccurrence SourceOccurrence { get; }
  }

  internal class EvaluatedProjectPropertyValue : IEvaluatedProjectPropertyValue
  {
    public string Value { get; }
    public IProjectPropertyOccurrence SourceOccurrence { get; }

    public EvaluatedProjectPropertyValue (string value, IProjectPropertyOccurrence sourceOccurrence)
    {
      Value = value;
      SourceOccurrence = sourceOccurrence;
    }
  }
}