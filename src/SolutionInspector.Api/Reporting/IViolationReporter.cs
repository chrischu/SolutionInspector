using System;
using System.Collections.Generic;
using SolutionInspector.Api.Rules;

namespace SolutionInspector.Api.Reporting
{
  /// <summary>
  ///   Reports violations in a configured <see cref="ViolationReportFormat" />.
  /// </summary>
  public interface IViolationReporter : IDisposable
  {
    /// <summary>
    ///   Report all given <paramref name="violations" />.
    /// </summary>
    void Report (IEnumerable<IRuleViolation> violations);
  }
}