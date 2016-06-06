using System;
using System.Collections.Generic;
using System.IO;
using SolutionInspector.Api.Extensions;
using SolutionInspector.Api.Rules;

namespace SolutionInspector.Api.Reporting
{
  internal class VisualStudioViolationReporter : ViolationReporterBase
  {
    public VisualStudioViolationReporter (TextWriter writer) : base(writer)
    {
    }

    protected override void Report (TextWriter writer, IEnumerable<IRuleViolation> violations)
    {
      foreach (var violation in violations)
        writer.WriteLine($"{violation.Target.FullPath}: SolutionInspector warning SI0000: {violation.Message} ({violation.Rule.GetType().Name.RemoveSuffix("Rule")})");
    }
  }
}