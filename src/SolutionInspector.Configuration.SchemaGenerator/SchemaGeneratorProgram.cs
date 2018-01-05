using System;
using System.Collections.Generic;
using Autofac;
using ManyConsole;
using Wrapperator.Interfaces.Reflection;
using Wrapperator.Wrappers;

namespace SolutionInspector.SchemaGenerator
{
  internal static class SchemaGeneratorProgram
  {
    public static int Main (string[] args)
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

      builder.RegisterType<GenerateCommand>().As<ConsoleCommand>();

      return builder.Build();
    }
  }
}