using System.Configuration;
using JetBrains.Annotations;
using SolutionInspector.Configuration.MsBuildParsing;
using SolutionInspector.Configuration.Rules;

namespace SolutionInspector.Configuration
{
  /// <summary>
  /// Configuration for the SolutionInspector.
  /// </summary>
  public interface ISolutionInspectorConfiguration
  {
    /// <summary>
    /// Controls how SolutionInspector parses MSBuild files.
    /// </summary>
    IMsBuildParsingConfiguration MsBuildParsing { get; }

    /// <summary>
    /// Configures the various rules that are checked against when running the SolutionInspector.
    /// </summary>
    IRulesConfiguration Rules { get; }
  }

  [UsedImplicitly]
  internal class SolutionInspectorConfiguration : ConfigurationSectionGroup, ISolutionInspectorConfiguration
  {
    public IMsBuildParsingConfiguration MsBuildParsing => (MsBuildParsingConfigurationSection) Sections["msBuildParsing"];

    public IRulesConfiguration Rules => (RulesConfigurationSection) Sections["rules"];

    public static SolutionInspectorConfiguration Load()
    {
      var configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
      return (SolutionInspectorConfiguration) configuration.GetSectionGroup("solutionInspector");
    }
  }
}