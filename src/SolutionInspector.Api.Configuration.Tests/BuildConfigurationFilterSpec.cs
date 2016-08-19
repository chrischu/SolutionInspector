using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.TestInfrastructure;

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
  [Subject (typeof(BuildConfigurationFilter))]
  class BuildConfigurationFilterSpec
  {
    class when_checking_for_match
    {
      Because of = () =>
      {
        /* Actual tests are in the its */
      };

      It works_for_exact_matches = () =>
          new BuildConfigurationFilter(new BuildConfiguration("A", "B")).IsMatch(new BuildConfiguration("A", "B")).Should().BeTrue();

      It works_when_one_part_differs = () =>
      {
        new BuildConfigurationFilter(new BuildConfiguration("A", "Y")).IsMatch(
            new BuildConfiguration("A", "B")).Should().BeFalse();

        new BuildConfigurationFilter(new BuildConfiguration("X", "B")).IsMatch(
            new BuildConfiguration("A", "B")).Should().BeFalse();
      };

      It works_with_wildcards = () =>
      {
        new BuildConfigurationFilter(new BuildConfiguration("A", "*")).IsMatch(
            new BuildConfiguration("A", Some.String())).Should().BeTrue();

        new BuildConfigurationFilter(new BuildConfiguration("*", "B")).IsMatch(
            new BuildConfiguration(Some.String(), "B")).Should().BeTrue();

        new BuildConfigurationFilter(new BuildConfiguration("*", "*")).IsMatch(
            new BuildConfiguration(Some.String(), Some.String())).Should().BeTrue();
      };
    }
  }
}