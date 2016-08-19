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

namespace SolutionInspector.Api.Configuration.Tests
{
  [Subject (typeof(NameFilterConverter))]
  class NameFilterConverterSpec
  {
    static NameFilterConverter SUT;

    Establish ctx = () => { SUT = new NameFilterConverter(); };

    class when_converting_to
    {
      Establish ctx = () => { NameFilter = new NameFilter(new[] { "A", "*B", "C*", "*D*" }, new[] { "E", "*F", "G*", "*H*" }); };

      Because of = () => Result = SUT.ConvertTo(NameFilter);

      It converts = () =>
          Result.Should().Be("+A;+*B;+C*;+*D*;-E;-*F;-G*;-*H*");

      static NameFilter NameFilter;
      static string Result;
    }

    class when_converting_to_with_null_value
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
        NameFilter = new NameFilter(new[] { "A", "B" }, new[] { "C", "D" });
        NameFilterString = SUT.ConvertTo(NameFilter);
      };

      Because of = () => Result = SUT.ConvertFrom(NameFilterString);

      It returns_original_NameFilter = () =>
          Result.ToString().Should().Be(NameFilter.ToString());

      static NameFilter NameFilter;
      static string NameFilterString;
      static NameFilter Result;
    }

    class when_converting_from_with_invalid_format
    {
      Establish ctx = () =>
      {
        NameFilterString = "THIS IS NOT A NAMEFILTER";
      };

      Because of = () => Exception = Catch.Exception(() => SUT.ConvertFrom(NameFilterString));

      It throws = () =>
          Exception.Should().Be<FormatException>().WithMessage($"The filter string '{NameFilterString}' is not in the correct format.");

      static NameFilter NameFilter;
      static string NameFilterString;
      static Exception Exception;
    }

    class when_converting_from_with_null_value
    {
      Because of = () => Result = SUT.ConvertFrom(null);

      It returns_null = () =>
          Result.Should().BeNull();

      static NameFilter Result;
    }
  }
}