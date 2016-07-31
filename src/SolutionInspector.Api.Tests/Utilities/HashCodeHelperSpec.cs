using System;
using FakeItEasy;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.Api.Utilities;
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

namespace SolutionInspector.Api.Tests.Utilities
{
  [Subject (typeof(HashCodeHelper))]
  class HashCodeHelperSpec
  {
    class when_getting_hash_code_from_objects
    {
      Because of = () =>
      {
        /* Actual tests are in the its. */
      };

      It replaces_nulls_with_zeros = () =>
          HashCodeHelper.GetHashCode((object) null).Should().Be(17 * 23);

      It works_with_no_object = () =>
          HashCodeHelper.GetHashCode(new object[0]).Should().Be(17);

      It works_with_one_object = () =>
          HashCodeHelper.GetHashCode(FakeWithHashCode(1)).Should().Be(17 * 23 + 1);

      It works_with_two_objects = () =>
          HashCodeHelper.GetHashCode(FakeWithHashCode(1), FakeWithHashCode(2)).Should().Be((17 * 23 + 1) * 23 + 2);

      It works_with_negative_hashcode = () =>
          HashCodeHelper.GetHashCode(FakeWithHashCode(-7)).Should().Be(17 * 23 - 7);

      It creates_differing_hash_codes_when_order_is_swapped = () =>
      {
        var hashCode1 = Some.Integer;
        var hashCode2 = Some.Integer;

        HashCodeHelper.GetHashCode(FakeWithHashCode(hashCode1), FakeWithHashCode(hashCode2))
            .Should()
            .NotBe(HashCodeHelper.GetHashCode(FakeWithHashCode(hashCode2), FakeWithHashCode(hashCode1)));
      };
    }

    class when_getting_hash_code_from_ints
    {
      Because of = () =>
      {
        /* Actual tests are in the its. */
      };

      It works_with_empty = () =>
          HashCodeHelper.GetHashCode(new int[0]).Should().Be(17);

      It works_with_one_object = () =>
          HashCodeHelper.GetHashCode(1).Should().Be(17 * 23 + 1);

      It works_with_two_objects = () =>
          HashCodeHelper.GetHashCode(1, 2).Should().Be((17 * 23 + 1) * 23 + 2);

      It works_with_negative_hashcode = () =>
          HashCodeHelper.GetHashCode(-7).Should().Be(17 * 23 - 7);

      It creates_differing_hash_codes_when_order_is_swapped = () =>
      {
        var hashCode1 = Some.Integer;
        var hashCode2 = Some.Integer;

        HashCodeHelper.GetHashCode(hashCode1, hashCode2).Should().NotBe(HashCodeHelper.GetHashCode(hashCode2, hashCode1));
      };
    }

    static object FakeWithHashCode (int hashCode)
    {
      var fake = A.Fake<object>();
      A.CallTo(() => fake.GetHashCode()).Returns(hashCode);
      return fake;
    }
  }
}