using System;
using SolutionInspector.Api.ObjectModel;

namespace SolutionInspector.Api.Rules
{
  /// <summary>
  ///   A SolutionInspector rule that targets <see cref="ISolution" />s.
  /// </summary>
  public interface ISolutionRule : IRule<ISolution>
  {
  }
}