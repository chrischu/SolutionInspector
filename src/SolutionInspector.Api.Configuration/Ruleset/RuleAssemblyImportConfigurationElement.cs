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
    public string Path
    {
      get { return GetConfigurationProperty<string>(); }
      set { SetConfigurationProperty(value); }
    }
  }
}