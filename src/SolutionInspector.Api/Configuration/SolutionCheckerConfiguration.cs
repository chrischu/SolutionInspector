using System;
using System.Configuration;
using JetBrains.Annotations;
using SolutionInspector.Api.Configuration.MsBuildParsing;
using SolutionInspector.Api.Configuration.RuleAssemblyImports;
using SolutionInspector.Api.Configuration.Rules;

namespace SolutionInspector.Api.Configuration
{
  /// <summary>
  ///   Configuration for the SolutionInspector.
  /// </summary>
  public interface ISolutionInspectorConfiguration
  {
    /// <summary>
    ///   Allows importing rule assemblies to enable access to rules contained in them.
    /// </summary>
    IRuleAssemblyImportsConfiguration RuleAssemblyImports { get; }

    /// <summary>
    ///   Controls how SolutionInspector parses MSBuild files.
    /// </summary>
    IMsBuildParsingConfiguration MsBuildParsing { get; }

    /// <summary>
    ///   Configures the various rules that are checked against when running the SolutionInspector.
    /// </summary>
    IRulesConfiguration Rules { get; }
  }

  [UsedImplicitly]
  internal class SolutionInspectorConfiguration : ConfigurationSectionGroup, ISolutionInspectorConfiguration
  {
    public IRuleAssemblyImportsConfiguration RuleAssemblyImports => (RuleAssemblyImportsConfigurationSection) Sections["ruleAssemblyImports"];

    public IMsBuildParsingConfiguration MsBuildParsing => (MsBuildParsingConfigurationSection) Sections["msBuildParsing"];

    public IRulesConfiguration Rules => (RulesConfigurationSection) Sections["rules"];

    public static SolutionInspectorConfiguration Load ()
    {
      var configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
      return (SolutionInspectorConfiguration) configuration.GetSectionGroup("solutionInspector");
    }
  }
}