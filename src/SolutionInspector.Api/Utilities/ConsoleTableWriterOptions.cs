using System;

namespace SolutionInspector.Api.Utilities
{
  internal class ConsoleTableWriterOptions
  {
    public int PreferredTableWidth { get; set; } = 120;
    public ConsoleTableWriterCharacters Characters { get; set; } = new ConsoleTableWriterCharacters();
  }
}