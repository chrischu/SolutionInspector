using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;
using SolutionInspector.Api.ObjectModel;

namespace SolutionInspector.Api.Rules
{
  /// <summary>
  /// Base class for rules that verify the contents of the project configuration file (App.config/Web.config).
  /// </summary>
  public abstract class ProjectConfigRuleBase<TConfiguration> : ConfigurableRule<IProject, TConfiguration>, IProjectRule
      where TConfiguration : ProjectConfigRuleConfigurationBase
  {
    /// <inheritdoc />
    protected ProjectConfigRuleBase(TConfiguration configuration)
        : base(configuration)
    {
    }

    /// <inheritdoc />
    public override IEnumerable<IRuleViolation> Evaluate(IProject target)
    {
      var configurationItem = target.ConfigurationProjectItem;

      if (configurationItem == null)
        return Configuration.ReportViolationOnMissingConfigurationFile
            ? new[] { new RuleViolation(this, target, $"For the project '{target.Name}' no configuration file could be found.") }
            : Enumerable.Empty<IRuleViolation>();

      return Evaluate(configurationItem, configurationItem.ConfigurationXml);
    }

    /// <summary>
    /// Checks the project configuration file (<paramref name="target"/>) and its contents (<paramref name="configurationXml"/>).
    /// </summary>
    /// <returns></returns>
    protected abstract IEnumerable<IRuleViolation> Evaluate(IConfigurationProjectItem target, XDocument configurationXml);
  }

  /// <summary>
  /// Base class for configuration for <see cref="ProjectConfigRuleBase{TConfiguration}"/>.
  /// </summary>
  public abstract class ProjectConfigRuleConfigurationBase : ConfigurationElement
  {
    /// <summary>
    /// Controls whether to report a violation when no configuration file can be found for the project or if the rule should just be skipped in that case.
    /// </summary>
    [ConfigurationProperty("reportViolationOnMissingConfigurationFile")]
    public bool ReportViolationOnMissingConfigurationFile
    {
      get { return (bool) this["reportViolationOnMissingConfigurationFile"]; }
      set { this["reportViolationOnMissingConfigurationFile"] = value; }
    }
  }
}