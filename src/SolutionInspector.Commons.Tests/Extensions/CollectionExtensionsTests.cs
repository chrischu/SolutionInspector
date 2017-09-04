using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Commons.Extensions;

namespace SolutionInspector.Commons.Tests.Extensions
{
  public class CollectionExtensionsTests
  {
    [Test]
    public void AddRange ()
    {
      ICollection<string> collection = new List<string> { "C" };

      // ACT
      collection.AddRange(new[] { "A", "B" });

      // ASSERT
      collection.Should().Equal("C", "A", "B");
    }
  }
}