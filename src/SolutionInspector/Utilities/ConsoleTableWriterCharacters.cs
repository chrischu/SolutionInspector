using JetBrains.Annotations;

namespace SolutionInspector.Utilities
{
  [PublicAPI]
  internal class ConsoleTableWriterCharacters
  {
    public static readonly ConsoleTableWriterCharacters AdvancedAscii = new ConsoleTableWriterCharacters
                                                                        {
                                                                            TopLeftCorner = '┌',
                                                                            BottomLeftCorner = '└',
                                                                            TopRightCorner = '┐',
                                                                            BottomRightCorner = '┘',
                                                                            Cross = '┼',
                                                                            LeftMiddle = '├',
                                                                            RightMiddle = '┤',
                                                                            TopMiddle = '┬',
                                                                            BottomMiddle = '┴',
                                                                            Vertical = '│',
                                                                            Horizontal = '─'
                                                                        };

    public char TopLeftCorner { get; set; } = '+';
    public char BottomLeftCorner { get; set; } = '+';
    public char TopRightCorner { get; set; } = '+';
    public char BottomRightCorner { get; set; } = '+';
    public char Cross { get; set; } = '+';
    public char LeftMiddle { get; set; } = '+';
    public char RightMiddle { get; set; } = '+';
    public char TopMiddle { get; set; } = '+';
    public char BottomMiddle { get; set; } = '+';
    public char Vertical { get; set; } = '│';
    public char Horizontal { get; set; } = '-';
  }
}