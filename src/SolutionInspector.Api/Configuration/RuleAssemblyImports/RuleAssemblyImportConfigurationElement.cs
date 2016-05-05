using System;
using System.Configuration;
using SolutionInspector.Api.Configuration.Infrastructure;

namespace SolutionInspector.Api.Configuration.RuleAssemblyImports
{
  public interface IRuleAssemblyImport
  {
    string Path { get; }
  }

  internal class RuleAssemblyImportConfigurationElement : KeyedConfigurationElement<string>, IRuleAssemblyImport
  {
    [ConfigurationProperty("path")]
    public string Path => Key;

    public override string KeyName => "path";
  }
}