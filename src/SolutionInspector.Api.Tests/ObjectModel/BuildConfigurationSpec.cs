using System;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.TestInfrastructure;
using SolutionInspector.TestInfrastructure.AssertionExtensions;

#region R# preamble for Machine.Specifications files

// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable ClassNeverInstantiated.Local
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Local
// ReSharper disable NotAccessedField.Local
// ReSharper disable StaticMemberInGenericType
// ReSharper disable UnassignedField.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable UnassignedGetOnlyAutoProperty

#endregion

namespace SolutionInspector.Api.Tests.ObjectModel
{
  [Subject (typeof(BuildConfiguration))]
  class BuildConfigurationSpec
  {
    class when_parsing
    {
      Because of = () => Result = BuildConfiguration.Parse("Debug|Any CPU");

      It parses_configuration = () =>
          Result.ConfigurationName.Should().Be("Debug");

      It parses_platform = () =>
          Result.PlatformName.Should().Be("Any CPU");

      static BuildConfiguration Result;
    }

    class when_parsing_invalid_string
    {
      Because of = () => Exception = Catch.Exception(() => BuildConfiguration.Parse("NOT_VALID"));

      It throws = () =>
          Exception.Should().BeArgumentException(@"The value 'NOT_VALID' is not a valid string representation of a BuildConfiguration.", "s");

      static Exception Exception;
    }

    class when_comparing_equality
    {
      Establish ctx = () => { BuildConfiguration = new BuildConfiguration(Some.String(), Some.String()); };

      Because of = () =>
      {
        /* Actual tests are in the its. */
      };

      It works_with_same_reference = () =>
          BuildConfiguration.Equals(BuildConfiguration).Should().BeTrue();

      It works_with_null = () =>
          BuildConfiguration.Equals(null).Should().BeFalse();

      It works_with_equal_instances = () =>
          BuildConfiguration.Equals(new BuildConfiguration(BuildConfiguration.ConfigurationName, BuildConfiguration.PlatformName)).Should().BeTrue();

      It works_with_differing_instances = () =>
          BuildConfiguration.Equals(new BuildConfiguration(BuildConfiguration.ConfigurationName, "DIFFERENT")).Should().BeFalse();

      It works_with_same_reference_as_object = () =>
          BuildConfiguration.Equals((object) BuildConfiguration).Should().BeTrue();

      It works_with_null_as_object = () =>
          BuildConfiguration.Equals((object) null).Should().BeFalse();

      static BuildConfiguration BuildConfiguration;
    }
  }
}