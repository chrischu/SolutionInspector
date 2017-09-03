using JetBrains.Annotations;
using SolutionInspector.Api.ObjectModel;

namespace SolutionInspector.Api.Rules
{
  /// <inheritdoc cref="IProjectItemRule" />
  [PublicAPI]
  public abstract class ProjectItemRule : Rule<IProjectItem>, IProjectItemRule
  {
  }
}