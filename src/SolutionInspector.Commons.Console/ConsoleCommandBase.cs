using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using ManyConsole;
using NLog;

namespace SolutionInspector.Commons.Console
{
  [PublicAPI]
  // ReSharper disable once MissingXmlDoc
  public interface IArgumentsBuilderWithSetValues<out TArguments>
  {
    IArgumentsBuilderWithSetValues<TArguments> Option<T> (
        string longKey,
        string shortKey,
        string description,
        Action<TArguments, T> setValue,
        T defaultValue = default(T));

    IArgumentsBuilderWithSetValues<TArguments> Flag (string longKey, string shortKey, string description, Action<TArguments, bool> setValue);
  }

  [PublicAPI]
  // ReSharper disable once MissingXmlDoc
  public interface IArgumentsBuilder<out TArguments>
  {
    IArgumentsBuilder<TArguments> Option<T> (
        string longKey,
        string shortKey,
        string description,
        Action<TArguments, T> setValue,
        T defaultValue = default(T));

    IArgumentsBuilder<TArguments> Flag (string longKey, string shortKey, string description, Action<TArguments, bool> setValue);

    IArgumentsBuilderWithSetValues<TArguments> Values (Action<IValueArgumentsBuilder<TArguments>> configureValueArguments);
  }

  [PublicAPI]
  // ReSharper disable once MissingXmlDoc
  public interface IValueArgumentsBuilder<out TArguments>
  {
    IValueArgumentsBuilder<TArguments> Value (string name, Action<TArguments, string> setValue);
  }

