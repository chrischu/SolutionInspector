using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using FluentAssertions;
using JetBrains.Annotations;
using ManyConsole;
using NLog;
using NLog.Config;
using NLog.Targets;
using NUnit.Framework;
using SolutionInspector.Commons.Console;

namespace SolutionInspector.TestInfrastructure.Console.Commands
{
  public class CommandTestsBase
  {
    private StringWriter _consoleOutput;

    private LoggingConfiguration _previousLoggingConfiguration;
    private RecordingTarget _logTarget;

    [SetUp]
    public void SetUp ()
    {
      _previousLoggingConfiguration = LogManager.Configuration;

      var configuration = new LoggingConfiguration();
      _logTarget = new RecordingTarget { Name = "test" };
      configuration.AddTarget(_logTarget);
      configuration.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, _logTarget));
      LogManager.Configuration = configuration;

      _consoleOutput = new StringWriter();
    }

    [TearDown]
    public void TearDown ()
    {
      LogManager.Configuration = _previousLoggingConfiguration;
    }

    protected int RunCommand (ConsoleCommand command, params string[] arguments)
    {
      return ConsoleCommandDispatcher.DispatchCommand(command, arguments, _consoleOutput);
    }

    protected void AssertLog(LogLevel level, string message, Exception exception = null)
    {
      Trace.Assert(!message.EndsWith("."), "Message must not end with a period.");

      LogManager.Flush();
      _logTarget.LogEvents.Should().Contain(new RecordedLogEvent(level, message, exception));
    }

    protected void AssertErrorLog (string message, Exception exception = null)
    {
      AssertLog(LogLevel.Error, message, exception);
    }

    protected void AssertCommandAbortion (int result)
    {
      AssertLog(LogLevel.Info, "Command was aborted");
      result.Should().Be(ConsoleConstants.SuccessExitCode);
    }

    private class RecordingTarget : Target
    {
      private readonly List<RecordedLogEvent> _logEvents = new List<RecordedLogEvent>();

      protected override void Write ([NotNull] LogEventInfo logEvent)
      {
        _logEvents.Add(new RecordedLogEvent(logEvent.Level, logEvent.Message, logEvent.Exception));
      }

      public IReadOnlyList<RecordedLogEvent> LogEvents => _logEvents;
    }

    [ExcludeFromCodeCoverage]
    private class RecordedLogEvent
    {
      private readonly LogLevel _level;
      private readonly string _message;
      private readonly Exception _exception;

      public RecordedLogEvent (LogLevel level, string message, [CanBeNull] Exception exception)
      {
        _level = level;
        _message = message;
        _exception = exception;
      }

      private bool Equals (RecordedLogEvent other)
      {
        return _level.Equals(other._level) && string.Equals(_message, other._message) && Equals(_exception, other._exception);
      }

      public override bool Equals ([CanBeNull] object obj)
      {
        if (obj is null)
          return false;
        if (this == obj)
          return true;

        return obj.GetType() == GetType() && Equals((RecordedLogEvent) obj);
      }

      public override int GetHashCode ()
      {
        unchecked
        {
          var hashCode = _level.GetHashCode();
          hashCode = (hashCode * 397) ^ _message.GetHashCode();
          hashCode = (hashCode * 397) ^ (_exception != null ? _exception.GetHashCode() : 0);
          return hashCode;
        }
      }

      public override string ToString ()
      {
        return _exception == null
            ? $"{_level.ToString().ToUpper()}: Message='{_message}'"
            : $"{_level.ToString().ToUpper()}: Message='{_message}' Exception=' {_exception.GetType().Name}'";
      }
    }
  }
}