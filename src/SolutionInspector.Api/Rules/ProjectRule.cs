using System;
using SolutionInspector.Api.ObjectModel;

namespace SolutionInspector.Api.Rules
{
  /// <inheritdoc />
  public abstract class ProjectRule : Rule<IProject>, IProjectRule
  {
  }
}