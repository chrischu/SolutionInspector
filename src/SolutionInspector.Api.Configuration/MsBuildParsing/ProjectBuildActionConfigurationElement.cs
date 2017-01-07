namespace SolutionInspector.Api.Configuration.MsBuildParsing
{
  internal class ProjectBuildActionConfigurationElement : KeyedConfigurationElement<string>
  {
    [System.Configuration.ConfigurationProperty ("name")]
    public string Name => Key;

    public override string KeyName => "name";
  }
}