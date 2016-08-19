using System.Linq;
using JetBrains.Annotations;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Configuration;

namespace SolutionInspector.Api.Configuration
{
  /// <summary>
  ///   Configuration converter that converts between <see cref="string" /> and <see cref="BuildConfigurationFilter" />.
  /// </summary>
  public class BuildConfigurationFilterConverter : IConfigurationConverter<BuildConfigurationFilter>
  {
    public string ConvertTo ([CanBeNull] BuildConfigurationFilter value)
    {
      return value?.ToString();
    }

    public BuildConfigurationFilter ConvertFrom ([CanBeNull] string value)
    {
      return value == null
        ? null
        : new BuildConfigurationFilter(value.Split(',').Select(BuildConfiguration.Parse));
    }
  }
}