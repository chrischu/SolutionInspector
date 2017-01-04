using FluentAssertions;
using SolutionInspector.Api.ObjectModel;
using Xunit;

namespace SolutionInspector.Api.Configuration.Tests
{
  public class BuildConfigurationFilterTests
  {
    [Theory]
    [InlineData("A|B", "A|B", true)]
    [InlineData("A|B", "A|Y", false)]
    [InlineData("A|B", "X|B", false)]
    [InlineData("A|B", "X|Y", false)]
    [InlineData("A|*", "A|Y", true)]
    [InlineData("*|B", "X|B", true)]
    [InlineData("*|*", "X|Y", true)]
    public void IsMatch(string filterString, string configurationString, bool expectedResult)
    {
      var filter = new BuildConfigurationFilterConverter().ConvertFrom(filterString);
      var configuration = BuildConfiguration.Parse(configurationString);

      // ACT
      var result = filter.IsMatch(configuration);

      // ASSERT
      result.Should().Be(expectedResult);
    }
  }
}