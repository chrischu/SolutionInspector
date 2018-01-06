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

    [Test]
    [TestCase(null, 1, null)]
    [TestCase("A", 0, "")]
    [TestCase("ABC", 2, "AB")]
    [TestCase("ABC", 5, "ABC")]
    public void Prefix ([CanBeNull] string input, int length, [CanBeNull] string expected)
    {
      // ACT
      var result = input.Prefix(length);

      // ASSERT
      result.Should().Be(expected);
    }

    [Test]
    [TestCase(null, 1, null)]
    [TestCase("A", 0, "")]
    [TestCase("ABC", 2, "BC")]
    [TestCase("ABC", 5, "ABC")]
    public void Suffix([CanBeNull] string input, int length, [CanBeNull] string expected)
    {
      // ACT
      var result = input.Suffix(length);

      // ASSERT
      result.Should().Be(expected);
    }

    [Test]
    [TestCase(null, null)]
    [TestCase("", "")]
    [TestCase("a", "A")]
    [TestCase("axxx", "Axxx")]
    [TestCase("Axxx", "Axxx")]
    public void ToFirstCharUpper ([CanBeNull] string input, [CanBeNull] string expected)
    {
      // ACT
      var result = input.ToFirstCharUpper();

      // ASSERT
      result.Should().Be(expected);
    }

    [Test]
    [TestCase(null, null)]
    [TestCase("", "")]
    [TestCase("A", "a")]
    [TestCase("Axxx", "axxx")]
    [TestCase("axxx", "axxx")]
    public void ToFirstCharLower ([CanBeNull] string input, [CanBeNull] string expected)
    {
      // ACT
      var result = input.ToFirstCharLower();

      // ASSERT
      result.Should().Be(expected);
    }
  }
}