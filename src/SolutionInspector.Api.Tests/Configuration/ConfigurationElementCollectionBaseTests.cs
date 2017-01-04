using System;
using System.Configuration;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Api.Configuration;
using SolutionInspector.TestInfrastructure.Configuration;

namespace SolutionInspector.Api.Tests.Configuration
{
  public class ConfigurationElementCollectionBaseTests
  {
    [Test]
    public void Deserializing()
    {
      var collection = new DummyConfigurationElementCollection();

      // ACT
      ConfigurationHelper.DeserializeElement(collection, @"<collection><element value=""a"" /><element value=""b"" /></collection>");

      // ASSERT
      collection.ShouldBeEquivalentTo(
        new[]
        {
          new { Value = "a" }, new { Value = "b" }
        },
        o => o.ExcludingMissingMembers());
    }

    [Test]
    public void Deserializing_WithUnrecognizedElement_Throws ()
    {
      var collection = new DummyConfigurationElementCollection();

      // ACT
      Action act = () => ConfigurationHelper.DeserializeElement(collection, @"<collection><element value=""a"" /><UNRECOGNIZED /></collection>");

      // ASSERT
      act.ShouldThrow<ConfigurationErrorsException>().WithMessage("Unrecognized element 'UNRECOGNIZED'.");
    }

    private class DummyConfigurationElementCollection : ConfigurationElementCollectionBase<DummyConfigurationElement>
    {
      protected override string ElementName => "element";
    }

    private class DummyConfigurationElement : ConfigurationElement
    {
      [ConfigurationProperty ("value")]
      // ReSharper disable once UnusedMember.Local
      public string Value
      {
        get { return (string) this["value"]; }
        set { this["value"] = value; }
      }
    }
  }
}