using System;
using System.Collections.Generic;
using SolutionInspector.Api.Rules;

namespace SolutionInspector.Api.Reporting
{
  internal interface IViolationReporter
  {
    void Report (IEnumerable<IRuleViolation> violations);
  }
}