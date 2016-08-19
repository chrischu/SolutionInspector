using FluentAssertions;
using JetBrains.Annotations;
using Machine.Specifications;
using SolutionInspector.Api.ObjectModel;

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

namespace SolutionInspector.Api.Configuration.Tests
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

    class when_converting_to_with_null
    {
      Because of = () => Result = SUT.ConvertTo(null);

      It returns_null = () =>
            Result.Should().BeNull();

      static string Result;
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

    class when_converting_from_with_null
    {
      Because of = () => Result = SUT.ConvertFrom(null);

      It returns_null = () =>
            Result.Should().BeNull();

      static BuildConfigurationFilter Result;
    }

    static string ConvertTo ([CanBeNull] BuildConfigurationFilter buildConfigurationFilter)
    {
      return SUT.ConvertTo(buildConfigurationFilter);
    }

    static BuildConfigurationFilter ConvertFrom ([CanBeNull] string buildConfigurationFilterString)
    {
      return (BuildConfigurationFilter) SUT.ConvertFrom(buildConfigurationFilterString);
    }
  }
}