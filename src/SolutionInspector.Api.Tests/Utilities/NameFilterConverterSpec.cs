using System;
using FluentAssertions;
using JetBrains.Annotations;
using Machine.Specifications;
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
  [Subject (typeof(NameFilterConverter))]
  class NameFilterConverterSpec
  {
    static NameFilterConverter SUT;

    Establish ctx = () => { SUT = new NameFilterConverter(); };

    class when_converting_to
    {
      Establish ctx = () => { NameFilter = new NameFilter(new[] { "A", "B" }, new[] { "C", "D" }); };

      Because of = () => Result = ConvertTo(NameFilter);

      It returns_ToString_result = () =>
          Result.Should().Be(NameFilter.ToString());

      static NameFilter NameFilter;
      static string Result;
    }

    class when_converting_to_with_null_value
    {
      Because of = () => Result = ConvertTo(null);

      It returns_null = () =>
          Result.Should().BeNull();

      static string Result;
    }

    class when_converting_to_with_non_NameFilter
    {
      Because of = () => Exception = Catch.Exception(() => ConvertTo("NOT A NAME FILTER"));

      It throws = () =>
          Exception.Should().BeArgumentException(
              $"Unsupported type '{typeof(string).FullName}', expected type '{typeof(NameFilter).FullName}'.",
              "value");

      static Exception Exception;
    }

    class when_converting_from
    {
      Establish ctx = () =>
      {
        NameFilter = new NameFilter(new[] { "A", "B" }, new[] { "C", "D" });
        NameFilterString = ConvertTo(NameFilter);
      };

      Because of = () => Result = ConvertFrom(NameFilterString);

      It returns_original_NameFilter = () =>
          Result.ToString().Should().Be(NameFilter.ToString());

      static NameFilter NameFilter;
      static string NameFilterString;
      static NameFilter Result;
    }

    class when_converting_from_with_null_value
    {
      Because of = () => Result = ConvertFrom(null);

      It returns_null = () =>
          Result.Should().BeNull();

      static NameFilter Result;
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

    static string ConvertTo ([CanBeNull] object nameFilter)
    {
      return (string) SUT.ConvertTo(null, null, nameFilter, nameFilter?.GetType() ?? typeof(object));
    }

    static NameFilter ConvertFrom ([CanBeNull] object nameFilterString)
    {
      return (NameFilter) SUT.ConvertFrom(null, null, nameFilterString);
    }
  }
}