using System;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Api.Configuration.MsBuildParsing;
using SolutionInspector.TestInfrastructure.Configuration;

namespace SolutionInspector.Api.Tests.Configuration.MsBuildParsing
{
  public class MsBuildParsingConfigurationSectionTests
  {
    [Test]
    public void Loading ()
    {
      // ACT
      var result = new MsBuildParsingConfigurationSection();
      ConfigurationHelper.DeserializeSection(result, MsBuildParsingConfigurationSection.ExampleConfiguration);

      // ASSERT
      result.IsValidProjectItemType("None").Should().BeTrue();
      result.IsValidProjectItemType("Something").Should().BeFalse();
    }
  }
}