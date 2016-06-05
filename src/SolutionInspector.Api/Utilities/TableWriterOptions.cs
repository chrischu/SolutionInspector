using System;
using System.Diagnostics.CodeAnalysis;

namespace SolutionInspector.Api.Utilities
{
  [ExcludeFromCodeCoverage]
  internal class TableWriterOptions
  {
    public int PreferredTableWidth { get; set; } = 120;
    public ConsoleTableWriterCharacters Characters { get; set; } = new ConsoleTableWriterCharacters();
  }
}