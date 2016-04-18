using System.Configuration;
using SolutionInspector.Configuration.Infrastructure;

namespace SolutionInspector.Configuration.MsBuildParsing
{
  internal class ProjectBuildActionConfigurationElement : KeyedConfigurationElement<string>
  {
    [ConfigurationProperty("name")]
    public string Name => Key;

    public override string KeyName => "name";
  }
}