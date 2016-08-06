using System;
using System.Configuration;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.Api.Configuration;
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

namespace SolutionInspector.Api.Tests.Configuration
{
  [Subject (typeof(ConfigurationElementCollectionBase<>))]
  class ConfigurationElementCollectionBaseSpec
  {
    static DummyConfigurationElementCollection SUT;

    class when_deserializing
    {
      Establish ctx = () => { SUT = new DummyConfigurationElementCollection(); };

      Because of = () => ConfigurationHelper.DeserializeElement(SUT, @"<collection><element value=""a"" /><element value=""b"" /></collection>");

      It deserializes_everything = () =>
          SUT.ShouldAllBeLike(new { Value = "a" }, new { Value = "b" });
    }

    class when_deserializing_unrecognized_element
    {
      Establish ctx = () => { SUT = new DummyConfigurationElementCollection(); };

      Because of = () => Exception = Catch.Exception(
          () => ConfigurationHelper.DeserializeElement(
              SUT,
              @"<collection><element value=""a"" /><UNRECOGNIZED /></collection>"));

      It throws = () =>
          Exception.Should().Be<ConfigurationErrorsException>().WithMessage("Unrecognized element 'UNRECOGNIZED'.");

      static Exception Exception;
    }

    class DummyConfigurationElementCollection : ConfigurationElementCollectionBase<DummyConfigurationElement>
    {
      protected override string ElementName => "element";
    }

    class DummyConfigurationElement : ConfigurationElement
    {
      [ConfigurationProperty ("value")]
      public string Value
      {
        get { return (string) this["value"]; }
        set { this["value"] = value; }
      }
    }
  }
}