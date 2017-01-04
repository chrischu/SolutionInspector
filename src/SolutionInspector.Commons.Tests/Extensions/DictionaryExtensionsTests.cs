using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Commons.Extensions;

namespace SolutionInspector.Commons.Tests.Extensions
{
  public class DictionaryExtensionsTests
  {
    [Test]
    public void GetValueOrDefault_ValueExists_ReturnsValue()
    {
      var dictionary = new Dictionary<string, int> { { "key", 7 } };

      // ACT
      var result = dictionary.GetValueOrDefault("key");

      // ASSERT
      result.Should().Be(7);
    }

    [Test]
    public void GetValueOrDefault_ValueDoesNotExist_ReturnsValue()
    {
      var dictionary = new Dictionary<string, int>();

      // ACT
      var result = dictionary.GetValueOrDefault("key");

      // ASSERT
      result.Should().Be(default(int));
    }
  }
}