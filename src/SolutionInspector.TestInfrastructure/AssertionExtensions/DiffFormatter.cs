using System;
using System.Text;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;

namespace SolutionInspector.TestInfrastructure.AssertionExtensions
{
  /// <summary>
  ///   Formats diffs to make them more readable.
  /// </summary>
  public static class DiffFormatter
  {
    /// <summary>
    ///   Formats the differences between <paramref name="old" /> and <paramref name="new" /> in a human-readable way.
    /// </summary>
    public static string FormatDiff (string old, string @new)
    {
      var diff = GetDiff(old, @new);
      return FormatDiff(diff);
    }

    private static DiffPaneModel GetDiff (string oldText, string newText)
    {
      var differ = new Differ();
      var inlineBuilder = new InlineDiffBuilder(differ);
      return inlineBuilder.BuildDiffModel(oldText, newText);
    }

    private static string FormatDiff (DiffPaneModel diff)
    {
      var sb = new StringBuilder();
      foreach (var line in diff.Lines)
        AppendLine(sb, line);

      return sb.ToString();
    }

    private static void AppendLine (StringBuilder sb, DiffPiece line)
    {
      switch (line.Type)
      {
        case ChangeType.Inserted:
          sb.Append("+ ");
          break;
        case ChangeType.Deleted:
          sb.Append("- ");
          break;
        default:
          sb.Append("  ");
          break;
      }

      sb.AppendLine(line.Text);
    }
  }
}