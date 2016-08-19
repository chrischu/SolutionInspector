using SolutionInspector.Api.Configuration;

namespace SolutionInspector.Configuration.MsBuildParsing
{
  internal class ProjectBuildActionConfigurationElement : KeyedConfigurationElement<string>
  {
    [System.Configuration.ConfigurationProperty ("name")]
    public string Name => Key;

    public override string KeyName => "name";
  }
}