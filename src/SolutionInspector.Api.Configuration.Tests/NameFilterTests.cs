using FluentAssertions;
using Xunit;

namespace SolutionInspector.Api.Configuration.Tests
{
  public class NameFilterTests
  {
    [Theory]
    [InlineData("Include", "Include", true)]
    [InlineData("Include", "NotIncluded", false)]
    [InlineData("Inc*lude", "IncXYZlude", true)]
    [InlineData("Inc*lude", "NotIncluded", false)]
    [InlineData("+*Include;-ExcludedInclude", "XYZInclude", true)]
    [InlineData("+*Include;-ExcludedInclude", "ExcludedInclude", false)]
    [InlineData("+*Include;-Exc*ludedInclude", "XYZInclude", true)]
    [InlineData("+*Include;-Exc*ludedInclude", "ExcXYZludedInclude", false)]
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