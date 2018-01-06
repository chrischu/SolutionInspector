using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Autofac;
using ManyConsole;
using SolutionInspector.BuildTool.Commands;
using Wrapperator.Interfaces;
using Wrapperator.Interfaces.IO;
using Wrapperator.Interfaces.Reflection;
using Wrapperator.Wrappers;

namespace SolutionInspector.BuildTool
{
  [ExcludeFromCodeCoverage]
  internal class BuildToolProgram
  {
    internal const string DefaultBaseSchemaVersion = "1";
    internal const string BaseSchemaNamespaceTemplate = "http://chrischu.github.io/SolutionInspector/schema/base_v{0}.xsd";

    public static int Main (string[] args)
    {
      return new BuildToolProgram().Run(args);
    }

    private int Run (string[] args)
    {
      using (var container = SetupContainer())
      {
        var commands = container.Resolve<IEnumerable<ConsoleCommand>>();
        return ConsoleCommandDispatcher.DispatchCommand(commands, args, Console.Out);
      }
    }

    private static IContainer SetupContainer ()
    {
      var builder = new ContainerBuilder();

      builder.Register(ctx => Wrapper.Assembly).As<IAssemblyStatic>();
      builder.Register(ctx => Wrapper.File).As<IFileStatic>();
      builder.Register(ctx => Wrapper.Console).As<IConsoleStatic>();

      builder.RegisterType<RuleAssemblySchemaCreator>().As<IRuleAssemblySchemaCreator>();
      builder.RegisterType<SchemaInfoRetriever>().As<ISchemaInfoRetriever>();

      builder.RegisterType<GenerateSchemaCommand>().As<ConsoleCommand>();

      return builder.Build();
    }
  }
}