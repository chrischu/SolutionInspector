using SolutionInspector.Configuration;

namespace SolutionInspector.Api.Configuration.MsBuildParsing
{
  internal class ProjectBuildActionConfigurationElement : ConfigurationElement
  {
    [ConfigurationValue]
    public string Name
    {
      get { return GetConfigurationProperty<string>(); }
      set { SetConfigurationProperty(value); }
    }
  }
}