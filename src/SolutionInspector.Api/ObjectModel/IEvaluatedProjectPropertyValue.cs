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
}