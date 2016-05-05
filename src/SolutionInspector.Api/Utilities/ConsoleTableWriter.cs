using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using JetBrains.Annotations;
using SolutionInspector.Api.Extensions;

namespace SolutionInspector.Api.Utilities
{
  [PublicAPI]
  internal interface IConsoleTableWriter
  {
    void Write<T> (IEnumerable<T> rows, params Expression<Func<T, object>>[] columnSelectors);
    void Write<T> (IEnumerable<T> rows, string[] headers, params Func<T, object>[] columnSelectors);
  }

  internal class ConsoleTableWriter : IConsoleTableWriter
  {
    private readonly TextWriter _consoleOut;
    private readonly ConsoleTableWriterOptions _options;

    public ConsoleTableWriter (TextWriter consoleOut, ConsoleTableWriterOptions options = null)
    {
      _consoleOut = consoleOut;
      _options = options ?? new ConsoleTableWriterOptions();
    }

    public void Write<T> (IEnumerable<T> rows, params Expression<Func<T, object>>[] columnSelectors)
    {
      var headers = columnSelectors.Select(GetMemberName).ToArray();
      var selectors = columnSelectors.Select(exp => exp.Compile()).ToArray();
      Write(rows, headers, selectors);
    }

    public void Write<T> (IEnumerable<T> rows, string[] headers, params Func<T, object>[] columnSelectors)
    {
      Write(rows.ToArray(), headers, columnSelectors);
    }

    private void Write<T> (T[] rows, string[] headers, Func<T, object>[] columnSelectors)
    {
      Trace.Assert(headers.Length == columnSelectors.Length);

      var cells = new string[rows.Length + 1, columnSelectors.Length];

      // Fill headers
      for (int colIndex = 0; colIndex < cells.GetLength(1); colIndex++)
      {
        cells[0, colIndex] = headers[colIndex];
      }

      // Fill table rows
      for (int rowIndex = 1; rowIndex < cells.GetLength(0); rowIndex++)
      {
        for (int colIndex = 0; colIndex < cells.GetLength(1); colIndex++)
        {
          object value = columnSelectors[colIndex].Invoke(rows[rowIndex - 1]);

          cells[rowIndex, colIndex] = value?.ToString() ?? "null";
        }
      }

      Write(cells);
    }

    private void Write (string[,] cells)
    {
      var columnWidthRanges = CalculatePreferredColumnWidthRanges(cells);
      var availableWidth = CalculateAvailableWidth(cells, columnWidthRanges);
      var columnWidths = CalculateColumnWidths(columnWidthRanges, availableWidth);

      WriteRowSeparator(columnWidths, -1);

      WriteHeaderRow(cells, columnWidths);
      WriteRowSeparator(columnWidths, 0);

      for (int rowIndex = 1; rowIndex < cells.GetLength(0); rowIndex++)
      {
        WriteRow(cells, rowIndex, columnWidths);

        if (rowIndex + 1 < cells.GetLength(0))
          WriteRowSeparator(columnWidths, 0);
      }

      WriteRowSeparator(columnWidths, 1);
    }

    private void WriteHeaderRow (string[,] cells, int[] columnWidths)
    {
      _consoleOut.Write(_options.Characters.Vertical);

      for (int colIndex = 0; colIndex < columnWidths.Length; colIndex++)
      {
        _consoleOut.Write(' ');

        var header = cells[0, colIndex];
        var excessWidth = columnWidths[colIndex] - header.Length;

        int left = 0;
        int right = 0;
        if (excessWidth > 0)
        {
          left = excessWidth / 2 + (excessWidth % 2 == 0 ? 0 : 1);
          right = excessWidth - left;
        }

        _consoleOut.Write(new string(' ', left));
        _consoleOut.Write(header);
        _consoleOut.Write(new string(' ', right));

        _consoleOut.Write(' ');
        _consoleOut.Write(_options.Characters.Vertical);
      }

      _consoleOut.WriteLine();
    }

    private void WriteRow (string[,] cells, int rowIndex, int[] columnWidths)
    {
      var row = new string[cells.GetLength(1)][];

      for (int colIndex = 0; colIndex < cells.GetLength(1); colIndex++)
        row[colIndex] = SplitIntoLines(cells[rowIndex, colIndex], columnWidths[colIndex]).ToArray();

      var lineCount = row.Max(r => r.Length);
      for (int lineIndex = 0; lineIndex < lineCount; lineIndex++)
      {
        _consoleOut.Write(_options.Characters.Vertical);

        for (int colIndex = 0; colIndex < row.Length; colIndex++)
        {
          _consoleOut.Write(' ');

          _consoleOut.Write(
              lineIndex >= row[colIndex].Length
                  ? new string(' ', columnWidths[colIndex])
                  : row[colIndex][lineIndex].PadRight(columnWidths[colIndex]));

          _consoleOut.Write(' ');
          _consoleOut.Write(_options.Characters.Vertical);
        }

        _consoleOut.WriteLine();
      }
    }

