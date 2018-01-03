using System.Configuration;

namespace SolutionInspector.Api.Configuration.MsBuildParsing
{
  internal class ProjectBuildActionConfigurationElement : KeyedConfigurationElement<string>
  {
    [ConfigurationProperty ("name")]
    public string Name => Key;

    public override string KeyName => "name";
  }
}