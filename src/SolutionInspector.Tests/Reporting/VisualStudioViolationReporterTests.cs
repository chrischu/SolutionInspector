using System.IO;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Api.Rules;
using SolutionInspector.Reporting;
using SolutionInspector.TestInfrastructure.AssertionExtensions;

namespace SolutionInspector.Tests.Reporting
{
  public class VisualStudioViolationReporterTests
  {
    private TextWriter _textWriter;

    private VisualStudioViolationReporter _sut;

    [SetUp]
    public void SetUp ()
    {
      _textWriter = new StringWriter();

      _sut = new VisualStudioViolationReporter(_textWriter);
    }

    [TearDown]
    public void TearDown ()
    {
      _sut.Dispose();
    }

    [Test]
    public void Report ()
    {
      var ruleViolations = new[]
                           {
                             RuleViolationObjectMother.Create(new SomeRule(), targetPath: "TP1", message: "MSG1"),
                             RuleViolationObjectMother.Create(new SomeRule(), targetPath: "TP2", message: "MSG2")
                           };
      // ACT
      _sut.Report(ruleViolations);

      // ASSERT
      _textWriter.ToString().Should().BeWithDiff(@"
TP1: SolutionInspector warning SI0000: MSG1 (Some)
TP2: SolutionInspector warning SI0000: MSG2 (Some)");
    }

    private class SomeRule : IRule
    {
    }
  }
}