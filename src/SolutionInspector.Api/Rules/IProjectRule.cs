using System;
using SolutionInspector.Api.ObjectModel;

namespace SolutionInspector.Api.Rules
{
  /// <summary>
  ///   A SolutionInspector rule that targets <see cref="IProject" />s.
  /// </summary>
  public interface IProjectRule : IRule<IProject>
  {
  }
}