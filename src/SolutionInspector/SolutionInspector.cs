using System;
using System.Collections.Generic;
using SystemInterface.IO;
using SystemWrapper.IO;
using Autofac;
using JetBrains.Annotations;
using ManyConsole;
using SolutionInspector.Commands;
using SolutionInspector.Configuration;
using SolutionInspector.Reporting;
using SolutionInspector.Rules;
using SolutionInspector.Utilities;

namespace SolutionInspector
{
  /// <summary>
  /// Entry point for a SolutionInspector run.
  /// </summary>
  [PublicAPI]
  public static class SolutionInspector
  {
    /// <summary>
    /// Runs the SolutionInspector with the given console <paramref name="args"/>.
    /// </summary>
    public static int Run(string[] args)
    {
      ////var solution =
      ////    Solution.Load(@"C:\Users\Chris\Documents\Visual Studio 2015\Projects\SolutionInspector.TestSolution\SolutionInspector.TestSolution.sln");

      //var solution =
      //    Solution.Load(@"D:\Development\SolutionInspector\SolutionInspector.sln");

      //// "D:\Development\SolutionInspector\SolutionInspector.sln"

      //var project = solution.Projects.Single(p => p.Name == "SolutionInspector");

      using (var container = SetupContainer())
      {
        var commands = container.Resolve<IEnumerable<ConsoleCommand>>();
        return ConsoleCommandDispatcher.DispatchCommand(commands, args, Console.Out);
      }
    }

    private static IContainer SetupContainer()
    {
      var builder = new ContainerBuilder();

      var configuration = SolutionInspectorConfiguration.Load();
      builder.Register(ctx => configuration).As<ISolutionInspectorConfiguration>();

      builder.RegisterType<FileWrap>().As<IFile>();

      builder.RegisterType<SolutionLoader>().As<ISolutionLoader>();

      builder.RegisterType<RuleCollectionBuilder>().As<IRuleCollectionBuilder>();
      builder.RegisterType<RuleTypeResolver>().As<IRuleTypeResolver>();

      builder.RegisterType<RuleConfigurationInstantiator>().As<IRuleConfigurationInstantiator>();

      builder.RegisterType<RuleViolationViewModelConverter>().As<IRuleViolationViewModelConverter>();

      builder.Register(
          ctx =>
              new ConsoleTableWriter(
                  Console.Out,
                  new ConsoleTableWriterOptions { PreferredTableWidth = 200, Characters = ConsoleTableWriterCharacters.AdvancedAscii })
          ).As<IConsoleTableWriter>();

      builder.Register(
          ctx => new ViolationReporterProxy(
              new Dictionary<ViolationReportFormat, IViolationReporter>
              {
                  {
                      ViolationReportFormat.Table,
                      new TableViolationReporter(ctx.Resolve<IRuleViolationViewModelConverter>(), ctx.Resolve<IConsoleTableWriter>())
                  },
                  { ViolationReportFormat.Xml, new XmlViolationReporter(Console.Out, ctx.Resolve<IRuleViolationViewModelConverter>()) },
                  { ViolationReportFormat.VisualStudio, new VisualStudioViolationReporter() }
              }
              )).As<IViolationReporterProxy>();

      builder.RegisterType<CheckCommand>().As<ConsoleCommand>();

      return builder.Build();
    }
  }
}