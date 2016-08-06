using System;
using SolutionInspector.Api.ObjectModel;

namespace SolutionInspector.Api.Rules
{
  /// <summary>
  ///   A SolutionInspector rule that targets <see cref="IProjectItem" />s.
  /// </summary>
  public interface IProjectItemRule : IRule<IProjectItem>
  {
  }
}