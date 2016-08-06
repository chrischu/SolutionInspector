using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Autofac.Features.Indexed;
using JetBrains.Annotations;
using SolutionInspector.Api.Reporting;

namespace SolutionInspector.Reporting
{
  internal interface IViolationReporterFactory
  {
    IViolationReporter CreateConsoleReporter (ViolationReportFormat reportFormat);
    IViolationReporter CreateFileReporter (ViolationReportFormat reportFormat, string filePath);
  }

  [UsedImplicitly /* by Autofac container */]
  [ExcludeFromCodeCoverage]
  internal class ViolationReporterFactory : IViolationReporterFactory
  {
    private readonly IIndex<ViolationReportFormat, Func<TextWriter, IViolationReporter>> _violationReporterFactories;

    public ViolationReporterFactory (IIndex<ViolationReportFormat, Func<TextWriter, IViolationReporter>> violationReporterFactories)
    {
      _violationReporterFactories = violationReporterFactories;
    }

    public IViolationReporter CreateConsoleReporter (ViolationReportFormat reportFormat)
    {
      var writer = Console.Out;
      return _violationReporterFactories[reportFormat](writer);
    }

    [SuppressMessage ("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
        Justification = "The StreamWriter is disposed when the IViolationReporter is disposed")]
    public IViolationReporter CreateFileReporter (ViolationReportFormat reportFormat, string filePath)
    {
      var writer = new StreamWriter(File.OpenWrite(filePath));
      return _violationReporterFactories[reportFormat](writer);
    }
  }
}