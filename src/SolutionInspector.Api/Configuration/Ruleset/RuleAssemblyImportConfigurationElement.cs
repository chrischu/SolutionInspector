using System;
using JetBrains.Annotations;
using SolutionInspector.Commons.Attributes;
using SolutionInspector.Configuration;

namespace SolutionInspector.Api.Configuration.Ruleset
{
  /// <summary>
  ///   Configuration for rule assembly imports.
  /// </summary>
  [PublicApi]
  public interface IRuleAssemblyImportConfiguration
  {
    /// <summary>
    ///   The path of the rule assembly to import.
    /// </summary>
    [CanBeNull]
    string Path { get; }
  }

  /// <inheritdoc cref="IRuleAssemblyImportConfiguration" />
  public class RuleAssemblyImportConfigurationElement : ConfigurationElement, IRuleAssemblyImportConfiguration
  {
    /// <inheritdoc />
    [CanBeNull]
    [ConfigurationValue]
    public string Path => GetConfigurationValue<string>();
  }
}