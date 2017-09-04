using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Commons.Attributes;

namespace SolutionInspector.Api.Rules
{
  /// <inheritdoc cref="ISolutionRule" />
  [PublicApi]
  public abstract class SolutionRule : Rule<ISolution>, ISolutionRule
  {
  }
}