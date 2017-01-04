using FluentAssertions;
using JetBrains.Annotations;
using NUnit.Framework;
using SolutionInspector.Commons.Extensions;

namespace SolutionInspector.Commons.Tests.Extensions
{
  public class StringExtensionsTests
  {
    [Test]
    [TestCase ("StuffSuffix", "Stuff")]
    [TestCase ("Stuff", "Stuff")]
    [TestCase (null, null)]
    [TestCase ("StuffSuffixSuffix", "StuffSuffix")]
    public void RemoveSuffix ([CanBeNull] string input, [CanBeNull] string expected)
    {
      // ACT
      var result = input.RemoveSuffix("Suffix");

      // ASSERT
      result.Should().Be(expected);
    }
  }
}