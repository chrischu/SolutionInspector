using System;
using System.Collections.Generic;
using System.IO;
using FakeItEasy;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.Api.Reporting;
using SolutionInspector.Api.Rules;
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

namespace SolutionInspector.Api.Tests.Reporting
{
  [Subject (typeof(XmlViolationReporter))]
  class XmlViolationReporterSpec
  {
    static TextWriter TextWriter;
    static IRuleViolationViewModelConverter RuleViolationViewModelConverter;

    static XmlViolationReporter SUT;

    Establish ctx = () =>
    {
      TextWriter = new StringWriter();
      RuleViolationViewModelConverter = A.Fake<IRuleViolationViewModelConverter>();

      SUT = new XmlViolationReporter(TextWriter, RuleViolationViewModelConverter);
    };

    Cleanup stuff = () => { SUT.Dispose(); };

    class when_reporting
    {
      Establish ctx = () =>
      {
        RuleViolations = A.Dummy<IEnumerable<IRuleViolation>>();
        RuleViolationViewModels = new[]
                                  {
                                      new RuleViolationViewModel(1, "Rule1", "Target1", "Message1"),
                                      new RuleViolationViewModel(2, "Rule2", "Target2", "Message2")
                                  };

        A.CallTo(() => RuleViolationViewModelConverter.Convert(A<IEnumerable<IRuleViolation>>._)).Returns(RuleViolationViewModels);
      };

      Because of = () => SUT.Report(RuleViolations);

      It calls_RuleViolationViewModelConverter = () =>
          A.CallTo(() => RuleViolationViewModelConverter.Convert(RuleViolations)).MustHaveHappened();

      It writes_xml = () =>
          TextWriter.ToString().Should().BeWithDiff(@"
<violations>
  <violation index=""1"" rule=""Rule1"" target=""Target1"">
    <message>Message1</message>
  </violation>
  <violation index=""2"" rule=""Rule2"" target=""Target2"">
    <message>Message2</message>
  </violation>
</violations>".Trim());

      static IEnumerable<IRuleViolation> RuleViolations;
      static IEnumerable<RuleViolationViewModel> RuleViolationViewModels;
    }
  }
}