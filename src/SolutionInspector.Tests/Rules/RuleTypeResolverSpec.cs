using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.Api.Extensions;
using SolutionInspector.Api.Rules;
using SolutionInspector.Rules;
using SolutionInspector.TestInfrastructure.AssertionExtensions;

#region R# preamble for Machine.Specifications files

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

namespace SolutionInspector.Tests.Rules
{
  [Subject (typeof(RuleTypeResolver))]
  class RuleTypeResolverSpec
  {
    static IRuleTypeResolver SUT;

    Establish ctx = () => { SUT = new RuleTypeResolver(); };

    class when_resolving_rule_contained_in_assembly
    {
      Because of = () => Result = SUT.Resolve(typeof(Rule).AssemblyQualifiedName.AssertNotNull());

      It returns_rule_type = () =>
          Result.RuleType.Should().Be(typeof(Rule));

      It returns_null_for_configuration_type = () =>
          Result.ConfigurationType.Should().BeNull();

      It retrieves_constructor = () =>
          Result.Constructor.Should().BeSameAs(typeof(Rule).GetConstructors().Single());

      static RuleTypeInfo Result;
    }

    class when_resolving_rule_contained_in_assembly_without_public_constructor
    {
      Because of = () => Exception = Catch.Exception(() => SUT.Resolve(typeof(RuleWithoutPublicCtor).AssemblyQualifiedName.AssertNotNull()));

      It throws = () =>
          Exception.Should()
              .Be<RuleTypeResolvingException>()
              .WithMessage("The rule type 'RuleWithoutPublicCtor' does not provide a public constructor.");

      static Exception Exception;
    }

    class when_resolving_rule_contained_in_assembly_with_multiple_constructors
    {
      Because of = () => Result = SUT.Resolve(typeof(RuleWithMultipleCtors).AssemblyQualifiedName.AssertNotNull());

      It returns_rule_type = () =>
          Result.RuleType.Should().Be(typeof(RuleWithMultipleCtors));

      It returns_null_for_configuration_type = () =>
          Result.ConfigurationType.Should().BeNull();

      It retrieves_constructor = () =>
          Result.Constructor.Should().BeSameAs(typeof(RuleWithMultipleCtors).GetConstructors().Single(c => c.GetParameters().Length == 0));

      static RuleTypeInfo Result;
    }

    class when_resolving_configurable_rule_contained_in_assembly
    {
      Because of = () => Result = SUT.Resolve(typeof(ConfigurableRule).AssemblyQualifiedName.AssertNotNull());

      It returns_rule_type = () =>
          Result.RuleType.Should().Be(typeof(ConfigurableRule));

      It returns_configuration_type = () =>
          Result.ConfigurationType.Should().Be(typeof(ConfigurableRuleConfiguration));

      It retrieves_constructor = () =>
          Result.Constructor.Should().BeSameAs(typeof(ConfigurableRule).GetConstructors().Single());

      static RuleTypeInfo Result;
    }

    class when_resolving_configurable_rule_contained_in_assembly_without_correct_constructor
    {
      Because of =
          () => Exception = Catch.Exception(() => SUT.Resolve(typeof(ConfigurableRuleWithoutCorrectCtor).AssemblyQualifiedName.AssertNotNull()));

      It throws = () =>
          Exception.Should()
              .Be<RuleTypeResolvingException>()
              .WithMessage(
                  "The rule type 'ConfigurableRuleWithoutCorrectCtor' does not provide a public constructor only " +
                  "taking a parameter of type 'ConfigurableRuleConfiguration' as a parameter.");

      static Exception Exception;
    }

    class when_resolving_configurable_rule_contained_in_assembly_with_multiple_constructors
    {
      Because of = () => Result = SUT.Resolve(typeof(ConfigurableRuleWithMultipleCtors).AssemblyQualifiedName.AssertNotNull());

      It returns_rule_type = () =>
          Result.RuleType.Should().Be(typeof(ConfigurableRuleWithMultipleCtors));

      It returns_configuration_type = () =>
          Result.ConfigurationType.Should().Be(typeof(ConfigurableRuleConfiguration));

      It retrieves_correct_constructor = () =>
          Result.Constructor.Should()
              .BeSameAs(typeof(ConfigurableRuleWithMultipleCtors).GetConstructors().Single(c => c.GetParameters().Length == 1));

      static RuleTypeInfo Result;
    }

    class when_resolving_non_existing_rule
    {
      Because of = () => Exception = Catch.Exception(() => SUT.Resolve("DoesNotExist"));

      It throws = () =>
          Exception.Should()
              .Be<RuleTypeResolvingException>()
              .WithMessage("Could not resolve rule type 'DoesNotExist'.");

      static Exception Exception;
    }

    class when_resolving_rule_that_does_not_implement_IRule
    {
      Because of = () => Exception = Catch.Exception(() => SUT.Resolve(typeof(string).AssemblyQualifiedName.AssertNotNull()));

      It throws = () =>
          Exception.Should().Be<RuleTypeResolvingException>().WithMessage("The type 'String' is not a valid rule type.");

      static Exception Exception;
    }

    class Rule : IRule
    {
    }

    class RuleWithoutPublicCtor : IRule
    {
      private RuleWithoutPublicCtor ()
      {
      }
    }

    class RuleWithMultipleCtors : IRule
    {
      public RuleWithMultipleCtors ()
      {
      }

      // ReSharper disable once UnusedParameter.Local
      public RuleWithMultipleCtors (object dummyParameter)
      {
      }
    }

    class ConfigurableRule : ConfigurableRule<IRuleTarget, ConfigurableRuleConfiguration>
    {
      public ConfigurableRule (ConfigurableRuleConfiguration configuration)
          : base(configuration)
      {
      }

      public override IEnumerable<IRuleViolation> Evaluate (IRuleTarget target)
      {
        return Enumerable.Empty<IRuleViolation>();
      }
    }

    class ConfigurableRuleWithoutCorrectCtor : IConfigurableRule<IRuleTarget, ConfigurableRuleConfiguration>
    {
      public IEnumerable<IRuleViolation> Evaluate (IRuleTarget target)
      {
        return Enumerable.Empty<IRuleViolation>();
      }

      public ConfigurableRuleConfiguration Configuration { get; }
    }

    class ConfigurableRuleWithMultipleCtors : ConfigurableRule<IRuleTarget, ConfigurableRuleConfiguration>
    {
      public ConfigurableRuleWithMultipleCtors (ConfigurableRuleConfiguration configuration)
          : base(configuration)
      {
      }

      // ReSharper disable once UnusedParameter.Local
      public ConfigurableRuleWithMultipleCtors (ConfigurableRuleConfiguration configuration, object dummyParameter)
          : base(configuration)
      {
      }

      public override IEnumerable<IRuleViolation> Evaluate (IRuleTarget target)
      {
        return Enumerable.Empty<IRuleViolation>();
      }
    }

    class ConfigurableRuleConfiguration : ConfigurationElement
    {
    }
  }

  namespace Same.Same.Different
  {
    class Rule1 : IRule
    {
    }
  }

  namespace Same.Same.Different2
  {
    class Rule2 : IRule
    {
    }
  }
}