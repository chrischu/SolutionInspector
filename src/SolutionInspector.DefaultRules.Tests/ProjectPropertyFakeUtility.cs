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
      var projectProperties = properties.ToDictionary(
        kvp => kvp.Key,
        kvp =>
        {
          var evaluatedProjectPropertyValue = A.Fake<IEvaluatedProjectPropertyValue>();

          A.CallTo(() => evaluatedProjectPropertyValue.SourceOccurrence).Returns(A.Dummy<IProjectPropertyOccurrence>());
          A.CallTo(() => evaluatedProjectPropertyValue.Value).Returns(kvp.Value);

          return evaluatedProjectPropertyValue;
        });

      A.CallTo(() => advancedProject.EvaluateProperties(buildConfiguration, null))
          .Returns(projectProperties);
    }

    public static void SetupFakeProperty (IAdvancedProject advancedProject, string property, [CanBeNull] string value)
    {
      var projectProperty = A.Fake<IProjectProperty>();
      A.CallTo(() => projectProperty.Name).Returns(property);
      A.CallTo(() => projectProperty.DefaultValue).Returns(value);

      A.CallTo(() => advancedProject.Properties)
          .Returns(new Dictionary<string, IProjectProperty> { { property, projectProperty } });
    }
  }
}