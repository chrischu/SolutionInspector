using System;
using FluentAssertions;
using FluentAssertions.Primitives;
using Machine.Specifications;

namespace SolutionInspector.TestInfrastructure.AssertionExtensions
{
  public static class StringAssertionExtensions
  {
    public static void BeWithDiff(this StringAssertions stringAssertions, string expected)
    {
      var actualTrimmed = stringAssertions.Subject.Trim();
      var expectedTrimmed = expected.Trim();

      try
      {
        actualTrimmed.Should().Be(expectedTrimmed);
      } catch(SpecificationException ex)
      {
        var message = $@"Differences:
------------
{DiffFormatter.FormatDiff(expectedTrimmed, actualTrimmed)}

{ex.Message}";
        throw new SpecificationException(message);
      }
    }
  }
}