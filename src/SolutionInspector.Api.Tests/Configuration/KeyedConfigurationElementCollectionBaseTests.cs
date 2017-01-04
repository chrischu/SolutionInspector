using System;
using System.Configuration;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Api.Configuration;
using SolutionInspector.TestInfrastructure.Configuration;

namespace SolutionInspector.Api.Tests.Configuration
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