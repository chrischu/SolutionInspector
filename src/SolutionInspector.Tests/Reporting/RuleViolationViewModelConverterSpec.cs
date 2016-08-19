using System.Collections.Generic;
using System.Linq;
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
  [Subject (typeof(RuleViolationViewModelConverter))]
  class RuleViolationViewModelConverterSpec
  {
    static RuleViolationViewModelConverter SUT;

    Establish ctx = () => { SUT = new RuleViolationViewModelConverter(); };

    class when_converting_one_violation
    {
      Establish ctx = () => { RuleViolation = RuleViolationObjectMother.Create(new ARule(), "Target", message: "Message"); };

      Because of = () => Result = SUT.Convert(new[] { RuleViolation });

      It converts = () =>
          Result.Single().Should().BeLike(new RuleViolationViewModel(1, "A", "Target", "Message"));

      static IRuleViolation RuleViolation;
      static IEnumerable<RuleViolationViewModel> Result;
    }

    class when_converting_multiple_violations
    {
      Establish ctx = () =>
      {
        RuleViolations = new[]
                         {
                             RuleViolationObjectMother.Create(new BRule(), "2"),
                             RuleViolationObjectMother.Create(new BRule(), "1"),
                             RuleViolationObjectMother.Create(new ARule(), "2"),
                             RuleViolationObjectMother.Create(new ARule(), "1")
                         };
      };

      Because of = () => Result = SUT.Convert(RuleViolations);

      It converts_and_sorts = () =>
          Result.ShouldAllBeLike(
              new { Rule = "A", Target = "1" },
              new { Rule = "A", Target = "2" },
              new { Rule = "B", Target = "1" },
              new { Rule = "B", Target = "2" });

      static IEnumerable<IRuleViolation> RuleViolations;
      static IEnumerable<RuleViolationViewModel> Result;
    }

    class ARule : IRule
    {
    }

    class BRule : IRule
    {
    }
  }
}