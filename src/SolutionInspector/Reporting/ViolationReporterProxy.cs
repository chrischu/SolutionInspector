using System.Collections.Generic;
using SolutionInspector.Rules;

namespace SolutionInspector.Reporting
{
  internal interface IViolationReporterProxy
  {
    void Report(ViolationReportFormat format, IEnumerable<IRuleViolation> violations);
  }

  internal class ViolationReporterProxy : IViolationReporterProxy
  {
    private readonly Dictionary<ViolationReportFormat, IViolationReporter> _violationReporters;

    public ViolationReporterProxy(Dictionary<ViolationReportFormat, IViolationReporter> violationReporters)
    {
      _violationReporters = violationReporters;
    }

    public void Report(ViolationReportFormat format, IEnumerable<IRuleViolation> violations)
    {
      _violationReporters[format].Report(violations);
    }
  }
}