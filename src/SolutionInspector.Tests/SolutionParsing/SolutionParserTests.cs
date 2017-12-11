using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.SolutionParsing;
using SolutionInspector.SolutionParsing.Tokenizing;
using SolutionInspector.TestInfrastructure;

namespace SolutionInspector.Tests.SolutionParsing
{
  [TestFixture]
  internal class SolutionParserTests
  {
    private const int c_errorLineNumber = -1;

    private string _temporaryFilePath;

    private ISolutionTokenizer _solutionTokenizer;

    private ISolutionParser _sut;

    [SetUp]
    public void SetUp ()
    {
      _temporaryFilePath = Path.GetTempFileName();

      _solutionTokenizer = A.Fake<ISolutionTokenizer>();

      _sut = new SolutionParser(_solutionTokenizer);
    }

    [Test]
    public void Parse ()
    {
      var projectId1 = Some.Guid;
      var projectName1 = Some.String;
      var projectRelativePath1 = Some.String;
      var projectType1 = Some.Guid;

      var projectId2 = Some.Guid;
      var projectName2 = Some.String;
      var projectRelativePath2 = Some.String;
      var projectType2 = Some.Guid;

      var tokens = new SolutionToken[]
                   {
                     Preamble(),

                     ProjectStart(projectId1, projectName1, projectRelativePath1, projectType1),
                     ProjectEnd(),

                     ProjectStart(projectId2, projectName2, projectRelativePath2, projectType2),
                     ProjectSectionStart(),
                     ProjectSectionItem("Item1"),
                     ProjectSectionItem("Item2"),
                     ProjectSectionEnd(),
                     ProjectEnd(),

                     Globals()
                   };

      // ACT
      var result = Parse(tokens);

      // ASSERT
      result.Projects.ShouldBeEquivalentTo(
        new[]
        {
          new
          {
            Id = projectId1,
            Type = projectType1,
            Name = projectName1,
            RelativePath = projectRelativePath1,
            Items = new string[0]
          },
          new
          {
            Id = projectId2,
            Type = projectType2,
            Name = projectName2,
            RelativePath = projectRelativePath2,
            Items = new[] { "Item1", "Item2" }
          }
        });
    }

    [Test]
    public void Parse_WithUnexpectedTokenType_Throws ()
    {
      // ACT
      Action act = () => Parse(Any());

      // ASSERT
      act.ShouldThrow<NotSupportedException>().WithMessage($"Unexpected token type '{typeof(DummySolutionToken).Name}");
    }

    [Test]
    [TestCaseSource(nameof(Parse_Errors_TestCaseSource))]
    public void Parse_Errors (IEnumerable<SolutionToken> tokens, string expectedMessage)
    {
      // ACT
      Action act = () => Parse(tokens.ToArray());

      // ASSERT
      act.ShouldThrow<SolutionParsingException>().WithMessage($"Error on line {c_errorLineNumber}: {expectedMessage}.");
    }

