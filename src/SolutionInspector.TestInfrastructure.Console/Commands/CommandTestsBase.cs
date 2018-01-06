using System;
using System.IO;
using ManyConsole;
using NUnit.Framework;

namespace SolutionInspector.TestInfrastructure.Console.Commands
{
  public class CommandTestsBase
  {
    protected TextWriter CapturedOutput { get; private set; }

    [SetUp]
    public void SetUp ()
    {
      CapturedOutput = new StringWriter();
    }

    protected int RunCommand (ConsoleCommand command, params string[] arguments)
    {
      return ConsoleCommandDispatcher.DispatchCommand(command, arguments, CapturedOutput);
    }
  }
}