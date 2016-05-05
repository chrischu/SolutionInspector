using System;
using System.Configuration;
using SolutionInspector.Api.Configuration.Infrastructure;

namespace SolutionInspector.Api.Configuration.MsBuildParsing
{
  internal class ProjectBuildActionConfigurationElement : KeyedConfigurationElement<string>
  {
    [ConfigurationProperty ("name")]
    public string Name => Key;

    public override string KeyName => "name";
  }
}