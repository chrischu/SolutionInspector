using System;
using SolutionInspector.Commons.Extensions;
using Wrapperator.Interfaces.Diagnostics;

namespace SolutionInspector.Commands
{
  internal class ConfigureCommand : SolutionInspectorCommand<ConfigureCommand.RawArguments, ConfigureCommand.ParsedArguments>
  {
    private readonly IProcessStatic _process;
    private readonly string _configurationUiPath;

    public ConfigureCommand (string configurationUiPath, IProcessStatic process)
        : base("configure", "Starts SolutionInspector configuration UI for more convenient configuration.")
    {
      _process = process;
      _configurationUiPath = configurationUiPath;
    }

    protected override void SetupArguments (IArgumentsBuilder<RawArguments> argumentsBuilder)
    {
    }

    protected override ParsedArguments ValidateAndParseArguments (RawArguments arguments, Func<string, Exception> reportError)
    {
      return new ParsedArguments();
    }

    protected override int Run (ParsedArguments arguments)
    {
      var process = _process.Start(_configurationUiPath).AssertNotNull();
      process.WaitForExit();
      return process.ExitCode;
    }

    public class RawArguments
    {
    }

    public class ParsedArguments
    {
    }
  }
}