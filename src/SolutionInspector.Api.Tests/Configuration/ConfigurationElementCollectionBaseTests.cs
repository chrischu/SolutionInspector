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
    public void Deserializing ()
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

    [Test]
    public void Contains_WithNonAddedKey_ReturnsFalse ()
    {
      var collection = new DummyConfigurationElementCollection();
      var element = new DummyConfigurationElement();

      // ACT
      var result = collection.Contains(element);

      // ASSERT
      result.Should().BeFalse();
    }

    [Test]
    public void Contains_WithAddedKey_ReturnsTrue ()
    {
      var collection = new DummyConfigurationElementCollection();
      var element = new DummyConfigurationElement();
      collection.Add(element);

      // ACT
      var result = collection.Contains(element);

      // ASSERT
      result.Should().BeTrue();
    }

    [Test]
    public void Remove ()
    {
      var collection = new DummyConfigurationElementCollection();
      var element = new DummyConfigurationElement();
      collection.Add(element);

      // ACT
      collection.Remove(element);

      // ASSERT
      collection.Contains(element).Should().BeFalse();
    }

    [Test]
    public void Clear ()
    {
      var collection = new DummyConfigurationElementCollection { new DummyConfigurationElement() };

      // ACT
      collection.Clear();

      // ASSERT
      collection.Count.Should().Be(0);
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
        get => (string) this["value"];
        set => this["value"] = value;
      }
    }
  }
}