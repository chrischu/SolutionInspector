using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using JetBrains.Annotations;
using SolutionInspector.Api.ObjectModel;

namespace SolutionInspector.DefaultRules.Tests
{
  public static class ProjectPropertyFakeUtility
  {
    public static void SetupFakeBuildConfigurationDependentProperty (
        IAdvancedProject advancedProject,
        BuildConfiguration buildConfiguration,
        string property,
        string value)
    {
      SetupFakeBuildConfigurationDependentProperties(advancedProject, buildConfiguration, new Dictionary<string, string> { { property, value } });
    }

    public static void SetupEmptyBuildConfigurationDependentProperties (IAdvancedProject advancedProject, BuildConfiguration buildConfiguration)
    {
      SetupFakeBuildConfigurationDependentProperties(advancedProject, buildConfiguration, new Dictionary<string, string>());
    }

    public static void SetupFakeBuildConfigurationDependentProperties (
        IAdvancedProject advancedProject,
        BuildConfiguration buildConfiguration,
        Dictionary<string, string> properties)
    {
      var projectProperties = properties.Select(
          p =>
          {
            var projectProperty = A.Fake<IProjectProperty>();
            A.CallTo(() => projectProperty.Name).Returns(p.Key);
            A.CallTo(() => projectProperty.Value).Returns(p.Value);
            return projectProperty;
          }).ToArray();

      A.CallTo(() => advancedProject.GetPropertiesBasedOnCondition(buildConfiguration, null))
          .Returns(projectProperties);
    }

    public static void SetupFakeProperty (IAdvancedProject advancedProject, string property, [CanBeNull] string value)
    {
      var projectProperty = A.Fake<IProjectProperty>();
      A.CallTo(() => projectProperty.Name).Returns(property);
      A.CallTo(() => projectProperty.Value).Returns(value);

      A.CallTo(() => advancedProject.Properties)
          .Returns(new[] { projectProperty });
    }
  }
}