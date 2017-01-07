using System.Collections.Generic;
using System.IO;
using SolutionInspector.Api.Rules;
using SolutionInspector.Commons.Extensions;

namespace SolutionInspector.Reporting
{
  internal class VisualStudioViolationReporter : ViolationReporterBase
  {
    public VisualStudioViolationReporter (TextWriter writer)
      : base(writer)
    {
    }

    protected override void Report (TextWriter writer, IEnumerable<IRuleViolation> violations)
    {
      foreach (var violation in violations)
        writer.WriteLine(
          $"{violation.Target.FullPath}: SolutionInspector warning SI0000: " +
          $"{violation.Message} ({violation.Rule.GetType().Name.RemoveSuffix("Rule")})");
    }
  }
}