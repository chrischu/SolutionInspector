using System.IO;
using ManyConsole;
using NUnit.Framework;

namespace SolutionInspector.Tests.Commands
{
  public class CommandTestsBase
  {
    protected TextWriter TextWriter { get; private set; }

    [SetUp]
    public void SetUp ()
    {
      TextWriter = new StringWriter();
    }

    protected int RunCommand (ConsoleCommand command, params string[] arguments)
    {
      return ConsoleCommandDispatcher.DispatchCommand(command, arguments, TextWriter);
    }
  }
}