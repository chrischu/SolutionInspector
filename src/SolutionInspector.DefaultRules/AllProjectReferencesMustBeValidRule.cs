using System;
using System.Collections.Generic;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;

namespace SolutionInspector.DefaultRules
{
  /// <summary>
  /// Verifies that all project references point to an existing .csproj file that is also part of the solution.
  /// </summary>
  public class AllProjectReferencesMustBeValidRule : ProjectRule
  {
    public override IEnumerable<IRuleViolation> Evaluate (IProject target)
    {
      throw new NotImplementedException();
    }
  }
}