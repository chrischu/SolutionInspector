using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.Api.Rules;
using SolutionInspector.Reporting;
using SolutionInspector.TestInfrastructure.AssertionExtensions;

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

namespace SolutionInspector.Tests.Reporting
{
  [Subject (typeof(VisualStudioViolationReporter))]
  class VisualStudioViolationReporterSpec
  {
    static TextWriter TextWriter;

    static VisualStudioViolationReporter SUT;

    Establish ctx = () =>
    {
      TextWriter = new StringWriter();

      SUT = new VisualStudioViolationReporter(TextWriter);
    };

    Cleanup stuff = () => { SUT.Dispose(); };

    class when_reporting
    {
      Establish ctx = () =>
      {
        RuleViolations = new[]
                         {
                             RuleViolationObjectMother.Create(new SomeRule(), targetPath: "TP1", message: "MSG1"),
                             RuleViolationObjectMother.Create(new SomeRule(), targetPath: "TP2", message: "MSG2")
                         };
      };

      Because of = () => SUT.Report(RuleViolations);

      It reports = () =>
          TextWriter.ToString().Should().BeWithDiff(@"
TP1: SolutionInspector warning SI0000: MSG1 (Some)
TP2: SolutionInspector warning SI0000: MSG2 (Some)");

      static IEnumerable<IRuleViolation> RuleViolations;
    }

    class SomeRule : IRule
    {
    }
  }
}