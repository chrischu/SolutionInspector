using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using ManyConsole;
using NLog;

namespace SolutionInspector.Api.Commands
{
  [PublicAPI]
  internal interface IArgumentsBuilderWithSetValues<out TArguments>
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
  internal interface IArgumentsBuilder<out TArguments>
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
  internal interface IValueArgumentsBuilder<out TArguments>
  {
    IValueArgumentsBuilder<TArguments> Value (string name, Action<TArguments, string> setValue);
  }

  internal abstract class SolutionInspectorCommand<TRawArguments, TParsedArguments> : ConsoleCommand
      where TRawArguments : new()
  {
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly TRawArguments _rawArguments;
    private readonly ArgumentsBuilder<TRawArguments> _rawArgumentsBuilder;
    private TParsedArguments _parsedArguments;

    [SuppressMessage ("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    protected SolutionInspectorCommand (string command, string description)
    {
      IsCommand(command, description);
      SkipsCommandSummaryBeforeRunning();
      _rawArguments = new TRawArguments();
      _rawArgumentsBuilder = new ArgumentsBuilder<TRawArguments>(this, _rawArguments);
      // ReSharper disable once VirtualMemberCallInContructor
      SetupArguments(_rawArgumentsBuilder);
    }

    protected abstract void SetupArguments (IArgumentsBuilder<TRawArguments> argumentsBuilder);

    public sealed override int? OverrideAfterHandlingArgumentsBeforeRun ([NotNull] string[] remainingArguments)
    {
      _rawArgumentsBuilder.HandleRemainingArguments(remainingArguments);

      if (!TryLoadingMsBuildToolsAssembly())
      {
        _logger.Error(
            "Could not find MSBuild assemblies in version 14.0. This most likely means that 'MSBuild Tools 2015' was not installed."
            + Environment.NewLine
            + "Just press the RETURN key to open a browser with the download page of the 'MSBuild Tools 2015' or press ESCAPE or any other key to cancel...");
        var key = Console.ReadKey();
        if (key.Key == ConsoleKey.Enter)
          Process.Start("https://www.microsoft.com/en-us/download/details.aspx?id=48159");

        return 1;
      }

      _parsedArguments = ValidateAndParseArguments(_rawArguments, message => new ConsoleHelpAsException(message));

      return base.OverrideAfterHandlingArgumentsBeforeRun(remainingArguments);
    }

    private bool TryLoadingMsBuildToolsAssembly ()
    {
      try
      {
        _logger.Info("Checking for 'MSBuild Tools 2015'...");
        Assembly.Load("Microsoft.Build, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
        _logger.Info("Check successful, 'MSBuild Tools 2015' are installed.");
        return true;
      }
      catch (FileNotFoundException)
      {
        return false;
      }
    }

    protected abstract TParsedArguments ValidateAndParseArguments (TRawArguments arguments, Func<string, Exception> reportError);

    public sealed override int Run ([NotNull] string[] remainingArguments)
    {
      return Run(_parsedArguments);
    }

    protected abstract int Run (TParsedArguments arguments);

    private class ArgumentsBuilder<TArguments> : IArgumentsBuilder<TArguments>, IArgumentsBuilderWithSetValues<TArguments>
        where TArguments : new()
    {
      private readonly ConsoleCommand _command;
      private readonly TArguments _arguments;
      private readonly ValueArgumentsBuilder _valueArgumentsBuilder = new ValueArgumentsBuilder();

      public ArgumentsBuilder (ConsoleCommand command, TArguments arguments)
      {
        _command = command;
        _arguments = arguments;
      }

      public IArgumentsBuilder<TArguments> Option<T> (
          string longKey,
          string shortKey,
          string description,
          Action<TArguments, T> setValue,
          T defaultValue = default(T))
      {
        setValue(_arguments, defaultValue);
        _command.HasOption<T>($"{shortKey}|{longKey}=", description, v => setValue(_arguments, v));
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
          T defaultValue)
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

      private class ValueArgumentsBuilder : IValueArgumentsBuilder<TArguments>
      {
        private readonly List<ValueArgument> _valueArguments = new List<ValueArgument>();

        private string AdditionalArgumentsString => string.Join(" ", _valueArguments.Select(a => $"<{a.Name}>"));

        public IValueArgumentsBuilder<TArguments> Value (string name, Action<TArguments, string> setValue)
        {
          _valueArguments.Add(new ValueArgument(name, setValue));
          return this;
        }

        private class ValueArgument
        {
          public string Name { get; }
          public Action<TArguments, string> SetValueAction { get; }

          public ValueArgument (string name, Action<TArguments, string> setValueAction)
          {
            Name = name;
            SetValueAction = setValueAction;
          }
        }

        public void SetupAdditionalArguments (ConsoleCommand command)
        {
          command.HasAdditionalArguments(_valueArguments.Count, AdditionalArgumentsString);
        }

        public void ParseAdditionalArguments (TArguments arguments, string[] remainingArguments)
        {
          Trace.Assert(remainingArguments.Length == _valueArguments.Count);

          for (int i = 0; i < remainingArguments.Length; i++)
            _valueArguments[i].SetValueAction(arguments, remainingArguments[i]);
        }
      }

      public void HandleRemainingArguments (string[] remainingArguments)
      {
        _valueArgumentsBuilder?.ParseAdditionalArguments(_arguments, remainingArguments);
      }
    }
  }
}