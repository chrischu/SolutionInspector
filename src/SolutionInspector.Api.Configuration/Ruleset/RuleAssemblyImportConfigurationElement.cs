using SolutionInspector.Configuration;

namespace SolutionInspector.Api.Configuration.Ruleset
{
  public interface IRuleAssemblyImportConfiguration
  {
    string Path { get; }
  }

  public class RuleAssemblyImportConfigurationElement : ConfigurationElement, IRuleAssemblyImportConfiguration
  {
    [ConfigurationValue]
    public string Path => GetConfigurationValue<string>();
  }
}