using System;
using System.Configuration;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.TestInfrastructure.Configuration;

namespace SolutionInspector.Api.Configuration.Tests
{
  public class KeyedConfigurationElementCollectionBaseTests
  {
    [Test]
    public void Deserialize_TwoElementsWithSameKey_Throws ()
    {
      var collection = new DummyConfigurationElementCollection();

      // ACT
      Action act = () => ConfigurationHelper.DeserializeElement(collection, @"<collection><add key=""a"" /><add key=""a"" /></collection>");

      // ASSERT
      act.ShouldThrow<ConfigurationErrorsException>().WithMessage(
        "The value for the property 'key' is not valid. " +
        "The error is: The key 'a' was already added to the collection once.");
    }

    [Test]
    public void Contains_WithNonAddedKey_ReturnsFalse ()
    {
      var collection = new DummyConfigurationElementCollection();
      var element = new DummyConfigurationElement();

      // ACT
      var result = collection.Contains(element.Key);
      
      // ASSERT
      result.Should().BeFalse();
    }

    [Test]
    public void Contains_WithAddedKey_ReturnsTrue()
    {
      var collection = new DummyConfigurationElementCollection();
      var element = new DummyConfigurationElement();
      collection.Add(element);

      // ACT
      var result = collection.Contains(element.Key);

      // ASSERT
      result.Should().BeTrue();
    }

    [Test]
    public void Remove()
    {
      var collection = new DummyConfigurationElementCollection();
      var element = new DummyConfigurationElement();
      collection.Add(element);

      // ACT
      collection.Remove(element.Key);

      // ASSERT
      collection.Contains(element.Key).Should().BeFalse();
    }

    class DummyConfigurationElementCollection : KeyedConfigurationElementCollectionBase<DummyConfigurationElement, string>
    {
      protected override string ElementName => "add";
    }

    class DummyConfigurationElement : KeyedConfigurationElement<string>
    {
      [ConfigurationProperty ("key")]
      public new string Key => base.Key;

      public override string KeyName => "key";
    }
  }
}