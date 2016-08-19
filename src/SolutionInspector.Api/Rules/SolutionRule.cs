using JetBrains.Annotations;
using SolutionInspector.Api.ObjectModel;

namespace SolutionInspector.Api.Rules
{
  /// <inheritdoc />
  [PublicAPI]
  public abstract class SolutionRule : Rule<ISolution>, ISolutionRule
  {
  }
}