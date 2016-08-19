using FluentAssertions;
using Machine.Specifications;
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
  [Subject (typeof(NameFilter))]
  class NameFilterSpec
  {
    class when_checking_for_match
    {
      Because of = () =>
      {
        /* Actual tests are in the its */
      };

      It works_for_non_wildcard_includes = () =>
      {
        var filter = new NameFilter(new[] { "Include" });
        filter.IsMatch("Include").Should().BeTrue();
        filter.IsMatch("NotIncluded").Should().BeFalse();
      };

      It works_for_wildcard_includes = () =>
      {
        var filter = new NameFilter(new[] { "Inc*lude" });
        filter.IsMatch($"Inc{Some.String()}lude").Should().BeTrue();
        filter.IsMatch("NotIncluded").Should().BeFalse();
      };

      It works_for_non_wildcard_excludes = () =>
      {
        var filter = new NameFilter(new[] { "*Include" }, new[] { "ExcludedInclude" });
        filter.IsMatch($"{Some.String()}Include").Should().BeTrue();
        filter.IsMatch("ExcludedIncluded").Should().BeFalse();
      };

      It works_for_wildcard_excludes = () =>
      {
        var filter = new NameFilter(new[] { "*Include" }, new[] { "Exc*ludedInclude" });
        filter.IsMatch($"{Some.String()}Include").Should().BeTrue();
        filter.IsMatch($"Exc{Some.String()}ludedIncluded").Should().BeFalse();
      };
    }
  }
}