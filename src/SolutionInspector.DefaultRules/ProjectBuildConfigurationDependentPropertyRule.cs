using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using SolutionInspector.Api.Configuration;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;
using SolutionInspector.Commons.Extensions;
using SolutionInspector.Configuration;

namespace SolutionInspector.DefaultRules
{
  /// <summary>
  ///   Verifies that a project's property has the expected value in the build configurations matched by the
  ///   <see cref="ProjectBuildConfigurationDependentPropertyRuleConfiguration.BuildConfigurationFilter" />.
  /// </summary>
  [Description ("Verifies that a project's property has the expected value in the build configurations matched by the" +
                "provided 'buildConfigurationFilter'.")]
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
    ///   Controls in which build configurations the <see cref="Property" /> is checked against the <see cref="ExpectedValue" />.
    /// </summary>
    [ConfigurationValue]
    [Description ("Controls in which build configurations the 'property' is checked against the 'expectedValue'.")]
    public BuildConfigurationFilter BuildConfigurationFilter
    {
      get { return GetConfigurationValue<BuildConfigurationFilter>(); }
      set { SetConfigurationValue(value); }
    }

    /// <summary>
    ///   Name of the property to check.
    /// </summary>
    [ConfigurationValue]
    [Description ("Name of the property to check.")]
    public string Property
    {
      get { return GetConfigurationValue<string>(); }
      set { SetConfigurationValue(value); }
    }

    /// <summary>
    ///   The expected property value to check against.
    /// </summary>
    [ConfigurationValue]
    [Description ("The expected property value to check against.")]
    public string ExpectedValue
    {
      get { return GetConfigurationValue<string>(); }
      set { SetConfigurationValue(value); }
    }
  }
}