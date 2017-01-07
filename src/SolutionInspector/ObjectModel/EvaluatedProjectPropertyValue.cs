using SolutionInspector.Api.ObjectModel;

namespace SolutionInspector.ObjectModel
{
  internal class EvaluatedProjectPropertyValue : IEvaluatedProjectPropertyValue
  {
    public EvaluatedProjectPropertyValue (string value, IProjectPropertyOccurrence sourceOccurrence)
    {
      Value = value;
      SourceOccurrence = sourceOccurrence;
    }

    public string Value { get; }
    public IProjectPropertyOccurrence SourceOccurrence { get; }
  }
}