using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using SolutionInspector.Api.Configuration.RuleAssemblyImports;
using SolutionInspector.Api.Configuration.Rules;

namespace SolutionInspector.Api.Configuration
{
  /// <summary>
  ///   Rule configuration for a solution that is to be processed by SolutionInspector.
  /// </summary>
  public interface ISolutionInspectorRuleset
  {
    /// <summary>
    ///   Allows importing rule assemblies to enable access to rules contained in them.
    /// </summary>
    IRuleAssemblyImportsConfiguration RuleAssemblyImports { get; }

    /// <summary>
    ///   Configures the various rules that are checked against when running the SolutionInspector.
    /// </summary>
    IRulesConfiguration Rules { get; }
  }

  [UsedImplicitly]
  [ExcludeFromCodeCoverage]
  internal class SolutionInspectorRuleset : ConfigurationSectionGroup, ISolutionInspectorRuleset
  {
    public IRuleAssemblyImportsConfiguration RuleAssemblyImports => (RuleAssemblyImportsConfigurationSection)Sections["ruleAssemblyImports"];
    public IRulesConfiguration Rules => (RulesConfigurationSection)Sections["rules"];
  }
}