using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Api.Rules;
using SolutionInspector.TestInfrastructure.AssertionExtensions;

namespace SolutionInspector.Api.Tests.Rules
{
  public class RuleTypeUtilityTests
  {
    [Test]
    public void GetConfigurationType_FromNonRuleType_Throws ()
    {
      // ACT
      Action act = () => RuleTypeUtility.GetConfigurationType(typeof(object));

      // ASSERT
      act.ShouldThrowArgumentException($"Given type '{typeof(object)}' is not a valid rule type.", "ruleType");
    }

    [Test]
    public void GetConfigurationType_FromNonConfigurableRuleType_ReturnsNull ()
    {
      // ACT
      var result = RuleTypeUtility.GetConfigurationType(typeof(Rule));

      // ASSERT
      result.Should().BeNull();
    }

    [Test]
    public void GetConfigurationType_FromConfigurableRuleType_ReturnsConfigurationType ()
    {
      // ACT
      var result = RuleTypeUtility.GetConfigurationType(typeof(ConfigurableRule));

      // ASSERT
      result.Should().Be(typeof(ConfigurableRuleConfiguration));
    }

    [Test]
    public void GetConfigurationType_FromIndirectlyConfigurableRuleType_ReturnsConfigurationType ()
    {
      // ACT
      var result = RuleTypeUtility.GetConfigurationType(typeof(IndirectlyConfigurableRule));

      // ASSERT
      result.Should().Be(typeof(ConfigurableRuleConfiguration));
    }

    private class Rule : IRule
    {
    }

    private class ConfigurableRule : IConfigurableRule<IRuleTarget, ConfigurableRuleConfiguration>
    {
      public IEnumerable<IRuleViolation> Evaluate (IRuleTarget target)
      {
        return Enumerable.Empty<IRuleViolation>();
      }

      // ReSharper disable once UnassignedGetOnlyAutoProperty
      public ConfigurableRuleConfiguration Configuration { get; }
    }

    private class SomeIntermediateRule : IConfigurableRule<IRuleTarget, ConfigurableRuleConfiguration>
    {
      public IEnumerable<IRuleViolation> Evaluate (IRuleTarget target)
      {
        return Enumerable.Empty<IRuleViolation>();
      }

      // ReSharper disable once UnassignedGetOnlyAutoProperty
      public ConfigurableRuleConfiguration Configuration { get; }
    }

    private class IndirectlyConfigurableRule : SomeIntermediateRule
    {
    }

    private class ConfigurableRuleConfiguration : ConfigurationElement
    {
    }
  }
}