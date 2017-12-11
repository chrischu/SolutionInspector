using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;
using System.Xml.XPath;
using JetBrains.Annotations;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;
using SolutionInspector.Configuration;

namespace SolutionInspector.DefaultRules
{
  /// <summary>
  ///   Checks that the configuration file for the project (App.config/Web.config) has the correct target framework version/SKU set.
  /// </summary>
  [Description ("Checks that the configuration file for the project (App.config/Web.config) has the correct target framework version/SKU set.")]
  public class ProjectConfigFileShouldHaveCorrectFrameworkVersionRule
      : ProjectConfigRuleBase<ProjectConfigurationFileShouldHaveCorrectFrameworkVersionRuleConfiguration>
  {
    /// <inheritdoc />
    public ProjectConfigFileShouldHaveCorrectFrameworkVersionRule (
      ProjectConfigurationFileShouldHaveCorrectFrameworkVersionRuleConfiguration configuration)
      : base(configuration)
    {
    }

    /// <inheritdoc />
    protected override IEnumerable<IRuleViolation> Evaluate ([NotNull] IConfigurationProjectItem target, [NotNull] XDocument configurationXml)
    {
      var supportedRuntimeElement = configurationXml.XPathSelectElement("/configuration/startup/supportedRuntime");

      if (supportedRuntimeElement == null)
      {
        yield return new RuleViolation(this, target, "No explicit configuration for the supported runtime version/SKU could be found.");
      }
      else
      {
        var version = supportedRuntimeElement.Attribute("version")?.Value;
        if (version != Configuration.ExpectedVersion)
          yield return
              new RuleViolation(
                this,
                target,
                $"Unexpected value for supported runtime version, was '{version ?? "<null>"}' but should be '{Configuration.ExpectedVersion}'.");

        var sku = supportedRuntimeElement.Attribute("sku")?.Value;
        if (sku != Configuration.ExpectedSKU)
          yield return
              new RuleViolation(
                this,
                target,
                $"Unexpected value for supported runtime SKU, was '{sku ?? "<null>"}' but should be '{Configuration.ExpectedSKU}'.");
      }
    }
  }

  /// <summary>
  ///   Configuration for the <see cref="ProjectConfigFileShouldHaveCorrectFrameworkVersionRule" />.
  /// </summary>
  public class ProjectConfigurationFileShouldHaveCorrectFrameworkVersionRuleConfiguration : ProjectConfigRuleConfigurationBase
  {
    /// <summary>
    ///   The expected framework version (e.g. 'v4.0')
    /// </summary>
    [ConfigurationValue]
    [Description ("The expected framework version (e.g. 'v4.0')")]
    public string ExpectedVersion
    {
      get => GetConfigurationValue<string>();
      set => SetConfigurationValue(value);
    }

    /// <summary>
    ///   The expected SKU (e.g. '.NETFramework,Version=v4.6.1').
    /// </summary>
    [ConfigurationValue]
    [Description ("The expected SKU (e.g. '.NETFramework,Version=v4.6.1').")]
    public string ExpectedSKU
    {
      get => GetConfigurationValue<string>();
      set => SetConfigurationValue(value);
    }
  }
}