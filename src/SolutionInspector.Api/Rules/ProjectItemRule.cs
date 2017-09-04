using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Commons.Attributes;

namespace SolutionInspector.Api.Rules
{
  /// <inheritdoc cref="IProjectItemRule" />
  [PublicApi]
  public abstract class ProjectItemRule : Rule<IProjectItem>, IProjectItemRule
  {
  }
}