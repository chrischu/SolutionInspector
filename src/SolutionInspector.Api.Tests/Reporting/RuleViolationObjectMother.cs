using System;
using FakeItEasy;
using SolutionInspector.Api.Rules;
using SolutionInspector.TestInfrastructure;

namespace SolutionInspector.Api.Tests.Reporting
{
  internal static class RuleViolationObjectMother
  {
    public static IRuleViolation Create (IRule rule = null, string target = null, string targetPath = null, string message = null)
    {
      var ruleViolation = A.Fake<IRuleViolation>();

      A.CallTo(() => ruleViolation.Rule).Returns(rule ?? new SomeRule());
      A.CallTo(() => ruleViolation.Target.Identifier).Returns(target ?? Some.String());
      A.CallTo(() => ruleViolation.Target.FullPath).Returns(targetPath ?? Some.String());
      A.CallTo(() => ruleViolation.Message).Returns(message ?? Some.String());

      return ruleViolation;
    }

    class SomeRule : IRule
    {
    }
  }
}