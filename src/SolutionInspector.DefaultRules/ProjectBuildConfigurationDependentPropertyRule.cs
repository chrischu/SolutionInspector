using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using JetBrains.Annotations;
using SolutionInspector.Api.Configuration;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;
using SolutionInspector.Commons.Extensions;
using SolutionInspector.Configuration.Attributes;

namespace SolutionInspector.DefaultRules
{
  /// <summary>
  ///   Verifies that a project's property has the expected value in the build configurations matched by the <see cref="BuildConfigurationFilter" />.
  /// </summary>
  [Description ("Verifies that a project's property has the expected value in the build configurations matched by the" +
                "provided 'buildConfigurationFilter'.")]
  public class ProjectBuildConfigurationDependentPropertyRule : ProjectRule
  {
    /// <summary>
    ///   Controls in which build configurations the <see cref="Property" /> is checked against the <see cref="ExpectedValue" />.
    /// </summary>
    [ConfigurationValue]
    [Description ("Controls in which build configurations the 'property' is checked against the 'expectedValue'.")]
    public BuildConfigurationFilter BuildConfigurationFilter
    {
      get => GetConfigurationValue<BuildConfigurationFilter>();
      set => SetConfigurationValue(value);
    }

    /// <summary>
    ///   Name of the property to check.
    /// </summary>
    [ConfigurationValue]
    [Description ("Name of the property to check.")]
    public string Property
    {
      get => GetConfigurationValue<string>();
      set => SetConfigurationValue(value);
    }

    /// <summary>
    ///   The expected property value to check against.
    /// </summary>
    [ConfigurationValue]
    [Description ("The expected property value to check against.")]
    public string ExpectedValue
    {
      get => GetConfigurationValue<string>();
      set => SetConfigurationValue(value);
    }

    /// <inheritdoc />
    public override IEnumerable<IRuleViolation> Evaluate ([NotNull] IProject target)
    {
      var matchingBuildConfigs = target.BuildConfigurations.Where(c => BuildConfigurationFilter.IsMatch(c));

      foreach (var matchingBuildConfig in matchingBuildConfigs)
      {
        var properties = target.Advanced.EvaluateProperties(matchingBuildConfig);

        var actualValue = properties.GetValueOrDefault(Property)?.Value;

        if (actualValue != ExpectedValue)
          yield return
              new RuleViolation(
                this,
                target,
                $"Unexpected value for property '{Property}' in build configuration '{matchingBuildConfig}', " +
                $"was '{actualValue ?? "<null>"}' but should be '{ExpectedValue}'.");
      }
    }
  }
}