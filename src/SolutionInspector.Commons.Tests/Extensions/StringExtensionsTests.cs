using FluentAssertions;
using JetBrains.Annotations;
using SolutionInspector.Commons.Extensions;
using Xunit;

namespace SolutionInspector.Commons.Tests.Extensions
{
  public class StringExtensionsTests
  {
    [Theory]
    [InlineData ("StuffSuffix", "Stuff")]
    [InlineData ("Stuff", "Stuff")]
    [InlineData (null, null)]
    [InlineData ("StuffSuffixSuffix", "StuffSuffix")]
    public void RemoveSuffix ([CanBeNull] string input, [CanBeNull] string expected)
    {
      // ACT
      var result = input.RemoveSuffix("Suffix");

      // ASSERT
      result.Should().Be(expected);
    }
  }
}