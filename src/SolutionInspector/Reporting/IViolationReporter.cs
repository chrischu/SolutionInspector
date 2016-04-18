using System.Collections.Generic;
using SolutionInspector.Rules;

namespace SolutionInspector.Reporting
{
  internal interface IViolationReporter
  {
    void Report(IEnumerable<IRuleViolation> violations);
  }
}