using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SolutionInspector.Configuration;
using SolutionInspector.Configuration.Attributes;
using SolutionInspector.Configuration.Collections;

namespace SolutionInspector.Api.Configuration.Ruleset
{
  /// <summary>
  ///   Configuration for a ruleset, containing <see cref="RuleAssemblyImports" /> and <see cref="Rules" />.
  /// </summary>
  public interface IRulesetConfiguration
  {
    /// <summary>
    ///   The collection of paths to rule assemblies to be imported.
    /// </summary>
    IReadOnlyCollection<string> RuleAssemblyImports { get; }

    /// <summary>
    ///   The rules in the ruleset.
    /// </summary>
    IRulesConfiguration Rules { get; }
  }

  /// <inheritdoc cref="IRulesetConfiguration" />
  /// >
  public class RulesetConfigurationDocument : ConfigurationDocument, IRulesetConfiguration
  {
    /// <summary>
    ///   A collection of <see cref="RuleAssemblyImportConfigurationElement" />s.
    /// </summary>
    [ConfigurationCollection(ElementName = "import")]
    public IConfigurationElementCollection<RuleAssemblyImportConfigurationElement> RuleAssemblyImports
      => GetConfigurationCollection<RuleAssemblyImportConfigurationElement>();

    /// <summary>
    ///   A collection of <see cref="RulesConfigurationElement" />s.
    /// </summary>
    [ConfigurationSubelement]
    public RulesConfigurationElement Rules => GetConfigurationSubelement<RulesConfigurationElement>();

    [ExcludeFromCodeCoverage]
    IReadOnlyCollection<string> IRulesetConfiguration.RuleAssemblyImports => RuleAssemblyImports.Select(i => i.Path).ToArray();

    [ExcludeFromCodeCoverage]
    IRulesConfiguration IRulesetConfiguration.Rules => Rules;
  }
}