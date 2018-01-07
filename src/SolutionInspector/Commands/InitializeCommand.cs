using System;
using System.IO;
using SolutionInspector.Commons.Console;
using SolutionInspector.Commons.Extensions;
using Wrapperator.Interfaces.IO;
using Wrapperator.Interfaces.Reflection;

namespace SolutionInspector.Commands
{
  internal class InitializeCommand : ConsoleCommandBase<InitializeCommand.RawArguments, InitializeCommand.ParsedArguments>
  {
    private readonly IConsoleHelper _consoleHelper;
    private readonly IFileStatic _file;
    private readonly IAssembly _resourceAssembly;

    public InitializeCommand (IAssembly resourceAssembly, IFileStatic file, IConsoleHelper consoleHelper)
      : base("initialize", "Creates a new SolutionInspector configuration file or overwrites an existing one.")
    {
      _resourceAssembly = resourceAssembly;
      _file = file;
      _consoleHelper = consoleHelper;
    }

    protected override void SetupArguments (IArgumentsBuilder<RawArguments> argumentsBuilder)
    {
      argumentsBuilder
          .Flag("force", "f", "Overwrite file if it exists without asking for confirmation.", (a, v) => a.Force = v)
          .Values(c => c.Value("configFilePath", (a, v) => a.ConfigFilePath = v));
    }

    protected override ParsedArguments ValidateAndParseArguments (RawArguments arguments)
    {
      return new ParsedArguments(arguments.ConfigFilePath, arguments.Force);
    }

    protected override int Run (ParsedArguments arguments)
    {
      if (_file.Exists(arguments.ConfigFilePath) && !arguments.Force)
      {
        if(!_consoleHelper.Confirm($"File '{arguments.ConfigFilePath}' already exists. Do you want to overwrite it?"))
          return ReportAbortion();
      }

      using (var sourceStream = _resourceAssembly.GetManifestResourceStream("SolutionInspector.Template.SolutionInspectorConfig").AssertNotNull())
      using (var destinationStream = _file.Open(arguments.ConfigFilePath, FileMode.Create))
      {
        sourceStream.CopyTo(destinationStream);
      }

      return 0;
    }

    public class RawArguments
    {
      public string ConfigFilePath { get; set; }
      public bool Force { get; set; }
    }

    public class ParsedArguments
    {
      public ParsedArguments (string configFilePath, bool force)
      {
        ConfigFilePath = configFilePath;
        Force = force;
      }

      public string ConfigFilePath { get; }
      public bool Force { get; }
    }
  }
}