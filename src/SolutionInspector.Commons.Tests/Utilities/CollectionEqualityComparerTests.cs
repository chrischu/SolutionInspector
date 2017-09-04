using System;
using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Commons.Utilities;

namespace SolutionInspector.Commons.Tests.Utilities
{
  public class CollectionEqualityComparerTests
  {
    private IEqualityComparer<string> _comparer;

    private CollectionEqualityComparer<string> _sut;

    [SetUp]
    public void SetUp ()
    {
      _comparer = A.Fake<IEqualityComparer<string>>(o => o.Wrapping(EqualityComparer<string>.Default));

      _sut = new CollectionEqualityComparer<string>(_comparer);
    }

    [Test]
    [TestCase(new[] { "A", "B" }, new[] { "B", "A" }, true)]
    [TestCase(new[] { "A", "A", "B" }, new[] { "A", "B", "A" }, true)]
    [TestCase(new[] { "A", "A", "B" }, new[] { "A", "B", "A", "C" }, false)]
    public void Equals (string[] c1, string[] c2, bool expectedResult)
    {
      // ACT
      var result = _sut.Equals(c1, c2);

      // ASSERT
      result.Should().Be(expectedResult);
    }

    [Test]
    public new void GetHashCode ()
    {
      A.CallTo(() => _comparer.GetHashCode(A<string>._)).ReturnsLazily((string s) => int.Parse(s));

      var c = new[] { "1", "2" };

      // ACT
      var result = _sut.GetHashCode(c);

      // ASSERT
      result.Should().Be(HashCodeHelper.GetOrderIndependentHashCode(1, 2));
    }

    [Test]
    public void GetHashCode_WithEqualCollectionsWithDifferentOrder_ReturnsSameHashCode ()
    {
      var c1 = new[] { "A", "B" };
      var c2 = new[] { "B", "A" };

      // ACT
      var r1 = _sut.GetHashCode(c1);
      var r2 = _sut.GetHashCode(c2);

      // ASSERT
      r1.Should().Be(r2);
    }
  }
}