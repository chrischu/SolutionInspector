using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Commons.Extensions;

namespace SolutionInspector.Api.Configuration.Tests
{
  public class BuildConfigurationFilterTests
  {
    [Test]
    [TestCase ("A|B", "A|B", true)]
    [TestCase ("A|B", "A|Y", false)]
    [TestCase ("A|B", "X|B", false)]
    [TestCase ("A|B", "X|Y", false)]
    [TestCase ("A|*", "A|Y", true)]
    [TestCase ("*|B", "X|B", true)]
    [TestCase ("*|*", "X|Y", true)]
    public void IsMatch (string filterString, string configurationString, bool expectedResult)
    {
      var filter = new BuildConfigurationFilterConverter().ConvertFrom(filterString).AssertNotNull();
      var configuration = BuildConfiguration.Parse(configurationString);

      // ACT
      var result = filter.IsMatch(configuration);

      // ASSERT
      result.Should().Be(expectedResult);
    }
  }
}