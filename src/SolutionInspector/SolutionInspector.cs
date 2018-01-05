using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Autofac;
using ManyConsole;
using NLog;
using SolutionInspector.Api.Configuration;
using SolutionInspector.Api.Reporting;
using SolutionInspector.Commands;
using SolutionInspector.Commons.Attributes;
using SolutionInspector.Commons.Extensions;
using SolutionInspector.Configuration;
using SolutionInspector.DefaultRules;
using SolutionInspector.Internals;
using SolutionInspector.Reporting;
using SolutionInspector.Rules;
using SolutionInspector.Utilities;
using Wrapperator.Interfaces;
using Wrapperator.Interfaces.Configuration;
using Wrapperator.Interfaces.Diagnostics;
using Wrapperator.Interfaces.IO;
using Wrapperator.Interfaces.Reflection;
using Wrapperator.Interfaces.Xml.Linq;
using Wrapperator.Wrappers;
using ConfigurationManager = SolutionInspector.Configuration.ConfigurationManager;

namespace SolutionInspector
{
  /// <summary>
  ///   Entry point for a SolutionInspector run.
  /// </summary>
  [PublicApi]
  [ExcludeFromCodeCoverage]
  public static class SolutionInspector
  {
    /// <summary>
    ///   File extension for SolutionInspector ruleset files.
    /// </summary>
    public const string RulesetFileExtension = "SolutionInspectorRuleset";

    private static Logger s_Logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    ///   Runs the SolutionInspector with the given console <paramref name="args" />.
    /// </summary>
    public static int Run (string[] args)
    {
      Environment.SetEnvironmentVariable("VSINSTALLDIR", @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community");
      Environment.SetEnvironmentVariable("VisualStudioVersion", "15.0");

      s_Logger.Debug($"SolutionInspector was run with the following arguments: [{args.Join(", ")}].");

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
      builder.Register(ctx => Wrapper.XDocument).As<IXDocumentStatic>();

      builder.RegisterType<ConfigurationManager>().As<IConfigurationManager>();

      builder.RegisterType<SolutionLoader>().As<ISolutionLoader>();

      builder.RegisterType<RuleAssemblyLoader>().As<IRuleAssemblyLoader>();

      builder.RegisterType<RuleCollectionBuilder>().As<IRuleCollectionBuilder>();

      builder.RegisterType<RuleInstantiator>().As<IRuleInstantiator>();

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
          Wrapper.Wrap(Assembly.GetAssembly(typeof(SolutionBuildConfigurationsRule))),
          ctx.Resolve<IFileStatic>(),
          ctx.Resolve<IConsoleStatic>())
      ).As<ConsoleCommand>();

      builder.Register(ctx => new ConfigureCommand("TODO", ctx.Resolve<IProcessStatic>())).As<ConsoleCommand>();

      builder.RegisterType<ConfigurationLoader>().As<IConfigurationLoader>();

      var configuration = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
      var solutionInspectorConfiguration = (ISolutionInspectorConfiguration) configuration.GetSectionGroup("solutionInspector").AssertNotNull();
      builder.Register(ctx => solutionInspectorConfiguration).As<ISolutionInspectorConfiguration>().SingleInstance();

      return builder.Build();
    }
  }
}