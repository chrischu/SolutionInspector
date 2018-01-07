using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Autofac;
using ManyConsole;
using NLog;
using NLog.Conditions;
using NLog.Config;
using NLog.Filters;
using NLog.Layouts;
using NLog.Targets;
using SolutionInspector.Commons.Extensions;

namespace SolutionInspector.Commons.Console
{
  /// <summary>
  ///   Base class for console programs.
  /// </summary>
  [ExcludeFromCodeCoverage]
  public abstract class ConsoleProgramBase
  {
    private readonly Logger _logger;

    protected ConsoleProgramBase ()
    {
      SetupLogging();
      _logger = LogManager.GetCurrentClassLogger();
    }

    protected int Run (string[] args)
    {
      _logger.Debug($"{Process.GetCurrentProcess().ProcessName} was started with the following arguments: [{string.Join(", ", args)}].");

      using (var container = SetupContainer())
      {
        return RunInternal(container, args);
      }
    }

    protected virtual int RunInternal(IContainer container, string[] args)
    {
      var commands = container.Resolve<IEnumerable<ConsoleCommand>>();
      return ConsoleCommandDispatcher.DispatchCommand(commands, args, System.Console.Out);
    }

    protected abstract void RegisterServices (ContainerBuilder builder);

    protected virtual void RegisterCommands (ContainerBuilder builder)
    {
      var consoleCommands = Assembly.GetEntryAssembly().GetTypes().Where(t => typeof(ConsoleCommand).IsAssignableFrom(t)).ToArray();
      builder.RegisterTypes(consoleCommands).As<ConsoleCommand>();
    }

    private IContainer SetupContainer ()
    {
      var builder = new ContainerBuilder();

      builder.RegisterType<ConsoleHelper>().As<IConsoleHelper>();

      RegisterServices(builder);
      RegisterCommands(builder);

      return builder.Build();
    }

    private void SetupLogging ()
    {
      var configuration = new LoggingConfiguration();

      ConfigureFileLogging(configuration);
      ConfigureConsoleLogging(configuration);

      LogManager.Configuration = configuration;
    }

    private void ConfigureFileLogging (LoggingConfiguration configuration)
    {
      var target = new FileTarget("file")
                   {
                       FileName = "${basedir:file=${processname}.log}",
                       Layout = new SimpleLayout("${time}|${uppercase:${level}}|${logger}: ${message} ${exception:format=ToString}")
                   };
      configuration.AddTarget(target);

      var rule = new LoggingRule("*", LogLevel.Debug, target);
      configuration.LoggingRules.Add(rule);
    }

    private void ConfigureConsoleLogging (LoggingConfiguration configuration)
    {
      ConsoleRowHighlightingRule RowHighlightingRule (
          LogLevel level,
          ConsoleOutputColor foregroundColor,
          ConsoleOutputColor backgroundColor = ConsoleOutputColor.NoChange)
      {
        return new ConsoleRowHighlightingRule(ConditionParser.ParseExpression($"level == LogLevel.{level}"), foregroundColor, backgroundColor);
      }

      var rowHighlightingRules = new[]
                                 {
                                     RowHighlightingRule(LogLevel.Fatal, ConsoleOutputColor.White, ConsoleOutputColor.Red),
                                     RowHighlightingRule(LogLevel.Error, ConsoleOutputColor.Red),
                                     RowHighlightingRule(LogLevel.Warn, ConsoleOutputColor.Yellow),
                                     RowHighlightingRule(LogLevel.Info, ConsoleOutputColor.White),
                                     RowHighlightingRule(LogLevel.Debug, ConsoleOutputColor.Gray),
                                     RowHighlightingRule(LogLevel.Trace, ConsoleOutputColor.DarkGray)
                                 };

      ConfigureConsoleLogging(configuration, rowHighlightingRules);
      ConfigureConsoleLoggingWithException(configuration, rowHighlightingRules);
    }

    private void ConfigureConsoleLogging (LoggingConfiguration configuration, ConsoleRowHighlightingRule[] rowHighlightingRules)
    {
      var target = new ColoredConsoleTarget("console") { Layout = "${message}" };
      target.RowHighlightingRules.AddRange(rowHighlightingRules);
      configuration.AddTarget(target);

      var rule = new LoggingRule("*", LogLevel.Info, target);
      rule.Filters.Add(
          new ConditionBasedFilter { Condition = ConditionParser.ParseExpression("not equals('${exception}', '')"), Action = FilterResult.Ignore });
      configuration.LoggingRules.Add(rule);
    }

    private void ConfigureConsoleLoggingWithException (LoggingConfiguration configuration, ConsoleRowHighlightingRule[] rowHighlightingRules)
    {
      var target =
          new ColoredConsoleTarget("consoleWithException") { Layout = "${message} (for further details see ${processname}.log): ${exception:format=Message}" };
      target.RowHighlightingRules.AddRange(rowHighlightingRules);
      configuration.AddTarget(target);

      var rule = new LoggingRule("*", LogLevel.Info, target);
      rule.Filters.Add(
          new ConditionBasedFilter { Condition = ConditionParser.ParseExpression("equals('${exception}', '')"), Action = FilterResult.Ignore });
      configuration.LoggingRules.Add(rule);
    }
  }
}