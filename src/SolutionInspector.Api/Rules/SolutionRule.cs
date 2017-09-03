using JetBrains.Annotations;
using SolutionInspector.Api.ObjectModel;

namespace SolutionInspector.Api.Rules
{
  /// <inheritdoc cref="ISolutionRule" />
  [PublicAPI]
  public abstract class SolutionRule : Rule<ISolution>, ISolutionRule
  {
  }
}