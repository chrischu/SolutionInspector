using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using SolutionInspector.Commons.Extensions;

namespace SolutionInspector.SolutionParsing.Tokenizing
{
  internal interface ISolutionTokenizer
  {
    IEnumerable<SolutionToken> Tokenize(LineEnumerator lines);
  }

  internal class SolutionTokenizer : ISolutionTokenizer
  {
    private static readonly Regex s_projectSectionItemRegex = new Regex("^(?<value1>[^ ]+) = (?<value2>[^ ]+)$");

    private static readonly Regex s_projectStartRegex;

    static SolutionTokenizer()
    {
      const string guidRegex = @"""{{(?<{0}>[A-F0-9]{{8}}-[A-F0-9]{{4}}-[A-F0-9]{{4}}-[A-F0-9]{{4}}-[A-F0-9]{{12}})}}""";
      const string stringRegex = @"""(?<{0}>[^""]+)""";

      var regex = $"^Project\\({string.Format(guidRegex, "type")}\\) = " +
                  $"{string.Format(stringRegex, "name")}, {string.Format(stringRegex, "path")}, {string.Format(guidRegex, "id")}$";

      s_projectStartRegex = new Regex(regex);
    }

    public IEnumerable<SolutionToken> Tokenize (LineEnumerator lines)
    {
      var preamble = GetPreamble(lines);
      if (preamble != null)
        yield return preamble;

      if (lines.ReachedEnd)
        yield break;

      do
      {
        var trimmedCurrentLine = lines.Current.AssertNotNull().Trim();

        if (trimmedCurrentLine.StartsWith("ProjectSection", StringComparison.Ordinal))
        {
          yield return new ProjectSectionStartSolutionToken(lines.Current, lines.LineNumber);
        }
        else if (trimmedCurrentLine.StartsWith("Project", StringComparison.Ordinal))
        {
          var match = s_projectStartRegex.Match(trimmedCurrentLine);

          if (!match.Success)
            throw new SolutionParsingException(lines.LineNumber, $"Invalid project start '{trimmedCurrentLine}'");

          yield return new ProjectStartSolutionToken(
            lines.Current, lines.LineNumber,
            Guid.Parse(match.Groups["type"].Value),
            match.Groups["name"].Value,
            match.Groups["path"].Value,
            Guid.Parse(match.Groups["id"].Value));
        }
        else if (trimmedCurrentLine == "EndProject")
        {
          yield return new ProjectEndSolutionToken(lines.Current, lines.LineNumber);
        }
        else if (trimmedCurrentLine == "EndProjectSection")
        {
          yield return new ProjectSectionEndSolutionToken(lines.Current, lines.LineNumber);
        }
        else if(trimmedCurrentLine == "Global")
        {
          yield return GetGlobals(lines);
        }
        else
        {
          var match = s_projectSectionItemRegex.Match(trimmedCurrentLine);

          if (!match.Success)
            throw new SolutionParsingException(lines.LineNumber, $"Unexpected line '{trimmedCurrentLine}'");

          var value1 = match.Groups["value1"].Value;
          var value2 = match.Groups["value2"].Value;

          if (value1 != value2)
            throw new SolutionParsingException(lines.LineNumber, "Invalid project section element (both values must match)");

          yield return new ProjectSectionItemSolutionToken(lines.Current, lines.LineNumber, value1);
        }

      } while (!lines.ReachedEnd && lines.MoveNext());
    }

    [CanBeNull]
    private PreambleSolutionToken GetPreamble(LineEnumerator lines)
    {
      var sb = new StringBuilder();
      var atLeastOneLine = false;

      while (lines.MoveNext() && !lines.Current.AssertNotNull().Trim().StartsWith("Project", StringComparison.Ordinal))
      {
        atLeastOneLine = true;
        sb.AppendLine(lines.Current);
      }

      return atLeastOneLine ? new PreambleSolutionToken(sb.ToString(), 1) : null;
    }

    private GlobalsSolutionToken GetGlobals(LineEnumerator lines)
    {
      var startingLineNumber = lines.LineNumber;
      return new GlobalsSolutionToken(ReadWhileToString(lines, l => l.Trim() != "EndGlobal"), startingLineNumber);
    }

    private string ReadWhileToString(IEnumerator<string> lines, Func<string, bool> continueReading)
    {
      var sb = new StringBuilder();
      ReadWhile(lines, continueReading, l => sb.AppendLine(l));
      return sb.ToString();
    }

    private void ReadWhile(IEnumerator<string> lines, Func<string, bool> continueReading, Action<string> processLine)
    {
      do
      {
        processLine(lines.Current);
      } while (lines.MoveNext() && continueReading(lines.Current));
    }
  }
}