using System;
using System.IO;
using Wrapperator.Interfaces;
using Wrapperator.Interfaces.IO;
using Wrapperator.Interfaces.Reflection;

namespace SolutionInspector.Api.Commands
{
  internal class InitializeCommand : SolutionInspectorCommand<InitializeCommand.RawArguments, InitializeCommand.ParsedArguments>
  {
    private readonly IAssembly _resourceAssembly;
    private readonly IFileStatic _file;
    private readonly IConsoleStatic _console;

    public InitializeCommand (IMsBuildInstallationChecker msBuildInstallationChecker, IAssembly resourceAssembly, IFileStatic file, IConsoleStatic console)
        : base(msBuildInstallationChecker, "initialize", "Creates a new SolutionInspector configuration file or overwrite an existing one.")
    {
      _resourceAssembly = resourceAssembly;
      _file = file;
      _console = console;
    }

    protected override void SetupArguments (IArgumentsBuilder<RawArguments> argumentsBuilder)
    {
      argumentsBuilder
          .Flag("force", "f", "Do not ask for confirmation if the file already exists.", (a, v) => a.Force = v)
          .Values(c => c.Value("configFilePath", (a, v) => a.ConfigFilePath = v));
    }

    protected override ParsedArguments ValidateAndParseArguments (RawArguments arguments, Func<string, Exception> reportError)
    {
      return new ParsedArguments(arguments.ConfigFilePath, arguments.Force);
    }

    protected override int Run (ParsedArguments arguments)
    {
      if (_file.Exists(arguments.ConfigFilePath) && !arguments.Force)
      {
        _console.WriteLine($"File '{arguments.ConfigFilePath}' already exists. Do you want to overwrite it? [y/N]");
        var answer = _console.ReadLine();
        if (string.IsNullOrEmpty(answer) || !string.Equals(answer, "y", StringComparison.OrdinalIgnoreCase))
        {
          _console.WriteLine("Command was aborted.");
          return 1;
        }
      }

      using (var sourceStream = _resourceAssembly.GetManifestResourceStream("Template.SolutionInspectorConfig"))
      using (var destinationStream = _file.Open(arguments.ConfigFilePath, FileMode.Create))
        sourceStream.CopyTo(destinationStream);

      return 0;
    }

    public class RawArguments
    {
      public string ConfigFilePath { get; set; }
      public bool Force { get; set; }
    }

    public class ParsedArguments
    {
      public string ConfigFilePath { get; }
      public bool Force { get; }

      public ParsedArguments (string configFilePath, bool force)
      {
        ConfigFilePath = configFilePath;
        Force = force;
      }
    }
  }
}