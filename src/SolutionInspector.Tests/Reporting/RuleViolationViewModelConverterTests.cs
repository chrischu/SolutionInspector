using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Api.Rules;
using SolutionInspector.Reporting;

namespace SolutionInspector.Tests.Reporting
{
  public class RuleViolationViewModelConverterTests
  {
    private RuleViolationViewModelConverter _sut;

    [SetUp]
    public void SetUp ()
    {
      _sut = new RuleViolationViewModelConverter();
    }

    [Test]
    public void Convert_SingleViolation_Converts ()
    {
      var ruleViolation = RuleViolationObjectMother.Create(new ARule(), "Target", message: "Message");

      // ACT
      var result = _sut.Convert(new[] { ruleViolation });

      // ASSERT
      result.ShouldBeEquivalentTo(
        new[]
        {
          new RuleViolationViewModel(1, "A", "Target", "Message")
        });
    }

    [Test]
    public void Convert_MultipleViolations_ConvertsAndSortsByRuleNameAndTarget ()
    {
      var ruleViolations = new[]
                           {
                             RuleViolationObjectMother.Create(new BRule(), "2"),
                             RuleViolationObjectMother.Create(new BRule(), "1"),
                             RuleViolationObjectMother.Create(new ARule(), "2"),
                             RuleViolationObjectMother.Create(new ARule(), "1")
                           };
      // ACT
      var result = _sut.Convert(ruleViolations);

      // ASSERT
      result.ShouldBeEquivalentTo(
        new[]
        {
          new { Rule = "A", Target = "1" },
          new { Rule = "A", Target = "2" },
          new { Rule = "B", Target = "1" },
          new { Rule = "B", Target = "2" }
        },
        o => o.ExcludingMissingMembers());
    }

    private class ARule : IRule
    {
    }

    private class BRule : IRule
    {
    }
  }
}