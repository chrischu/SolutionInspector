using System;
using SolutionInspector.Commons.Extensions;
using Wrapperator.Interfaces.Diagnostics;

namespace SolutionInspector.Commands
{
  internal class ConfigureCommand : SolutionInspectorCommand<ConfigureCommand.RawArguments, ConfigureCommand.ParsedArguments>
  {
    private readonly IProcessStatic _process;

    public ConfigureCommand (IProcessStatic process)
        : base("configure", "Starts SolutionInspector configuration UI for more convenient configuration.")
    {
      _process = process;
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
      // TODO: 
      var process = _process.Start(@"..\..\..\SolutionInspector.ConfigurationUi\bin\Debug\SolutionInspector.ConfigurationUi.exe").AssertNotNull();
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