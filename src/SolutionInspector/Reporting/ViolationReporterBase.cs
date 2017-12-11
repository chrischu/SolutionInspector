using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using SolutionInspector.Api.Reporting;
using SolutionInspector.Api.Rules;

namespace SolutionInspector.Reporting
{
  internal abstract class ViolationReporterBase : IViolationReporter
  {
    private readonly TextWriter _writer;

    protected ViolationReporterBase (TextWriter writer)
    {
      _writer = writer;
    }

    public void Dispose ()
    {
      _writer.Dispose();
    }

    public void Report ([NotNull] IEnumerable<IRuleViolation> violations)
    {
      Report(_writer, violations);
    }

    protected abstract void Report (TextWriter writer, IEnumerable<IRuleViolation> violations);
  }
}