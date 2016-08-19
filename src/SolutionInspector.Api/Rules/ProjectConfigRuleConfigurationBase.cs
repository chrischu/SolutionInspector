using System.Configuration;

namespace SolutionInspector.Api.Rules
{
  /// <summary>
  ///   Base class for configuration for <see cref="ProjectConfigRuleBase{TConfiguration}" />.
  /// </summary>
  public abstract class ProjectConfigRuleConfigurationBase : ConfigurationElement
  {
    /// <summary>
    ///   Controls whether to report a violation when no configuration file can be found for the project or if the rule should just be skipped in that case.
    /// </summary>
    [ConfigurationProperty ("reportViolationOnMissingConfigurationFile")]
    public bool ReportViolationOnMissingConfigurationFile
    {
      get { return (bool) this["reportViolationOnMissingConfigurationFile"]; }
      set { this["reportViolationOnMissingConfigurationFile"] = value; }
    }
  }
}