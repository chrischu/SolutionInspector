using SolutionInspector.Api.ObjectModel;

namespace SolutionInspector.Api.Rules
{
  /// <inheritdoc cref="IProjectRule" />
  public abstract class ProjectRule : Rule<IProject>, IProjectRule
  {
  }
}