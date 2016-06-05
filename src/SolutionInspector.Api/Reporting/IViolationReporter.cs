using System;
using System.Collections.Generic;
using System.IO;
using SolutionInspector.Api.Rules;

namespace SolutionInspector.Api.Reporting
{
  internal interface IViolationReporter : IDisposable
  {
    void Report (IEnumerable<IRuleViolation> violations);
  }

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

    public void Report (IEnumerable<IRuleViolation> violations)
    {
      Report(_writer, violations);
    }

    protected abstract void Report (TextWriter writer, IEnumerable<IRuleViolation> violations);
  }
}