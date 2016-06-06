using System;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.Api.Extensions;

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

namespace SolutionInspector.Api.Tests.Extensions
{
  [Subject (typeof(StringExtensions))]
  class StringExtensionsSpec
  {
    class when_removing_suffix
    {
      Because of = () =>
      {
        /* Actual tests are in the its */
      };

      It works_with_suffix = () =>
          "StuffSuffix".RemoveSuffix("Suffix").Should().Be("Stuff");

      It works_without_suffix = () =>
          "Stuff".RemoveSuffix("Suffix").Should().Be("Stuff");

      It works_with_null = () =>
          ((string) null).RemoveSuffix("Suffix").Should().BeNull();

      It works_with_double_suffix = () =>
          "StuffSuffixSuffix".RemoveSuffix("Suffix").Should().Be("StuffSuffix");
    }
  }
}