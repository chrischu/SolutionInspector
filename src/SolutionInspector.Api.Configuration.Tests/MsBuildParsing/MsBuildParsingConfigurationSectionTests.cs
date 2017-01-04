using System.Xml.Linq;
using FluentAssertions;
using SolutionInspector.Api.Configuration.MsBuildParsing;
using SolutionInspector.Configuration;
using Xunit;

namespace SolutionInspector.Api.Configuration.Tests.MsBuildParsing
{
  public class MsBuildParsingConfigurationSectionTests
  {
    [Fact]
    public void Loading ()
    {
      var element = XDocument.Parse(MsBuildParsingConfigurationSection.ExampleConfiguration).Root;

      // ACT
      var result = ConfigurationElement.Load<MsBuildParsingConfigurationSection>(element);

      // ASSERT
      result.IsValidProjectItemType("None").Should().BeTrue();
      result.IsValidProjectItemType("Something").Should().BeFalse();
    }
  }
}