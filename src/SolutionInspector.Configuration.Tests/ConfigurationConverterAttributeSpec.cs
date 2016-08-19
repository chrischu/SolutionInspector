using System;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.TestInfrastructure.AssertionExtensions;

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

namespace SolutionInspector.Configuration.Tests
{
  [Subject (typeof(ConfigurationConverterAttribute))]
  class ConfigurationConverterAttributeSpec
  {
    class when_creating_with_non_IConfigurationConverter_type
    {
      Because of = () => Exception = Catch.Exception(() => new ConfigurationConverterAttribute(typeof(string)));

      It throws = () =>
        Exception.Should()
            .BeArgumentException($"The given type must derive from '{typeof(IConfigurationConverter)}'.", "configurationConverterType");

      static Exception Exception;
    }

    class when_creating_with_type_without_default_ctor
    {
      Because of = () => Exception = Catch.Exception(() => new ConfigurationConverterAttribute(typeof(DummyConverter)));

      It throws = () =>
        Exception.Should()
            .BeArgumentException("The given type must provide a public default constructor.", "configurationConverterType");

      static Exception Exception;
    }

    class DummyConverter : IConfigurationConverter
    {
      // ReSharper disable once UnusedParameter.Local
      public DummyConverter (int x)
      {
        
      }
    }
  }
}