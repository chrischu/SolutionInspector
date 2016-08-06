using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using FakeItEasy;
using Machine.Specifications;
using SolutionInspector.Api.Rules;
using SolutionInspector.Reporting;
using SolutionInspector.Utilities;

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
  [Subject (typeof(TableViolationReporter))]
  class TableViolationReporterSpec
  {
    static TextWriter TextWriter;
    static IRuleViolationViewModelConverter RuleViolationViewModelConverter;
    static ITableWriter TableWriter;

    static TableViolationReporter SUT;

    Establish ctx = () =>
    {
      TextWriter = A.Dummy<TextWriter>();
      RuleViolationViewModelConverter = A.Fake<IRuleViolationViewModelConverter>();
      TableWriter = A.Fake<ITableWriter>();

      SUT = new TableViolationReporter(TextWriter, RuleViolationViewModelConverter, TableWriter);
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

      It calls_table_writer = () =>
          A.CallTo(
              () => TableWriter.Write(
                  TextWriter,
                  RuleViolationViewModels,
                  A<Expression<Func<RuleViolationViewModel, object>>[]>.That.Matches(expressions => AssertColumnSelectors(expressions))))
              .MustHaveHappened();

      static bool AssertColumnSelectors (Expression<Func<RuleViolationViewModel, object>>[] expressions)
      {
        var ruleViolationViewModel = RuleViolationViewModels.First();

        return
            expressions.Length == 4 &&
            expressions[0].Compile()(ruleViolationViewModel).Equals(1) &&
            expressions[1].Compile()(ruleViolationViewModel).Equals("Rule1") &&
            expressions[2].Compile()(ruleViolationViewModel).Equals("Target1") &&
            expressions[3].Compile()(ruleViolationViewModel).Equals("Message1");
      }

      static IEnumerable<IRuleViolation> RuleViolations;
      static IEnumerable<RuleViolationViewModel> RuleViolationViewModels;
    }
  }
}