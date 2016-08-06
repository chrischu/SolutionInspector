using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace SolutionInspector.Utilities
{
  [PublicAPI]
  [ExcludeFromCodeCoverage]
  internal class TableWriterCharacters
  {
    public static readonly TableWriterCharacters AdvancedAscii = new TableWriterCharacters
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