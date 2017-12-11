using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.SolutionParsing;
using SolutionInspector.SolutionParsing.Tokenizing;
using SolutionInspector.Utilities;

namespace SolutionInspector.Tests.Utilities
{
  [TestFixture]
  public class SolutionTokenizerTests
  {
    private ISolutionTokenizer _sut;

    private string _validProjectStart;
    private ProjectStartSolutionToken _validProjectStartToken;

    [SetUp]
    public void SetUp ()
    {
      _sut = new SolutionTokenizer();
      _validProjectStart = " Project(\"{AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA}\") = \"Name\", \"Path\", \"{00000000-0000-0000-0000-000000000001}\" ";
      _validProjectStartToken = new ProjectStartSolutionToken(
        _validProjectStart,
        2,
        Guid.Parse("00000000-0000-0000-0000-000000000001"),
        "Name",
        "Path",
        Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA"));
    }

    [Test]
    public void Tokenize ()
    {
      var input = new[]
                  {
                    " Anything goes here (Preamble) ",
                    _validProjectStart,
                    " Global ",
                    " EndGlobal ",
                    " EndProject ",
                    " EndProjectSection ",
                    " ProjectSection ",
                    " VALUE = VALUE "
                  };

      // ACT
      var result = Tokenize(input);

      // ASSERT
      result.ShouldBeEquivalentTo(
        new SolutionToken[]
        {
          new PreambleSolutionToken(" Anything goes here (Preamble) ", 1),
          _validProjectStartToken,
          new GlobalsSolutionToken($" Global{Environment.NewLine} EndGlobal ", 3),
          new ProjectEndSolutionToken(" EndProject", 5),
          new ProjectSectionEndSolutionToken(" EndProjectSection ", 6),
          new ProjectSectionStartSolutionToken(" ProjectSection ", 7),
          new ProjectSectionItemSolutionToken(" VALUE = VALUE ", 8, "VALUE")
        },
        opt => opt.WithStrictOrdering());
    }

    [Test]
    public void Tokenize_WithEmpty_ReturnsEmptyEnumerable ()
    {
      // ACT
      var result = Tokenize(new string[0]);

      // ASSERT
      result.Should().BeEmpty();
    }

    [Test]
    public void Tokenize_WithOnlyPreamble_ReturnsOnlyPreamble ()
    {
      // ACT
      var result = Tokenize(new[] { " Anything goes here (Preamble)" });

      // ASSERT
      result.ShouldBeEquivalentTo(new SolutionToken[] { new PreambleSolutionToken(" Anything goes here (Preamble)", 1) });
    }

    [Test]
    public void Tokenize_WithInvalidProjectStart_Throws ()
    {
      // ACT
      Action act = () => Tokenize(new[] { "   Project   " });

      // ASSERT
      act.ShouldThrow<SolutionParsingException>().WithMessage("Error on line 1: Invalid project start 'Project'.");
    }

    [Test]
    public void Tokenize_WithInvalidProjectSectionItem_Throws ()
    {
      // ACT
      Action act = () => Tokenize(new[] { _validProjectStart, " A = B " });

      // ASSERT
      act.ShouldThrow<SolutionParsingException>().WithMessage("Error on line 2: Invalid project section element (both values must match).");
    }

    [Test]
    public void Tokenize_WithUnexpectedValue_Throws ()
    {
      // ACT
      Action act = () => Tokenize(new[] { _validProjectStart, " XXX " });

      // ASSERT
      act.ShouldThrow<SolutionParsingException>().WithMessage("Error on line 2: Unexpected line 'XXX'.");
    }

    private IReadOnlyList<SolutionToken> Tokenize(IEnumerable<string> lines)
    {
      return _sut.Tokenize(new LineEnumerator(lines.GetEnumerator())).ToList();
    }
  }
}