using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using FakeItEasy;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.Api.ObjectModel;
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

namespace SolutionInspector.DefaultRules.Tests
{
  [Subject(typeof (ProjectXPathRule))]
  class ProjectXPathRuleSpec
  {
    static IProject Project;

    static ProjectXPathRule SUT;

    Establish ctx = () =>
    {
      Project = A.Fake<IProject>();
      A.CallTo(() => Project.ProjectXml).Returns(XDocument.Parse("<xml><element attr=\"7\" /></xml>"));

      SUT = new ProjectXPathRule(
          new ProjectXPathRuleConfiguration
          {
              XPath = "boolean(//element[@attr=7])"
          });
    };

    class when_evaluating_XPath_expression_that_evaluates_to_true
    {
      Establish ctx = () =>
      {
        SUT = new ProjectXPathRule(
          new ProjectXPathRuleConfiguration
          {
            XPath = "boolean(//element[@attr=7])"
          });
      };

      Because of = () => Result = SUT.Evaluate(Project);

      It does_not_return_violations = () =>
          Result.Should().BeEmpty();

      static IEnumerable<IRuleViolation> Result;
    }

    class when_evaluating_XPath_expression_that_evaluates_to_false
    {
      Establish ctx = () =>
      {
        SUT = new ProjectXPathRule(
          new ProjectXPathRuleConfiguration
          {
            XPath = "boolean(//nonExistingElement)"
          });
      };

      Because of = () => Result = SUT.Evaluate(Project);

      It returns_violation = () =>
          Result.ShouldAllBeEquivalentTo(
              new RuleViolation(SUT, Project, $"The XPath expression '{SUT.Configuration.XPath}' did not evaluate to 'true', but to 'false'."));

      static IEnumerable<IRuleViolation> Result;
    }

    class when_evaluating_XPath_expression_that_does_not_evaluate_to_boolean
    {
      Establish ctx = () =>
      {
        SUT = new ProjectXPathRule(
          new ProjectXPathRuleConfiguration
          {
            XPath = "//element"
          });
      };

      Because of = () => Exception = Catch.Exception(() => SUT.Evaluate(Project).ToArray());

      It throws = () =>
          Exception.Should().Be<ProjectXPathRule.InvalidXPathExpressionException>()
              .WithMessage($"The configured XPath expression '{SUT.Configuration.XPath}' does not evaluate to a boolean value.");

      static Exception Exception;
    }
  }
}