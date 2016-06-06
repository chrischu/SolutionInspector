using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FakeItEasy;
using Machine.Specifications;
using SolutionInspector.Api.Reporting;
using SolutionInspector.Api.Rules;

#region R# preamble for Machine.Specifications files

// ReSharper disable ArrangeTypeModifiers
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable ClassNeverInstantiated.Local
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Local
// ReSharper disable NotAccessedField.Local
// ReSharper disable StaticMemberInGenericType
// ReSharper disable UnassignedField.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable UnassignedGetOnlyAutoProperty

#endregion

namespace SolutionInspector.Api.Tests.Reporting
{
  [Subject (typeof(ViolationReporterBase))]
  class ViolationReporterBaseSpec
  {
    static TextWriter TextWriter;
    static Action<TextWriter, IEnumerable<IRuleViolation>> ReportAction;

    static ViolationReporterBase SUT;

    Establish ctx = () =>
    {
      TextWriter = A.Fake<TextWriter>();
      ReportAction = A.Fake<Action<TextWriter, IEnumerable<IRuleViolation>>>();

      SUT = new DummyViolationReporter(TextWriter, ReportAction);
    };

    class when_reporting
    {
      Establish ctx = () =>
      {
        RuleViolations = A.Dummy<IEnumerable<IRuleViolation>>();
      };

      Because of = () => SUT.Report(RuleViolations);

      It calls_report = () =>
          A.CallTo(() => ReportAction(TextWriter, RuleViolations)).MustHaveHappened();

      static IEnumerable<IRuleViolation> RuleViolations;
    }

    class when_disposing
    {
      Establish ctx = () =>
      {
        DisposeAction = A.Fake<Action>();
        TextWriter = new DummyTextWriter(DisposeAction);
        SUT = new DummyViolationReporter(TextWriter, ReportAction);
      };

      Because of = () => SUT.Dispose();

      It disposes_TextWriter = () =>
          A.CallTo(() => DisposeAction()).MustHaveHappened();

      static Action DisposeAction;
    }
    
    class DummyViolationReporter : ViolationReporterBase
    {
      readonly Action<TextWriter, IEnumerable<IRuleViolation>> _report;

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

    class DummyTextWriter : TextWriter
    {
      readonly Action _dispose;

      public DummyTextWriter (Action dispose)
      {
        _dispose = dispose;
      }

      public override Encoding Encoding { get; }

      protected override void Dispose (bool disposing)
      {
        _dispose();
        base.Dispose(disposing);
      }
    }
  }
}