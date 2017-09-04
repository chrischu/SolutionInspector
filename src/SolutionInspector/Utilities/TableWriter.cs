using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using SolutionInspector.Commons.Attributes;
using SolutionInspector.Commons.Extensions;

namespace SolutionInspector.Utilities
{
  [ForFutureUse]
  internal interface ITableWriter
  {
    void Write<T> (TextWriter writer, IEnumerable<T> rows, params Expression<Func<T, object>>[] columnSelectors);
    void Write<T> (TextWriter writer, IEnumerable<T> rows, string[] headers, params Func<T, object>[] columnSelectors);
  }

  internal class TableWriter : ITableWriter
  {
    private readonly TableWriterOptions _options;

    public TableWriter (TableWriterOptions options = null)
    {
      _options = options ?? new TableWriterOptions();
    }

    public void Write<T> (TextWriter writer, IEnumerable<T> rows, params Expression<Func<T, object>>[] columnSelectors)
    {
      var headers = columnSelectors.Select(GetMemberName).ToArray();
      var selectors = columnSelectors.Select(exp => exp.Compile()).ToArray();
      Write(writer, rows, headers, selectors);
    }

    public void Write<T> (TextWriter writer, IEnumerable<T> rows, string[] headers, params Func<T, object>[] columnSelectors)
    {
      Write(writer, rows.ToArray(), headers, columnSelectors);
    }

    private void Write<T> (TextWriter writer, T[] rows, string[] headers, Func<T, object>[] columnSelectors)
    {
      Trace.Assert(headers.Length == columnSelectors.Length);

      var cells = new string[rows.Length + 1, columnSelectors.Length];

      // Fill headers
      for (var colIndex = 0; colIndex < cells.GetLength(1); colIndex++)
        cells[0, colIndex] = headers[colIndex];

      // Fill table rows
      for (var rowIndex = 1; rowIndex < cells.GetLength(0); rowIndex++)
      for (var colIndex = 0; colIndex < cells.GetLength(1); colIndex++)
      {
        var value = columnSelectors[colIndex].Invoke(rows[rowIndex - 1]);

        cells[rowIndex, colIndex] = value?.ToString() ?? "null";
      }

      Write(writer, cells);
    }

    private void Write (TextWriter writer, string[,] cells)
    {
      var columnWidthRanges = CalculatePreferredColumnWidthRanges(cells);
      var availableWidth = CalculateAvailableWidth(cells, columnWidthRanges);
      var columnWidths = CalculateColumnWidths(columnWidthRanges, availableWidth);

      WriteRowSeparator(writer, columnWidths, -1);

      WriteHeaderRow(writer, cells, columnWidths);
      WriteRowSeparator(writer, columnWidths, 0);

      for (var rowIndex = 1; rowIndex < cells.GetLength(0); rowIndex++)
      {
        WriteRow(writer, cells, rowIndex, columnWidths);

        if (rowIndex + 1 < cells.GetLength(0))
          WriteRowSeparator(writer, columnWidths, 0);
      }

      WriteRowSeparator(writer, columnWidths, 1);
    }

    private void WriteHeaderRow (TextWriter writer, string[,] cells, int[] columnWidths)
    {
      writer.Write(_options.Characters.Vertical);

      for (var colIndex = 0; colIndex < columnWidths.Length; colIndex++)
      {
        writer.Write(' ');

        var header = cells[0, colIndex];
        var excessWidth = columnWidths[colIndex] - header.Length;

        var left = 0;
        var right = 0;
        if (excessWidth > 0)
        {
          left = excessWidth / 2 + (excessWidth % 2 == 0 ? 0 : 1);
          right = excessWidth - left;
        }

        writer.Write(new string(' ', left));
        writer.Write(header);
        writer.Write(new string(' ', right));

        writer.Write(' ');
        writer.Write(_options.Characters.Vertical);
      }

      writer.WriteLine();
    }

    private void WriteRow (TextWriter writer, string[,] cells, int rowIndex, int[] columnWidths)
    {
      var row = new string[cells.GetLength(1)][];

      for (var colIndex = 0; colIndex < cells.GetLength(1); colIndex++)
        row[colIndex] = SplitIntoLines(cells[rowIndex, colIndex], columnWidths[colIndex]).ToArray();

      var lineCount = row.Max(r => r.Length);
      for (var lineIndex = 0; lineIndex < lineCount; lineIndex++)
      {
        writer.Write(_options.Characters.Vertical);

        for (var colIndex = 0; colIndex < row.Length; colIndex++)
        {
          writer.Write(' ');

          writer.Write(
            lineIndex >= row[colIndex].Length
              ? new string(' ', columnWidths[colIndex])
              : row[colIndex][lineIndex].PadRight(columnWidths[colIndex]));

          writer.Write(' ');
          writer.Write(_options.Characters.Vertical);
        }

        writer.WriteLine();
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

    private void WriteRowSeparator (TextWriter writer, int[] columnWidths, int indicator)
    {
      var start = indicator < 0
        ? _options.Characters.TopLeftCorner
        : indicator > 0 ? _options.Characters.BottomLeftCorner : _options.Characters.LeftMiddle;
      var end = indicator < 0
        ? _options.Characters.TopRightCorner
        : indicator > 0 ? _options.Characters.BottomRightCorner : _options.Characters.RightMiddle;
      var cross = indicator < 0 ? _options.Characters.TopMiddle : indicator > 0 ? _options.Characters.BottomMiddle : _options.Characters.Cross;

      writer.Write(start);

      for (var i = 0; i < columnWidths.Length; i++)
      {
        writer.Write(new string(_options.Characters.Horizontal, columnWidths[i] + 2));
        if (i + 1 < columnWidths.Length)
          writer.Write(cross);
      }

      writer.WriteLine(end);
    }

    private int[] CalculateColumnWidths (WidthRange[] columnWidthRanges, int availableWidth)
    {
      var columnWidths = columnWidthRanges.Select(r => r.Min).ToArray();

      var dividableWidth = availableWidth - columnWidths.Sum();

      for (var i = 0; i < columnWidths.Length; i++)
        if (columnWidthRanges[i].Diff != 0)
          columnWidths[i] += (int) Math.Floor((decimal) dividableWidth / columnWidthRanges[i].Diff);

      dividableWidth = availableWidth - columnWidths.Sum();
      var index = 0;
      while (dividableWidth > 0 && columnWidths.Select((w, i) => new { w, i }).Any(x => x.w < columnWidthRanges[x.i].Max))
      {
        if (columnWidths[index] < columnWidthRanges[index].Max)
        {
          columnWidths[index]++;
          dividableWidth--;
        }
        index = (index + 1) % columnWidths.Length;
      }

      return columnWidths;
    }

    private WidthRange[] CalculatePreferredColumnWidthRanges (string[,] cells)
    {
      var widthRanges = new WidthRange[cells.GetLength(1)];

      for (var colIndex = 0; colIndex < cells.GetLength(1); colIndex++)
      {
        widthRanges[colIndex] = new WidthRange();

        for (var rowIndex = 0; rowIndex < cells.GetLength(0); rowIndex++)
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