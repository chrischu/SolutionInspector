using System;
using FluentAssertions;
using JetBrains.Annotations;
using Machine.Specifications;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Utilities;
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

namespace SolutionInspector.Api.Tests.Utilities
{
  [Subject (typeof(BuildConfigurationFilterConverter))]
  class BuildConfigurationFilterConverterSpec
  {
    static BuildConfigurationFilterConverter SUT;

    Establish ctx = () => { SUT = new BuildConfigurationFilterConverter(); };

    class when_converting_to
    {
      Establish ctx =
          () => { BuildConfigurationFilter = new BuildConfigurationFilter(new BuildConfiguration("A", "B"), new BuildConfiguration("C", "D")); };

      Because of = () => Result = ConvertTo(BuildConfigurationFilter);

      It converts = () =>
          Result.Should().Be("A|B,C|D");

      static BuildConfigurationFilter BuildConfigurationFilter;
      static string Result;
    }

    class when_converting_to_with_null_value
    {
      Because of = () => Result = ConvertTo(null);

      It returns_null = () =>
          Result.Should().BeNull();

      static string Result;
    }

    class when_converting_to_with_non_BuildConfigurationFilter
    {
      Because of = () => Exception = Catch.Exception(() => ConvertTo("NOT A NAME FILTER"));

      It throws = () =>
          Exception.Should().BeArgumentException(
              $"Unsupported type '{typeof(string).FullName}', expected type '{typeof(BuildConfigurationFilter).FullName}'.",
              "value");

      static Exception Exception;
    }

    class when_converting_from
    {
      Establish ctx = () =>
      {
        BuildConfigurationFilter = new BuildConfigurationFilter(new BuildConfiguration("A", "B"), new BuildConfiguration("C", "D"));
        BuildConfigurationFilterString = ConvertTo(BuildConfigurationFilter);
      };

      Because of = () => Result = ConvertFrom(BuildConfigurationFilterString);

      It returns_original_BuildConfigurationFilter = () =>
          Result.ToString().Should().Be(BuildConfigurationFilter.ToString());

      static BuildConfigurationFilter BuildConfigurationFilter;
      static string BuildConfigurationFilterString;
      static BuildConfigurationFilter Result;
    }

    class when_converting_from_with_null_value
    {
      Because of = () => Result = ConvertFrom(null);

      It returns_null = () =>
          Result.Should().BeNull();

      static BuildConfigurationFilter Result;
    }

    class when_converting_from_with_non_string
    {
      Because of = () => Exception = Catch.Exception(() => ConvertFrom(1));

      It throws = () =>
          Exception.Should().BeArgumentException(
              $"Unsupported type '{typeof(int).FullName}', expected type '{typeof(string).FullName}'.",
              "data");

      static Exception Exception;
    }

    static string ConvertTo ([CanBeNull] object buildConfigurationFilter)
    {
      return (string) SUT.ConvertTo(null, null, buildConfigurationFilter, buildConfigurationFilter?.GetType() ?? typeof(object));
    }

    static BuildConfigurationFilter ConvertFrom ([CanBeNull] object buildConfigurationFilterString)
    {
      return (BuildConfigurationFilter) SUT.ConvertFrom(null, null, buildConfigurationFilterString);
    }
  }
}