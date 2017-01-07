using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FakeItEasy;
using NUnit.Framework;
using SolutionInspector.Api.Rules;
using SolutionInspector.Reporting;

namespace SolutionInspector.Tests.Reporting
{
  public class ViolationReporterBaseTests
  {
    private Action<TextWriter, IEnumerable<IRuleViolation>> _reportAction;

    private ViolationReporterBase _sut;
    private TextWriter _textWriter;

    [SetUp]
    public void SetUp ()
    {
      _textWriter = A.Fake<TextWriter>();
      _reportAction = A.Fake<Action<TextWriter, IEnumerable<IRuleViolation>>>();

      _sut = new DummyViolationReporter(_textWriter, _reportAction);
    }

    [TearDown]
    public void TearDown ()
    {
      _sut.Dispose();
    }

    [Test]
    public void Report_CallsReportAction ()
    {
      var ruleViolations = A.Dummy<IReadOnlyCollection<IRuleViolation>>();

      // ACT
      _sut.Report(ruleViolations);

      // ASSERT
      A.CallTo(() => _reportAction(_textWriter, ruleViolations)).MustHaveHappened();
    }

    [Test]
    public void Dispose_DisposesTestWriter ()
    {
      var disposeAction = A.Fake<Action>();
      _textWriter = new DummyTextWriter(disposeAction);
      _sut = new DummyViolationReporter(_textWriter, _reportAction);

      // ACT
      _sut.Dispose();

      // ASSERT
      A.CallTo(() => disposeAction()).MustHaveHappened();
    }

    private class DummyViolationReporter : ViolationReporterBase
    {
      private readonly Action<TextWriter, IEnumerable<IRuleViolation>> _report;

      public DummyViolationReporter (TextWriter writer, Action<TextWriter, IEnumerable<IRuleViolation>> report)
        : base(writer)
      {
        _report = report;
      }

      protected override void Report (TextWriter writer, IEnumerable<IRuleViolation> violations)
      {
        _report(writer, violations);
      }
    }

    private class DummyTextWriter : TextWriter
    {
      private readonly Action _dispose;

      public DummyTextWriter (Action dispose)
      {
        _dispose = dispose;
      }

      public override Encoding Encoding => Encoding.Default;

      protected override void Dispose (bool disposing)
      {
        _dispose();
        base.Dispose(disposing);
      }
    }
  }
}