using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Autofac;
using JetBrains.Annotations;
using ManyConsole;
using NLog;
using SolutionInspector.Api.Configuration;
using SolutionInspector.Api.Reporting;
using SolutionInspector.Commands;
using SolutionInspector.Configuration;
using SolutionInspector.Internals;
using SolutionInspector.Reporting;
using SolutionInspector.Rules;
using SolutionInspector.Utilities;
using Wrapperator.Interfaces;
using Wrapperator.Interfaces.Configuration;
using Wrapperator.Interfaces.Diagnostics;
using Wrapperator.Interfaces.IO;
using Wrapperator.Interfaces.Reflection;
using Wrapperator.Wrappers;

namespace SolutionInspector
{
  /// <summary>
  ///   Entry point for a SolutionInspector run.
  /// </summary>
  [PublicAPI]
  [ExcludeFromCodeCoverage]
  public static class SolutionInspector
  {
    /// <summary>
    ///   File extension for SolutionInspector ruleset files.
    /// </summary>
    public const string RulesetFileExtension = "SolutionInspectorRuleset";

    private static Logger s_logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    ///   Runs the SolutionInspector with the given console <paramref name="args" />.
    /// </summary>
    public static int Run (string[] args)
    {
      s_logger.Debug($"SolutionInspector was run with the following arguments: [{string.Join(", ", args)}].");

      using (var container = SetupContainer())
      {
        var commands = container.Resolve<IEnumerable<ConsoleCommand>>();
        return ConsoleCommandDispatcher.DispatchCommand(commands, args, Console.Out);
      }
    }

    private static IContainer SetupContainer ()
    {
      var builder = new ContainerBuilder();

      builder.Register(ctx => Wrapper.Console).As<IConsoleStatic>();
      builder.Register(ctx => Wrapper.File).As<IFileStatic>();
      builder.Register(ctx => Wrapper.Directory).As<IDirectoryStatic>();
      builder.Register(ctx => Wrapper.Assembly).As<IAssemblyStatic>();
      builder.Register(ctx => Wrapper.ConfigurationManager).As<IConfigurationManagerStatic>();
      builder.Register(ctx => Wrapper.Process).As<IProcessStatic>();

      builder.RegisterType<ConfigurationManager>().As<IConfigurationManager>();

      builder.RegisterType<SolutionLoader>().As<ISolutionLoader>();

      builder.RegisterType<RuleAssemblyLoader>().As<IRuleAssemblyLoader>();

      builder.RegisterType<RuleCollectionBuilder>().As<IRuleCollectionBuilder>();
      builder.RegisterType<RuleTypeResolver>().As<IRuleTypeResolver>();

      builder.RegisterType<RuleConfigurationInstantiator>().As<IRuleConfigurationInstantiator>();

      builder.RegisterType<RuleViolationViewModelConverter>().As<IRuleViolationViewModelConverter>();

      builder.Register(
          ctx =>
              new TableWriter(new TableWriterOptions { PreferredTableWidth = 200, Characters = TableWriterCharacters.AdvancedAscii })
          ).As<ITableWriter>();

      builder.RegisterViolationReporter(
          ViolationReportFormat.Table,
          ctx => tw => new TableViolationReporter(tw, ctx.Resolve<IRuleViolationViewModelConverter>(), ctx.Resolve<ITableWriter>()));

      builder.RegisterViolationReporter(
          ViolationReportFormat.Xml,
          ctx => tw => new XmlViolationReporter(tw, ctx.Resolve<IRuleViolationViewModelConverter>()));

      builder.RegisterViolationReporter(
          ViolationReportFormat.VisualStudio,
          ctx => tw => new VisualStudioViolationReporter(tw));

      builder.RegisterType<ViolationReporterFactory>().As<IViolationReporterFactory>();

      builder.RegisterType<InspectCommand>().As<ConsoleCommand>();
      builder.Register(
          ctx => new InitializeCommand(
              Wrapper.Wrap(Assembly.GetAssembly(typeof(DefaultRules.SolutionBuildConfigurationsRule))),
              ctx.Resolve<IFileStatic>(),
              ctx.Resolve<IConsoleStatic>())
          ).As<ConsoleCommand>();
      builder.RegisterType<ConfigureCommand>().As<ConsoleCommand>();

      builder.RegisterType<ConfigurationLoader>().As<IConfigurationLoader>();

      builder.Register(ctx => (ISolutionInspectorConfiguration) System.Configuration.ConfigurationManager.GetSection("solutionInspector"))
          .As<ISolutionInspectorConfiguration>()
          .SingleInstance();

      return builder.Build();
    }
  }
}