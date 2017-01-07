using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using FakeItEasy;
using NUnit.Framework;
using SolutionInspector.Api.Rules;
using SolutionInspector.Reporting;
using SolutionInspector.Utilities;

namespace SolutionInspector.Tests.Reporting
{
  public class TableViolationReporterTests
  {
    private IRuleViolationViewModelConverter _ruleViolationViewModelConverter;

    private TableViolationReporter _sut;
    private ITableWriter _tableWriter;
    private TextWriter _textWriter;

    [SetUp]
    public void SetUp ()
    {
      _textWriter = A.Dummy<TextWriter>();
      _ruleViolationViewModelConverter = A.Fake<IRuleViolationViewModelConverter>();
      _tableWriter = A.Fake<ITableWriter>();

      _sut = new TableViolationReporter(_textWriter, _ruleViolationViewModelConverter, _tableWriter);
    }

    [TearDown]
    public void TearDown ()
    {
      _sut.Dispose();
    }

    [Test]
    public void Report ()
    {
      var ruleViolations = A.Dummy<IReadOnlyCollection<IRuleViolation>>();
      var ruleViolationViewModels = new[]
                                    {
                                      new RuleViolationViewModel(1, "Rule1", "Target1", "Message1"),
                                      new RuleViolationViewModel(2, "Rule2", "Target2", "Message2")
                                    };

      A.CallTo(() => _ruleViolationViewModelConverter.Convert(ruleViolations)).Returns(ruleViolationViewModels);

      // ACT
      _sut.Report(ruleViolations);

      // ASSERT
      A.CallTo(() => _ruleViolationViewModelConverter.Convert(ruleViolations)).MustHaveHappened();

      A.CallTo(
            () => _tableWriter.Write(
              _textWriter,
              ruleViolationViewModels,
              A<Expression<Func<RuleViolationViewModel, object>>[]>.That.Matches(
                expressions => AssertColumnSelectors(ruleViolationViewModels[0], expressions))))
          .MustHaveHappened();
    }

    private bool AssertColumnSelectors (
      RuleViolationViewModel ruleViolationViewModel,
      Expression<Func<RuleViolationViewModel, object>>[] expressions)
    {
      return
          expressions.Length == 4 &&
          expressions[0].Compile()(ruleViolationViewModel).Equals(1) &&
          expressions[1].Compile()(ruleViolationViewModel).Equals("Rule1") &&
          expressions[2].Compile()(ruleViolationViewModel).Equals("Target1") &&
          expressions[3].Compile()(ruleViolationViewModel).Equals("Message1");
    }
  }
}