using System;
using System.Xml.Linq;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Configuration.Validation;

namespace SolutionInspector.Configuration.Tests.Validation
{
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

    private class DummyConfigurationElement : ConfigurationElement
    {
      [ConfigurationValue (IsOptional = false)]
      // ReSharper disable once UnusedMember.Local
      public string Value { get; set; }
    }
  }
}