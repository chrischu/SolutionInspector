using System.Collections.Generic;
using System.Configuration;
using System.Xml.Linq;
using System.Xml.XPath;
using SolutionInspector.ObjectModel;
using SolutionInspector.Rules;

namespace SolutionInspector.DefaultRules
{
  /// <summary>
  /// Checks that the configuration file for the project (App.config/Web.config) has the correct target framework version/SKU set.
  /// </summary>
  public class ProjectConfigFileShouldHaveCorrectFrameworkVersionRule : ProjectConfigRuleBase<ProjectConfigurationFileShouldHaveCorrectFrameworkVersionRuleConfiguration>
  {
    /// <inheritdoc />
    public ProjectConfigFileShouldHaveCorrectFrameworkVersionRule(ProjectConfigurationFileShouldHaveCorrectFrameworkVersionRuleConfiguration configuration)
        : base(configuration)
    {
    }

    /// <inheritdoc />
    protected override IEnumerable<IRuleViolation> Evaluate(IProject target, IConfigurationProjectItem configurationItem, XDocument configurationXml)
    {
      var supportedRuntimeElement = configurationXml.XPathSelectElement("/configuration/startup/supportedRuntime");

      var version = supportedRuntimeElement.Attribute("version").Value;
      if (version != Configuration.ExpectedVersion)
        yield return
            new RuleViolation(
                this,
                target,
                $"Unexpected value for supported runtime version, was '{version}' but should be '{Configuration.ExpectedVersion}'.");

      var sku = supportedRuntimeElement.Attribute("sku").Value;
      if (sku != Configuration.ExpectedSKU)
        yield return
            new RuleViolation(
                this,
                target,
                $"Unexpected value for supported runtime SKU, was '{sku}' but should be '{Configuration.ExpectedSKU}'.");
    }
  }

  /// <summary>
  /// Configuration for the <see cref="ProjectConfigFileShouldHaveCorrectFrameworkVersionRule"/>.
  /// </summary>
  public class ProjectConfigurationFileShouldHaveCorrectFrameworkVersionRuleConfiguration : ProjectConfigRuleConfigurationBase
  {
    /// <summary>
    /// The expected framework version (e.g. "v4.0")
    /// </summary>
    [ConfigurationProperty("expectedVersion", DefaultValue = "", IsRequired = true)]
    public string ExpectedVersion
    {
      get { return (string) this["expectedVersion"]; }
      set { this["expectedVersion"] = value; }
    }

    /// <summary>
    /// The expected SKU (e.g. ".NETFramework,Version=v4.6.1").
    /// </summary>
    [ConfigurationProperty("expectedSKU", DefaultValue = "", IsRequired = true)]
    public string ExpectedSKU
    {
      get { return (string)this["expectedSKU"]; }
      set { this["expectedSKU"] = value; }
    }
  }
}