    public static IEnumerable Parse_Errors_TestCaseSource ()
    {
      yield return TestCase("DuplicatePreamble", "There can be only one preamble", Preamble(), Preamble(c_errorLineNumber));

      yield return TestCase("ElementAfterGlobals", "No element is allowed after the globals section", Globals(), Any(c_errorLineNumber));

      yield return TestCase(
        "ProjectInProjectPreSection",
        "Project cannot be contained in another project",
        ProjectStart(),
        ProjectStart(lineNumber: c_errorLineNumber));

      yield return TestCase(
        "ProjectInProjectPostSection",
        "Project cannot be contained in another project",
        ProjectStart(),
        ProjectSectionStart(),
        ProjectSectionEnd(),
        ProjectStart(lineNumber: c_errorLineNumber));

      yield return TestCase(
        "ProjectInProjectSection",
        "Project cannot be contained in a project section",
        ProjectStart(),
        ProjectSectionStart(),
        ProjectStart(lineNumber: c_errorLineNumber));

      yield return TestCase("UnexpectedProjectEnd", "Unexpected project end", ProjectEnd(lineNumber: c_errorLineNumber));

      yield return TestCase(
        "UnexpectedProjectSectionStart",
        "Unexpected project section start (a project section can only be contained in a project)",
        ProjectSectionStart(lineNumber: c_errorLineNumber));

      yield return TestCase(
        "DuplicateProjectSection",
        "There can only be one project section per project",
        ProjectStart(),
        ProjectSectionStart(),
        ProjectSectionEnd(),
        ProjectSectionStart(lineNumber: c_errorLineNumber));

      yield return TestCase(
        "UnexpectedProjectSectionEnd",
        "Unexpected project section end",
        ProjectSectionEnd(lineNumber: c_errorLineNumber));

      yield return TestCase(
        "UnexpectedProjectSectionItem",
        "Unexpected project section item (can only be contained in a project section)",
        ProjectSectionItem(lineNumber: c_errorLineNumber));

      yield return TestCase(
        "GlobalsInsideProject",
        "The globals section cannot be contained inside a project",
        ProjectStart(),
        Globals(lineNumber: c_errorLineNumber));

      yield return TestCase(
        "GlobalsInsideProjectSection",
        "The globals section cannot be contained inside a project section",
        ProjectStart(),
        ProjectSectionStart(),
        Globals(lineNumber: c_errorLineNumber));

      yield return TestCase("MissingGlobals", "Missing globals section", Preamble());

      TestCaseData TestCase (string name, string expectedMessage, params SolutionToken[] tokens)
      {
        return new TestCaseData(tokens, expectedMessage) { TestName = name };
      }
    }

    private Solution Parse (params SolutionToken[] tokens)
    {
      A.CallTo(() => _solutionTokenizer.Tokenize(A<LineEnumerator>._)).Returns(tokens);
      return _sut.Parse(_temporaryFilePath);
    }

    private static GlobalsSolutionToken Globals (int? lineNumber = null)
    {
      return new GlobalsSolutionToken(Some.String, lineNumber ?? Some.PositiveInteger);
    }

    private static PreambleSolutionToken Preamble (int? lineNumber = null)
    {
      return new PreambleSolutionToken(Some.String, lineNumber ?? Some.PositiveInteger);
    }

    private static ProjectStartSolutionToken ProjectStart (
      Guid? id = null,
      string name = null,
      string relativePath = null,
      Guid? type = null,
      int? lineNumber = null)
    {
      return new ProjectStartSolutionToken(
        Some.String,
        lineNumber ?? Some.PositiveInteger,
        id ?? Some.Guid,
        name ?? Some.String,
        relativePath ?? Some.String,
        type ?? Some.Guid);
    }

    private static ProjectEndSolutionToken ProjectEnd(int? lineNumber = null)
    {
      return new ProjectEndSolutionToken(Some.String, lineNumber ?? Some.PositiveInteger);
    }

    private static ProjectSectionStartSolutionToken ProjectSectionStart (int? lineNumber = null)
    {
      return new ProjectSectionStartSolutionToken(Some.String, lineNumber ?? Some.PositiveInteger);
    }

    private static ProjectSectionEndSolutionToken ProjectSectionEnd (int? lineNumber = null)
    {
      return new ProjectSectionEndSolutionToken(Some.String, lineNumber ?? Some.PositiveInteger);
    }

    private static ProjectSectionItemSolutionToken ProjectSectionItem (string name = null, int? lineNumber = null)
    {
      return new ProjectSectionItemSolutionToken(Some.String, lineNumber ?? Some.PositiveInteger, name ?? Some.String);
    }

    private static SolutionToken Any (int? lineNumber = null)
    {
      return new DummySolutionToken(Some.String, lineNumber ?? Some.PositiveInteger);
    }

    private class DummySolutionToken : SolutionToken
    {
      public DummySolutionToken (string rawValue, int lineNumber)
        : base(rawValue, lineNumber)
      {
      }
    }
  }
}