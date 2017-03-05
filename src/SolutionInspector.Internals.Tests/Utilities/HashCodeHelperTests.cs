using System.Collections;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Commons.Utilities;
using SolutionInspector.TestInfrastructure;

namespace SolutionInspector.Internals.Tests.Utilities
{
  public class HashCodeHelperTests
  {
    [Test]
    [TestCaseSource (nameof(GetHashCodeFromObjectsTestData))]
    public int GetHashCodeFromObjects (object[] input)
    {
      // ACT & ASSERT
      return HashCodeHelper.GetHashCode(input);
    }

    private static IEnumerable GetHashCodeFromObjectsTestData ()
    {
      yield return
          new TestCaseData((object) new[] { (object) null })
          {
            ExpectedResult = 17 * 23,
            TestName = "GetHashCodeFromObjects_Null_ReplacesNullsWithZeros"
          };
      yield return new TestCaseData((object) new object[0]) { ExpectedResult = 17, TestName = "GetHashCodeFromObjects_WithoutObjects_Works" };
      yield return
          new TestCaseData((object) new[] { ObjectWithHashCode(1) })
          {
            ExpectedResult = 17 * 23 + 1,
            TestName = "GetHashCodeFromObjects_OneObject_Works"
          };
      yield return
          new TestCaseData((object) new[] { ObjectWithHashCode(1), ObjectWithHashCode(2) })
          {
            ExpectedResult = (17 * 23 + 1) * 23 + 2,
            TestName = "GetHashCodeFromObjects_TwoObjects_Works"
          };
      yield return
          new TestCaseData((object) new[] { ObjectWithHashCode(-7) })
          {
            ExpectedResult = 17 * 23 - 7,
            TestName = "GetHashCodeFromObjects_ObjectWithNegativeHashCode_Works"
          };
    }

    [Test]
    public void GetHashCodeFromObjects_SameObjectsInDifferentOrder_ReturnsDifferentHashCodes ()
    {
      var hashCode1 = Some.Integer;
      var hashCode2 = Some.Integer;

      // ACT
      var result1 = HashCodeHelper.GetHashCode(ObjectWithHashCode(hashCode1), ObjectWithHashCode(hashCode2));
      var result2 = HashCodeHelper.GetHashCode(ObjectWithHashCode(hashCode2), ObjectWithHashCode(hashCode1));

      // ASSERT
      result1.Should().NotBe(result2);
    }

    [Test]
    [TestCaseSource (nameof(GetHashCodeFromIntsTestData))]
    public int GetHashCodeFromInts (int[] input)
    {
      // ACT & ASSERT
      return HashCodeHelper.GetHashCode(input);
    }

    private static IEnumerable GetHashCodeFromIntsTestData ()
    {
      yield return new TestCaseData(new int[0]) { ExpectedResult = 17, TestName = "GetHashCodeFromInts_WithoutInts_Works" };
      yield return new TestCaseData(new[] { 1 }) { ExpectedResult = 17 * 23 + 1, TestName = "GetHashCodeFromInts_OneInt_Works" };
      yield return
          new TestCaseData(new[] { 1, 2 })
          {
            ExpectedResult = (17 * 23 + 1) * 23 + 2,
            TestName = "GetHashCodeFromInts_TwoInts_Works"
          };
      yield return
          new TestCaseData(new[] { -7 })
          {
            ExpectedResult = 17 * 23 - 7,
            TestName = "GetHashCodeFromInts_NegativeHashCode_Works"
          };
    }

    [Test]
    public void GetHashCodeFromInts_SameIntsInDifferentOrder_ReturnsDifferentHashCodes ()
    {
      var hashCode1 = Some.Integer;
      var hashCode2 = Some.Integer;

      // ACT
      var result1 = HashCodeHelper.GetHashCode(hashCode1, hashCode2);
      var result2 = HashCodeHelper.GetHashCode(hashCode2, hashCode1);

      // ASSERT
      result1.Should().NotBe(result2);
    }

    private static object ObjectWithHashCode (int hashCode)
    {
      return new DummyWithHashCode(hashCode);
    }

    private class DummyWithHashCode
    {
      private readonly int _hashCode;

      public DummyWithHashCode (int hashCode)
      {
        _hashCode = hashCode;
      }

      public override int GetHashCode ()
      {
        return _hashCode;
      }
    }
  }
}