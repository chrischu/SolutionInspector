using System;
using System.Configuration;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.Api.Utilities;
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

namespace SolutionInspector.Api.Tests.Utilities
{
  [Subject (typeof (NameFilter))]
  class NameFilterSpec
  {
    class when_parsing_explicit_include
    {
      Because of = () => Result = ParseFilter("+Include");

      It matches_included = () =>
          Result.IsMatch("Include").Should().BeTrue();

      It does_not_match_not_included = () =>
          Result.IsMatch("NotIncluded").Should().BeFalse();

      It parses_correctly = () =>
          Result.ToString().Should().Be("+Include");

      static NameFilter Result;
    }

    class when_parsing_implicit_include
    {
      Because of = () => Result = ParseFilter("Include");

      It matches_included = () =>
          Result.IsMatch("Include").Should().BeTrue();

      It does_not_match_not_included = () =>
          Result.IsMatch("NotIncluded").Should().BeFalse();

      It parses_correctly = () =>
          Result.ToString().Should().Be("+Include");

      static NameFilter Result;
    }

    class when_parsing_include_with_wildcard
    {
      Because of = () => Result = ParseFilter("Inc*lude");

      It matches_included_without_extra_characters = () =>
          Result.IsMatch("Include").Should().BeTrue();

      It matches_included_with_extra_characters = () =>
          Result.IsMatch("IncEXTRASTUFFlude").Should().BeTrue();

      It does_not_match_not_included = () =>
          Result.IsMatch("NotIncluded").Should().BeFalse();

      It parses_correctly = () =>
          Result.ToString().Should().Be("+Inc*lude");

      static NameFilter Result;
    }

    class when_parsing_exclude
    {
      Because of = () => Result = ParseFilter("+*Include;-ExcludedInclude");

      It matches_included = () =>
          Result.IsMatch("Include").Should().BeTrue();

      It does_not_match_excluded = () =>
          Result.IsMatch("ExcludedInclude").Should().BeFalse();

      It parses_correctly = () =>
          Result.ToString().Should().Be("+*Include;-ExcludedInclude");

      static NameFilter Result;
    }

    class when_parsing_malformed_filter_string
    {
      Because of = () => Exception = Catch.Exception(() => ParseFilter(";"));

      private It throws = () =>
          Exception.Should().Be<ConfigurationErrorsException>()
              .WithMessage("The value of the property 'filter' cannot be parsed. The error is: The filter string ';' is not in the correct format.");

      static Exception Exception;
    }

    static NameFilter ParseFilter (string filterString)
    {
      var configuration = new Configuration();
      ConfigurationHelper.DeserializeElement(configuration, $@"<element filter=""{filterString}"" />");
      return configuration.NameFilter;
    }

    class Configuration : ConfigurationElement
    {
      [ConfigurationProperty ("filter")]
      public NameFilter NameFilter
      {
        get { return (NameFilter) this["filter"]; }
        set { this["filter"] = value; }
      }
    }
  }
}