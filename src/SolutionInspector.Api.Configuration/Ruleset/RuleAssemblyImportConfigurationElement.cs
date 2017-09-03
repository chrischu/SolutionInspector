using JetBrains.Annotations;
using SolutionInspector.Configuration;

namespace SolutionInspector.Api.Configuration.Ruleset
{
  /// <summary>
  ///   Configuration for rule assembly imports.
  /// </summary>
  [PublicAPI]
  public interface IRuleAssemblyImportConfiguration
  {
    string Path { get; }
  }

  /// <inheritdoc cref="IRuleAssemblyImportConfiguration" />
  public class RuleAssemblyImportConfigurationElement : ConfigurationElement, IRuleAssemblyImportConfiguration
  {
    [ConfigurationValue]
    public string Path => GetConfigurationValue<string>();
  }
}