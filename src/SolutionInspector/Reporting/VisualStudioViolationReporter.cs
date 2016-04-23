using System;
using System.Collections.Generic;
using SolutionInspector.Rules;

namespace SolutionInspector.Reporting
{
  internal class VisualStudioViolationReporter : IViolationReporter
  {
    public void Report(IEnumerable<IRuleViolation> violations)
    {
      foreach(var violation in violations)
      {
        Console.Error.WriteLine($"{violation.Target.FullPath}: SolutionInspector warning SI0000: {violation.Message} ({violation.Rule})");
      }
    }
  }
}