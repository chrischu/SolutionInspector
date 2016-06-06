using System;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.Api.Extensions;
using SolutionInspector.TestInfrastructure;

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
  [Subject (typeof(EnumerableExtensions))]
  class EnumerableExtensionsSpec
  {
    class when_checking_if_enumerable_contains_more_than_one
    {
      Because of = () =>
      {
        /* actual tests are in the its */
      };

      It works_for_empty = () =>
          new int[0].ContainsMoreThanOne().Should().BeFalse();

      It works_for_one = () =>
          new[] { Some.Integer }.ContainsMoreThanOne().Should().BeFalse();

      It works_for_more_than_one = () =>
          new[] { Some.Integer, Some.Integer }.ContainsMoreThanOne().Should().BeTrue();

      static int Result;
    }
  }
}