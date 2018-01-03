using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Configuration;

namespace SolutionInspector.Api.Rules
{
  /// <summary>
  ///   Base class for rules that verify the contents of the project configuration file (App.config/Web.config).
  /// </summary>
  public abstract class ProjectConfigRuleBase : ProjectRule
  {
    /// <summary>
    ///   Controls whether to report a violation when no configuration file can be found for the project or if the rule should just be skipped in that case.
    /// </summary>
    [ConfigurationValue(IsOptional = true, DefaultValue = "true")]
    public bool ReportViolationOnMissingConfigurationFile
    {
      get => GetConfigurationValue<bool>();
      set => SetConfigurationValue(value);
    }

    /// <inheritdoc cref="Evaluate(IProject)" />
    public override IEnumerable<IRuleViolation> Evaluate (IProject target)
    {
      var configurationItem = target.ConfigurationProjectItem;

      if (configurationItem == null)
        return ReportViolationOnMissingConfigurationFile
          ? new[] { new RuleViolation(this, target, $"For the project '{target.Name}' no configuration file could be found.") }
          : Enumerable.Empty<IRuleViolation>();

      return Evaluate(configurationItem, configurationItem.ConfigurationXml);
    }

    /// <summary>
    ///   Checks the project configuration file (<paramref name="target" />) and its contents (<paramref name="configurationXml" />).
    /// </summary>
    /// <returns></returns>
    protected abstract IEnumerable<IRuleViolation> Evaluate (IConfigurationProjectItem target, XDocument configurationXml);
  }
}