using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using SystemInterface.IO;
using SystemInterface.Reflection;
using SystemWrapper.IO;
using SystemWrapper.Reflection;
using Autofac;
using JetBrains.Annotations;
using ManyConsole;
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
    /// <summary>
    ///   Runs the SolutionInspector with the given console <paramref name="args" />.
    /// </summary>
    public static int Run (string[] args)
    {
      CheckMsBuildToolsInstallation();

      using (var container = SetupContainer())
      {
        var ruleAssemblyLoader = container.Resolve<IRuleAssemblyLoader>();
        var solutionInspectorConfiguration = container.Resolve<ISolutionInspectorConfiguration>();
        ruleAssemblyLoader.LoadRuleAssemblies(solutionInspectorConfiguration.RuleAssemblyImports.Imports);

        var commands = container.Resolve<IEnumerable<ConsoleCommand>>();
        return ConsoleCommandDispatcher.DispatchCommand(commands, args, Console.Out);
      }
    }

    private static void CheckMsBuildToolsInstallation ()
    {
      try
      {
        Assembly.Load("Microsoft.Build, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
      }
      catch (FileNotFoundException)
      {
        Console.Error.WriteLine(
            "Could not find MSBuild assemblies in version 14.0 this most likely means that 'MSBuild Tools 2015' was not installed.");
        Console.Error.WriteLine("Just press any key to open a browser with the download page of the 'MSBuild Tools 2015'...");
        Console.ReadKey();
        Process.Start("https://www.microsoft.com/en-us/download/details.aspx?id=48159");
        Environment.Exit(1);
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

      builder.RegisterType<InspectCommand>().As<ConsoleCommand>();

      return builder.Build();
    }
  }
}