    private IEnumerable<string> SplitIntoLines (string cellValue, int columnWidth)
    {
      var sb = new StringBuilder();

      var words = cellValue.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

      foreach (var word in words)
      {
        if (sb.Length + word.Length > columnWidth)
        {
          Trace.Assert(sb.Length > 0);
          sb.Remove(sb.Length - 1, 1);
          yield return sb.ToString();
          sb.Clear();
        }

        sb.Append(word);
        sb.Append(' ');
      }

      Trace.Assert(sb.Length > 0);
      sb.Remove(sb.Length - 1, 1);
      yield return sb.ToString();
    }

    private void WriteRowSeparator (int[] columnWidths, int indicator)
    {
      var start = indicator < 0
          ? _options.Characters.TopLeftCorner
          : indicator > 0 ? _options.Characters.BottomLeftCorner : _options.Characters.LeftMiddle;
      var end = indicator < 0
          ? _options.Characters.TopRightCorner
          : indicator > 0 ? _options.Characters.BottomRightCorner : _options.Characters.RightMiddle;
      var cross = indicator < 0 ? _options.Characters.TopMiddle : indicator > 0 ? _options.Characters.BottomMiddle : _options.Characters.Cross;

      _consoleOut.Write(start);

      for (int i = 0; i < columnWidths.Length; i++)
      {
        _consoleOut.Write(new string(_options.Characters.Horizontal, columnWidths[i] + 2));
        if (i + 1 < columnWidths.Length)
          _consoleOut.Write(cross);
      }

      _consoleOut.WriteLine(end);
    }

    private int[] CalculateColumnWidths (WidthRange[] columnWidthRanges, int availableWidth)
    {
      var columnWidths = columnWidthRanges.Select(r => r.Min).ToArray();

      int dividableWidth = availableWidth - columnWidths.Sum();

      for (int i = 0; i < columnWidths.Length; i++)
      {
        if (columnWidthRanges[i].Diff != 0)
          columnWidths[i] += (int) Math.Floor((decimal) dividableWidth / columnWidthRanges[i].Diff);
      }

      dividableWidth = availableWidth - columnWidths.Sum();
      var index = 0;
      while (dividableWidth > 0 && columnWidths.Select((w, i) => new { w, i }).Any(x => x.w < columnWidthRanges[x.i].Max))
      {
        if (columnWidths[index] < columnWidthRanges[index].Max)
        {
          columnWidths[index] ++;
          dividableWidth--;
        }
        index = (index + 1) % columnWidths.Length;
      }

      return columnWidths;
    }

    private WidthRange[] CalculatePreferredColumnWidthRanges (string[,] cells)
    {
      var widthRanges = new WidthRange[cells.GetLength(1)];

      for (int colIndex = 0; colIndex < cells.GetLength(1); colIndex++)
      {
        widthRanges[colIndex] = new WidthRange();

        for (int rowIndex = 0; rowIndex < cells.GetLength(0); rowIndex++)
        {
          var cellValue = cells[rowIndex, colIndex];
          var cellMax = cellValue.Length;
          var cellMin = cellValue.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Max(s => s.Length);

          widthRanges[colIndex].TrySetNewMax(cellMax);
          widthRanges[colIndex].TrySetNewMin(cellMin);
        }
      }

      return widthRanges;
    }

    private int CalculateAvailableWidth (string[,] cells, WidthRange[] widthRanges)
    {
      var widthForLines =
          2 * 2 + // outer lines '| ' and ' |'
          (cells.GetLength(1) - 1) * 3 + // inner lines ' | '
          1; // fixes line break issues in Win10 command line

      var availableWidth = _options.PreferredTableWidth - widthForLines - 1;

      var minimumRequiredWidth = widthRanges.Sum(r => r.Min);

      if (availableWidth < minimumRequiredWidth)
        availableWidth = minimumRequiredWidth;

      return availableWidth;
    }

    private string GetMemberName<T> (Expression<Func<T, object>> expression)
    {
      var memberExpression = expression.Body as MemberExpression ?? ((UnaryExpression) expression.Body).Operand as MemberExpression;
      return memberExpression.AssertNotNull().Member.Name;
    }

    private class WidthRange
    {
      public int Max { get; private set; } = int.MinValue;
      public int Min { get; private set; } = int.MinValue;

      public int Diff => Max - Min;

      public void TrySetNewMax (int max)
      {
        Max = Math.Max(max, Max);
      }

      public void TrySetNewMin (int min)
      {
        Min = Math.Max(min, Min);
      }
    }
  }
}