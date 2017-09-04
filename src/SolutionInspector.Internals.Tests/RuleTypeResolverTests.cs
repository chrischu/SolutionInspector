using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Api.Rules;
using SolutionInspector.Commons.Extensions;
using SolutionInspector.Configuration;
using SolutionInspector.TestInfrastructure;

namespace SolutionInspector.Internals.Tests
{
  public class RuleTypeResolverTests
  {
    private IRuleTypeResolver _sut;

    [SetUp]
    public void SetUp ()
    {
      _sut = new RuleTypeResolver();
    }

    [Test]
    public void Resolve_NonConfigurableRuleContainedInAssembly_ResolvesCorrectly ()
    {
      // ACT
      var result = _sut.Resolve(typeof(Rule).AssemblyQualifiedName.AssertNotNull());

      // ASSERT
      result.RuleType.Should().Be(typeof(Rule));
      result.IsConfigurable.Should().BeFalse();
      result.ConfigurationType.Should().BeNull();
      result.Constructor.Should().BeSameAs(typeof(Rule).GetConstructors().Single());
    }

    [Test]
    public void Resolve_ConfigurableRuleContainedInAssembly_ResolvesCorrectly ()
    {
      // ACT
      var result = _sut.Resolve(typeof(ConfigurableRule).AssemblyQualifiedName.AssertNotNull());

      // ASSERT
      result.RuleType.Should().Be(typeof(ConfigurableRule));
      result.IsConfigurable.Should().BeTrue();
      result.ConfigurationType.Should().Be(typeof(ConfigurableRuleConfiguration));
      result.Constructor.Should().BeSameAs(typeof(ConfigurableRule).GetConstructors().Single());
    }

    [Test]
    public void Resolve_RuleContainedInAssemblyWithoutPublicConstructor_Throws ()
    {
      // ACT
      Action act = () => Dev.Null = _sut.Resolve(typeof(RuleWithoutPublicCtor).AssemblyQualifiedName.AssertNotNull());

      // ASSERT
      act.ShouldThrow<RuleTypeResolvingException>()
          .WithMessage("The rule type 'RuleWithoutPublicCtor' does not provide a public constructor.");
    }

    [Test]
    public void Resolve_NonConfigurableRuleContainedInAssemblyWithMultipleConstructors_ResolvesCorrectly ()
    {
      // ACT
      var result = _sut.Resolve(typeof(RuleWithMultipleCtors).AssemblyQualifiedName.AssertNotNull());

      // ASSERT
      result.RuleType.Should().Be(typeof(RuleWithMultipleCtors));
      result.IsConfigurable.Should().BeFalse();
      result.ConfigurationType.Should().BeNull();
      result.Constructor.Should().BeSameAs(typeof(RuleWithMultipleCtors).GetConstructors().Single(c => c.GetParameters().Length == 0));
    }

    [Test]
    public void Resolve_ConfigurableRuleContainedInAssemblyWithMultipleConstructors_ResolvesCorrectly ()
    {
      // ACT
      var result = _sut.Resolve(typeof(ConfigurableRuleWithMultipleCtors).AssemblyQualifiedName.AssertNotNull());

      // ASSERT
      result.RuleType.Should().Be(typeof(ConfigurableRuleWithMultipleCtors));
      result.IsConfigurable.Should().BeTrue();
      result.ConfigurationType.Should().Be(typeof(ConfigurableRuleConfiguration));
      result.Constructor.Should().BeSameAs(typeof(ConfigurableRuleWithMultipleCtors).GetConstructors().Single(c => c.GetParameters().Length == 1));
    }

    [Test]
    public void Resolve_ConfigurableRuleContainedInAssemblyWithoutCorrectConstructor_Throws ()
    {
      // ACT
      Action act = () => Dev.Null = _sut.Resolve(typeof(ConfigurableRuleWithoutCorrectCtor).AssemblyQualifiedName.AssertNotNull());

      // ASSERT
      act.ShouldThrow<RuleTypeResolvingException>()
          .WithMessage(
            "The rule type 'ConfigurableRuleWithoutCorrectCtor' does not provide a public constructor only " +
            "taking a parameter of type 'ConfigurableRuleConfiguration' as a parameter.");
    }

    [Test]
    public void Resolve_NonExistingRule_Throws ()
    {
      // ACT
      Action act = () => Dev.Null = _sut.Resolve("DoesNotExist");

      // ASSERT
      act.ShouldThrow<RuleTypeResolvingException>().WithMessage("Could not resolve rule type 'DoesNotExist'.");
    }

    [Test]
    public void Resolve_TypeThatDoesNotImplementIRule_Throws ()
    {
      // ACT
      Action act = () => Dev.Null = _sut.Resolve(typeof(string).AssemblyQualifiedName.AssertNotNull());

      // ASSERT
      act.ShouldThrow<RuleTypeResolvingException>().WithMessage("The type 'String' is not a valid rule type.");
    }

    // ReSharper disable UnusedMember.Local
    private class Rule : IRule
    {
    }

    private class RuleWithoutPublicCtor : IRule
    {
      private RuleWithoutPublicCtor ()
      {
      }
    }

    private class RuleWithMultipleCtors : IRule
    {
      public RuleWithMultipleCtors ()
      {
      }

      // ReSharper disable once UnusedParameter.Local
      public RuleWithMultipleCtors (object dummyParameter)
      {
      }
    }

    private class ConfigurableRule : ConfigurableRule<IRuleTarget, ConfigurableRuleConfiguration>
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

    private class ConfigurableRuleWithoutCorrectCtor : IConfigurableRule<IRuleTarget, ConfigurableRuleConfiguration>
    {
      public IEnumerable<IRuleViolation> Evaluate (IRuleTarget target)
      {
        return Enumerable.Empty<IRuleViolation>();
      }

      // ReSharper disable once UnassignedGetOnlyAutoProperty
      public ConfigurableRuleConfiguration Configuration { get; }
    }

    private class ConfigurableRuleWithMultipleCtors : ConfigurableRule<IRuleTarget, ConfigurableRuleConfiguration>
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

    private class ConfigurableRuleConfiguration : ConfigurationElement
    {
    }
  }
}