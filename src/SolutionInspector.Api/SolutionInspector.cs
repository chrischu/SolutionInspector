using System;
using System.Collections.Generic;
using System.IO;
using SystemInterface.IO;
using SystemInterface.Reflection;
using SystemWrapper.IO;
using SystemWrapper.Reflection;
using Autofac;
using JetBrains.Annotations;
using ManyConsole;
using NLog;
using SolutionInspector.Api.Commands;
using SolutionInspector.Api.Configuration;
using SolutionInspector.Api.Reporting;
using SolutionInspector.Api.Rules;
using SolutionInspector.Api.Utilities;

namespace SolutionInspector.Api
{
  /// <summary>
  ///   Entry point for a SolutionInspector run.
  /// </summary>
  [PublicAPI]
  public static class SolutionInspector
  {
    private static Logger s_logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    ///   Runs the SolutionInspector with the given console <paramref name="args" />.
    /// </summary>
    public static int Run (string[] args)
    {
      s_logger.Debug($"SolutionInspector was run with the following arguments: [{string.Join(", ", args)}].");

      using (var container = SetupContainer())
      {
        var ruleAssemblyLoader = container.Resolve<IRuleAssemblyLoader>();
        var solutionInspectorConfiguration = container.Resolve<ISolutionInspectorConfiguration>();
        ruleAssemblyLoader.LoadRuleAssemblies(solutionInspectorConfiguration.RuleAssemblyImports.Imports);

        var commands = container.Resolve<IEnumerable<ConsoleCommand>>();
        return ConsoleCommandDispatcher.DispatchCommand(commands, args, Console.Out);
      }
    }

   private static IContainer SetupContainer ()
    {
      var builder = new ContainerBuilder();

      var configuration = SolutionInspectorConfiguration.Load();
      builder.Register(ctx => configuration).As<ISolutionInspectorConfiguration>();

      builder.RegisterType<FileWrap>().As<IFile>();
      builder.RegisterType<DirectoryWrap>().As<IDirectory>();
      builder.RegisterType<AssemblyWrap>().As<IAssembly>();

      builder.RegisterType<SolutionLoader>().As<ISolutionLoader>();

      builder.RegisterType<RuleAssemblyLoader>().As<IRuleAssemblyLoader>();

      builder.RegisterType<RuleCollectionBuilder>().As<IRuleCollectionBuilder>();
      builder.RegisterType<RuleTypeResolver>().As<IRuleTypeResolver>();

      builder.RegisterType<RuleConfigurationInstantiator>().As<IRuleConfigurationInstantiator>();

      builder.RegisterType<RuleViolationViewModelConverter>().As<IRuleViolationViewModelConverter>();

      builder.Register(
          ctx =>
              new TableWriter(new TableWriterOptions { PreferredTableWidth = 200, Characters = ConsoleTableWriterCharacters.AdvancedAscii })
          ).As<ITableWriter>();

      builder.Register(
          ctx => new ViolationReporterFactory(
              new Dictionary<ViolationReportFormat, Func<TextWriter, IViolationReporter>>
              {
                  {
                      ViolationReportFormat.Table,
                      writer =>
                      new TableViolationReporter(writer, ctx.Resolve<IRuleViolationViewModelConverter>(), ctx.Resolve<ITableWriter>())
                  },
                  { ViolationReportFormat.Xml, writer => new XmlViolationReporter(writer, ctx.Resolve<IRuleViolationViewModelConverter>()) },
                  { ViolationReportFormat.VisualStudio, writer => new VisualStudioViolationReporter(writer) }
              })).As<IViolationReporterFactory>();

      builder.RegisterType<InspectCommand>().As<ConsoleCommand>();

      return builder.Build();
    }
  }
}