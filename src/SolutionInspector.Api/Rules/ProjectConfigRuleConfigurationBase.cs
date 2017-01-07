using SolutionInspector.Configuration;

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
    [ConfigurationValue]
    public bool ReportViolationOnMissingConfigurationFile
    {
      get { return GetConfigurationValue<bool>(); }
      set { SetConfigurationValue(value); }
    }
  }
}