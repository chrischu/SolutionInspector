using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using SolutionInspector.Api.Extensions;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;
using SolutionInspector.Api.Utilities;

namespace SolutionInspector.DefaultRules
{
  /// <summary>
  ///   Verifies that a project's property has the expected value in the build configurations matched by the
  ///   <see cref="ProjectBuildConfigurationDependentPropertyRuleConfiguration.BuildConfigurationFilter" />.
  /// </summary>
  public class ProjectBuildConfigurationDependentPropertyRule : ConfigurableProjectRule<ProjectBuildConfigurationDependentPropertyRuleConfiguration>
  {
    /// <inheritdoc />
    public ProjectBuildConfigurationDependentPropertyRule (ProjectBuildConfigurationDependentPropertyRuleConfiguration configuration)
        : base(configuration)
    {
    }

    /// <inheritdoc />
    public override IEnumerable<IRuleViolation> Evaluate (IProject target)
    {
      var matchingBuildConfigs = target.BuildConfigurations.Where(c => Configuration.BuildConfigurationFilter.IsMatch(c));

      foreach (var matchingBuildConfig in matchingBuildConfigs)
      {
        var properties = target.Advanced.EvaluateProperties(matchingBuildConfig);

        var actualValue = properties.GetValueOrDefault(Configuration.Property)?.Value;

        if (actualValue != Configuration.ExpectedValue)
          yield return
              new RuleViolation(
                  this,
                  target,
                  $"Unexpected value for property '{Configuration.Property}' in build configuration '{matchingBuildConfig}', " +
                  $"was '{actualValue ?? "<null>"}' but should be '{Configuration.ExpectedValue}'.");
      }
    }
  }

  /// <summary>
  ///   Configuration for the <see cref="ProjectBuildConfigurationDependentPropertyRule" />.
  /// </summary>
  public class ProjectBuildConfigurationDependentPropertyRuleConfiguration : ConfigurationElement
  {
    /// <summary>
    ///   Controlls in which build configurations the <see cref="Property" /> is checked against the <see cref="ExpectedValue" />.
    /// </summary>
    [TypeConverter (typeof (BuildConfigurationFilterConverter))]
    [ConfigurationProperty ("buildConfigurationFilter", DefaultValue = "*|*", IsRequired = true)]
    public BuildConfigurationFilter BuildConfigurationFilter
    {
      get { return (BuildConfigurationFilter) this["buildConfigurationFilter"]; }
      set { this["buildConfigurationFilter"] = value; }
    }

    /// <summary>
    ///   The property to check.
    /// </summary>
    [ConfigurationProperty ("property", DefaultValue = "", IsRequired = true)]
    public string Property
    {
      get { return (string) this["property"]; }
      set { this["property"] = value; }
    }

    /// <summary>
    ///   The expected property value to check against.
    /// </summary>
    [ConfigurationProperty ("expectedValue", DefaultValue = "", IsRequired = true)]
    public string ExpectedValue
    {
      get { return (string) this["expectedValue"]; }
      set { this["expectedValue"] = value; }
    }
  }
}