  /// <summary>
  ///   Base class for console commands.
  /// </summary>
  public abstract class ConsoleCommandBase<TRawArguments, TParsedArguments> : ConsoleCommand
      where TRawArguments : new()
  {
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly ArgumentsBuilder<TRawArguments> _rawArgumentsBuilder;
    private TParsedArguments _parsedArguments;

    [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    protected ConsoleCommandBase (string command, string description)
    {
      IsCommand(command, description);
      SkipsCommandSummaryBeforeRunning();
      _rawArgumentsBuilder = new ArgumentsBuilder<TRawArguments>(this);
      // ReSharper disable once VirtualMemberCallInContructor
      SetupArguments(_rawArgumentsBuilder);
    }

    protected abstract void SetupArguments (IArgumentsBuilder<TRawArguments> argumentsBuilder);

    public sealed override int? OverrideAfterHandlingArgumentsBeforeRun ([NotNull] string[] remainingArguments)
    {
      _rawArgumentsBuilder.HandleRemainingArguments(remainingArguments);

      var rawArguments = GetRawArguments();
      _parsedArguments = ValidateAndParseArguments(rawArguments);

      return base.OverrideAfterHandlingArgumentsBeforeRun(remainingArguments);
    }

    private TRawArguments GetRawArguments ()
    {
      try
      {
        return _rawArgumentsBuilder.Build();
      }
      catch (ArgumentParsingException ex)
      {
        throw ReportArgumentValidationError(ex.Message);
      }
    }

    protected abstract TParsedArguments ValidateAndParseArguments (TRawArguments arguments);

    public sealed override int Run ([NotNull] string[] remainingArguments)
    {
      return Run(_parsedArguments);
    }

    protected abstract int Run (TParsedArguments arguments);

    [MustUseReturnValue("Returned exception must be thrown")]
    protected Exception ReportArgumentValidationError (string message, Exception ex = null)
    {
      Trace.Assert(!message.EndsWith("."), "Message must not end with a '.'");

      _logger.Error(ex, message);

      return new ConsoleHelpAsException("");
    }

    [MustUseReturnValue("Returned value must be used as exit code")]
    protected int ReportExecutionError (string message, Exception ex)
    {
      _logger.Error(ex, message);
      return ConsoleConstants.ErrorExitCode;
    }

    [MustUseReturnValue("Returned value must be used as exit code")]
    protected int ReportAbortion ()
    {
      _logger.Info("Command was aborted");
      return ConsoleConstants.SuccessExitCode;
    }

    protected void LogInfo (string message)
    {
      _logger.Log(LogLevel.Info, message);
    }

    protected void LogDebug (string message)
    {
      _logger.Log(LogLevel.Debug, message);
    }

    protected void LogError(string message)
    {
      _logger.Log(LogLevel.Error, message);
    }

    protected void LogWarning(string message)
    {
      _logger.Log(LogLevel.Warn, message);
    }

    private class ArgumentsBuilder<TArguments> : IArgumentsBuilder<TArguments>, IArgumentsBuilderWithSetValues<TArguments>
        where TArguments : new()
    {
      private readonly TArguments _arguments;
      private readonly ConsoleCommand _command;
      private readonly ValueArgumentsBuilder _valueArgumentsBuilder = new ValueArgumentsBuilder();
      private readonly List<UnparsedArgument> _unparsedArguments = new List<UnparsedArgument>();

      public ArgumentsBuilder (ConsoleCommand command)
      {
        _arguments = new TArguments();
        _command = command;
      }

      public TArguments Build ()
      {
        foreach (var unparsedArgument in _unparsedArguments)
          unparsedArgument.Parse(_arguments);

        return _arguments;
      }

      public IArgumentsBuilder<TArguments> Option<T> (
          string longKey,
          string shortKey,
          string description,
          Action<TArguments, T> setValue,
          T defaultValue = default(T))
      {
        setValue(_arguments, defaultValue);

        var unparsedArgument = new UnparsedArgument(longKey, typeof(T), (args, value) => setValue(args, (T) value));
        _unparsedArguments.Add(unparsedArgument);
        _command.HasOption<string>($"{shortKey}|{longKey}=", description, v => unparsedArgument.SetUnparsedArgumentValue(v));
        return this;
      }

      public IArgumentsBuilder<TArguments> Flag (string longKey, string shortKey, string description, Action<TArguments, bool> setValue)
      {
        _command.HasOption($"{shortKey}|{longKey}", description, v => setValue(_arguments, v != null));
        return this;
      }

      public IArgumentsBuilderWithSetValues<TArguments> Values (Action<IValueArgumentsBuilder<TArguments>> configureValueArguments)
      {
        configureValueArguments(_valueArgumentsBuilder);
        _valueArgumentsBuilder.SetupAdditionalArguments(_command);
        return this;
      }

      [ExcludeFromCodeCoverage]
      IArgumentsBuilderWithSetValues<TArguments> IArgumentsBuilderWithSetValues<TArguments>.Option<T> (
          string longKey,
          string shortKey,
          string description,
          Action<TArguments, T> setValue,
          [CanBeNull] T defaultValue)
      {
        return (IArgumentsBuilderWithSetValues<TArguments>) Option(longKey, shortKey, description, setValue);
      }

      [ExcludeFromCodeCoverage]
      IArgumentsBuilderWithSetValues<TArguments> IArgumentsBuilderWithSetValues<TArguments>.Flag (
          string longKey,
          string shortKey,
          string description,
          Action<TArguments, bool> setValue)
      {
        return (IArgumentsBuilderWithSetValues<TArguments>) Flag(longKey, shortKey, description, setValue);
      }

      public void HandleRemainingArguments (string[] remainingArguments)
      {
        _valueArgumentsBuilder.ParseAdditionalArguments(_arguments, remainingArguments);
      }

      private class UnparsedArgument
      {
        private string _unparsedArgument;
        private readonly Type _argumentType;
        private readonly string _argumentLongKey;
        private readonly Action<TArguments, object> _setParsedValue;

        public UnparsedArgument (string argumentLongKey, Type parsedType, Action<TArguments, object> setParsedValue)
        {
          _argumentType = Nullable.GetUnderlyingType(parsedType) ?? parsedType;
          _argumentLongKey = argumentLongKey;
          _setParsedValue = setParsedValue;
        }

        public void SetUnparsedArgumentValue (string unparsedArgument)
        {
          _unparsedArgument = unparsedArgument;
        }

        public void Parse (TArguments arguments)
        {
          if (_unparsedArgument == null)
            return;

          try
          {
            var parsedArgument = TypeDescriptor.GetConverter(_argumentType).ConvertFromString(_unparsedArgument);
            _setParsedValue(arguments, parsedArgument);
          }
          catch (Exception ex)
          {
            throw new ArgumentParsingException($"Invalid value for argument '{_argumentLongKey}': '{_unparsedArgument}'", ex);
          }
        }
      }

      private class ValueArgumentsBuilder : IValueArgumentsBuilder<TArguments>
      {
        private readonly List<ValueArgument> _valueArguments = new List<ValueArgument>();

        private string AdditionalArgumentsString => string.Join(" ", _valueArguments.Select(a => $"<{a.Name}>"));

        public IValueArgumentsBuilder<TArguments> Value (string name, Action<TArguments, string> setValue)
        {
          _valueArguments.Add(new ValueArgument(name, setValue));
          return this;
        }

        public void SetupAdditionalArguments (ConsoleCommand command)
        {
          command.HasAdditionalArguments(_valueArguments.Count, AdditionalArgumentsString);
        }

        public void ParseAdditionalArguments (TArguments arguments, string[] remainingArguments)
        {
          Trace.Assert(remainingArguments.Length == _valueArguments.Count);

          for (var i = 0; i < remainingArguments.Length; i++)
            _valueArguments[i].SetValueAction(arguments, remainingArguments[i]);
        }

        private class ValueArgument
        {
          public ValueArgument (string name, Action<TArguments, string> setValueAction)
          {
            Name = name;
            SetValueAction = setValueAction;
          }

          public string Name { get; }
          public Action<TArguments, string> SetValueAction { get; }
        }
      }
    }

    [Serializable]
    private class ArgumentParsingException : Exception
    {
      public ArgumentParsingException (string message, Exception innerException)
          : base(message, innerException)
      {
      }

      [ExcludeFromCodeCoverage]
      protected ArgumentParsingException (SerializationInfo info, StreamingContext context)
          : base(info, context)
      {
      }
    }
  }
}