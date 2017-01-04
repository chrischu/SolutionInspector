using System.Xml.Linq;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Api.Configuration.MsBuildParsing;
using SolutionInspector.Configuration;

namespace SolutionInspector.Api.Configuration.Tests.MsBuildParsing
{
  public class MsBuildParsingConfigurationSectionTests
  {
    [Test]
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