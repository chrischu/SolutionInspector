using FluentAssertions;
using NUnit.Framework;

namespace SolutionInspector.Api.Configuration.Tests
{
  public class NameFilterTests
  {
    [Test]
    [TestCase("Include", "Include", true)]
    [TestCase("Include", "NotIncluded", false)]
    [TestCase("Inc*lude", "IncXYZlude", true)]
    [TestCase("Inc*lude", "NotIncluded", false)]
    [TestCase("+*Include;-ExcludedInclude", "XYZInclude", true)]
    [TestCase("+*Include;-ExcludedInclude", "ExcludedInclude", false)]
    [TestCase("+*Include;-Exc*ludedInclude", "XYZInclude", true)]
    [TestCase("+*Include;-Exc*ludedInclude", "ExcXYZludedInclude", false)]
    public void IsMatch(string filterString, string testString, bool expectedResult)
    {
      var filter = new NameFilterConverter().ConvertFrom(filterString);
      
      // ACT
      var result = filter.IsMatch(testString);

      // ASSERT
      result.Should().Be(expectedResult);
    }
  }
}