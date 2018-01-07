using System;
using Wrapperator.Interfaces;

namespace SolutionInspector.Commons.Console
{
  /// <summary>
  ///   Helper class to make some interactions with the console simpler.
  /// </summary>
  public interface IConsoleHelper
  {
    bool Confirm (string question);
  }

  internal class ConsoleHelper : IConsoleHelper
  {
    private readonly IConsoleStatic _console;

    public ConsoleHelper (IConsoleStatic console)
    {
      _console = console;
    }

    public bool Confirm (string question)
    {
      _console.Write($"{question} [y/N] ");
      var answer = _console.ReadLine();
      return answer != null && string.Equals(answer, "y", StringComparison.OrdinalIgnoreCase);
    }
  }
}