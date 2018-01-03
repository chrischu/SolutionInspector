using System;
using System.Xml.Linq;
using FluentAssertions;
using JetBrains.Annotations;
using NUnit.Framework;
using SolutionInspector.Api.Configuration.Ruleset;
using SolutionInspector.Api.Rules;
using SolutionInspector.Commons.Extensions;
using SolutionInspector.Configuration;

namespace SolutionInspector.Internals.Tests
{
  public class RuleInstantiatorTests
  {
    private IRuleInstantiator _sut;

    [SetUp]
    public void SetUp ()
    {
      _sut = new RuleInstantiator();
    }

    [Test]
    public void Instantiate_InstantiateRuleConfigurationAndSetsProperties ()
    {
      // ACT
      var result = _sut.Instantiate(
          typeof(Rule).AssemblyQualifiedName.AssertNotNull(),
          XElement.Parse(@"<rule type=""DONT CARE"" property=""value"" />"));

      // ASSERT
      result.Should().BeOfType<Rule>().Which.Property.Should().Be("value");
    }

    [Test]
    public void Resolve_NonExistingRule_Throws ()
    {
      // ACT
      Action act = () => _sut.Instantiate("DoesNotExist", new XElement("rule"));

      // ASSERT
      act.ShouldThrow<RuleInstantiationException>().WithMessage("Could not resolve rule type 'DoesNotExist'.");
    }

    [Test]
    public void Resolve_TypeThatDoesNotImplementIRule_Throws ()
    {
      // ACT
      Action act = () => _sut.Instantiate(typeof(string).AssemblyQualifiedName.AssertNotNull(), new XElement("rule"));

      // ASSERT
      act.ShouldThrow<RuleInstantiationException>().WithMessage("The type 'String' is not a valid rule type.");
    }

    // ReSharper disable UnusedMember.Local
    private class Rule : RuleConfigurationElement, IRule
    {
      [CanBeNull]
      [ConfigurationValue]
      public string Property => GetConfigurationValue<string>();
    }
  }
}