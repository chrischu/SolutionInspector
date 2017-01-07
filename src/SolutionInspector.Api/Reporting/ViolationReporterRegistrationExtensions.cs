using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Autofac;

namespace SolutionInspector.Api.Reporting
{
  /// <summary>
  ///   Provides extension methods that makes registering <see cref="IViolationReporter" />s with the Autofac container easier.
  /// </summary>
  [ExcludeFromCodeCoverage]
  public static class ViolationReporterRegistrationExtensions
  {
    /// <summary>
    ///   Registers a <see cref="IViolationReporter" /> for the <paramref name="reportFormat" /> with the Autofac container.
    /// </summary>
    public static void RegisterViolationReporter (
      this ContainerBuilder builder,
      ViolationReportFormat reportFormat,
      Func<IComponentContext, Func<TextWriter, IViolationReporter>> reporterFactory)
    {
      builder.Register(
        ctx =>
        {
          var context = ctx.Resolve<IComponentContext>();
          return reporterFactory(context);
        }).Keyed<Func<TextWriter, IViolationReporter>>(reportFormat);
    }
  }
}