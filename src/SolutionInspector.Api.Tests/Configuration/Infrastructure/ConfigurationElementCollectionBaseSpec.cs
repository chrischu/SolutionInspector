using System;
using System.Configuration;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.Api.Configuration.Infrastructure;
using SolutionInspector.TestInfrastructure.AssertionExtensions;
using SolutionInspector.TestInfrastructure.Configuration;

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

#endregion

namespace SolutionInspector.Api.Tests.Configuration.Infrastructure
{
  [Subject (typeof(KeyedConfigurationElementCollectionBase<,>))]
  class ConfigurationElementCollectionBaseSpec
  {
    static DummyConfigurationElementCollection SUT;

    class when_adding_two_elements_with_the_same_key
    {
      Establish ctx = () => { SUT = new DummyConfigurationElementCollection(); };

      Because of = () =>
          Exception = Catch.Exception(() => ConfigurationHelper.DeserializeElement(SUT, @"<collection><add key=""a"" /><add key=""a"" />"));

      It throws = () =>
          Exception.Should().Be<ConfigurationErrorsException>()
              .WithMessage(
                  "The value for the property 'key' is not valid. " +
                  "The error is: The key 'a' was already added to the collection once.");

      static Exception Exception;
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