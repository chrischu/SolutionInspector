using System;
using System.Xml.Linq;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Configuration.Attributes;
using SolutionInspector.Configuration.Validation;
using SolutionInspector.TestInfrastructure.AssertionExtensions;

namespace SolutionInspector.Configuration.Tests.Validation
{
  [TestFixture]
  public class ConfigurationValidatorTests
  {
    [Test]
    public void Validate_ValidConfigurationElement_DoesNotThrow ()
    {
      var configurationElement = ConfigurationElement.Load<DummyConfigurationElement>(XElement.Parse("<element value=\"A\" />"));

      // ACT
      Action act = () => ConfigurationValidator.Validate(configurationElement);

      // ASSERT
      act.ShouldNotThrow();
    }

    [Test]
    public void Validate_InvalidConfigurationElement_Throws ()
    {
      var configurationElement = ConfigurationElement.Create<DummyConfigurationElement>("element");

      // ACT
      Action act = () => ConfigurationValidator.Validate(configurationElement);

      // ASSERT
      act.ShouldThrow<ConfigurationValidationException>();
    }

    [Test]
    public void Validate_WithInvalidType_Throws ()
    {
      // ACT
      Action act = () => ConfigurationValidator.Validate(typeof(string));

      // ASSERT
      act.ShouldThrowArgumentException($"The given type '{typeof(string)}' does not derive from {typeof(ConfigurationBase)}.", "configurationType");
    }

    private class DummyConfigurationElement : ConfigurationElement
    {
      [ConfigurationValue (IsOptional = false)]
      // ReSharper disable once UnusedMember.Local
      public string Value { get; set; }
    }
  }
}