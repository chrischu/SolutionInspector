using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using FluentAssertions;
using Machine.Specifications;
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
// ReSharper disable ClassNeverInstantiated.Global

#endregion

namespace SolutionInspector.Api.Tests.Rules
{
  [Subject (typeof (RuleTypeUtility))]
  class RuleTypeUtilitySpec
  {
    class when_getting_configuration_type_from_non_rule_type
    {
      Because of = () => Exception = Catch.Exception(() => RuleTypeUtility.GetConfigurationType(typeof (object)));

      It throws = () =>
          Exception.Should().BeArgumentException($"Given type '{typeof (object)}' is not a valid rule type.", "ruleType");

      static Exception Exception;
    }

    class when_getting_configuration_type_from_non_configurable_rule_type
    {
      Because of = () => Result = RuleTypeUtility.GetConfigurationType(typeof (Rule));

      It returns_null = () =>
          Result.Should().BeNull();

      static Type Result;
    }

    class when_getting_configuration_type_from_configurable_rule_type
    {
      Because of = () => Result = RuleTypeUtility.GetConfigurationType(typeof (ConfigurableRule));

      It returns_configuration_type = () =>
          Result.Should().Be(typeof (ConfigurableRuleConfiguration));

      static Type Result;
    }

    class when_getting_configuration_type_from_indirectly_configurable_rule_type
    {
      Because of = () => Result = RuleTypeUtility.GetConfigurationType(typeof (IndirectlyConfigurableRule));

      It returns_configuration_type = () =>
          Result.Should().Be(typeof (ConfigurableRuleConfiguration));

      static Type Result;
    }

    class Rule : IRule
    {
    }

    class ConfigurableRule : IConfigurableRule<IRuleTarget, ConfigurableRuleConfiguration>
    {
      public IEnumerable<IRuleViolation> Evaluate (IRuleTarget target)
      {
        return Enumerable.Empty<IRuleViolation>();
      }

      public ConfigurableRuleConfiguration Configuration { get; }
    }

    class SomeIntermediateRule : IConfigurableRule<IRuleTarget, ConfigurableRuleConfiguration>
    {
      public IEnumerable<IRuleViolation> Evaluate (IRuleTarget target)
      {
        return Enumerable.Empty<IRuleViolation>();
      }

      public ConfigurableRuleConfiguration Configuration { get; }
    }

    class IndirectlyConfigurableRule : SomeIntermediateRule
    {
    }

    class ConfigurableRuleConfiguration : ConfigurationElement
    {
    }
  }
}