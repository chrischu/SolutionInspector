using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Commons.Extensions;

namespace SolutionInspector.Commons.Tests.Extensions
{
  public class EnumerableExtensionsTests
  {
    [Test]
    [TestCase(new int[0], false)]
    [TestCase(new[] { 7 }, false)]
    [TestCase(new[] { 7, 9 }, true)]
    
    public void ContainsMoreThanOne(IEnumerable<int> enumerable, bool expected)
    {
      // ACT
      var result = enumerable.ContainsMoreThanOne();

      // ASSERT
      result.Should().Be(expected);
    }
  }
}