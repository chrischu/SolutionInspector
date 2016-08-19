using System.Collections.Generic;
using FluentAssertions;
using SolutionInspector.Commons.Extensions;
using Xunit;

namespace SolutionInspector.Commons.Tests.Extensions
{
  public class EnumerableExtensionsTests
  {
    [Theory]
    [InlineData(new int[0], false)]
    [InlineData(new[] { 7 }, false)]
    [InlineData(new[] { 7, 9 }, true)]
    
    public void ContainsMoreThanOne(IEnumerable<int> enumerable, bool expected)
    {
      // ACT
      var result = enumerable.ContainsMoreThanOne();

      // ASSERT
      result.Should().Be(expected);
    }
  }
}