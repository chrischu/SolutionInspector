using System.Collections.Generic;
using System.ComponentModel;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;
using SolutionInspector.Commons.Extensions;
using SolutionInspector.Configuration;

namespace SolutionInspector.DefaultRules
{
  /// <summary>
  ///   Verifies that a project's property has the expected value.
  /// </summary>
  [Description ("Verifies that a project's property has the expected value.")]
  public class ProjectPropertyRule : ConfigurableProjectRule<ProjectPropertyRuleConfiguration>
  {
    /// <inheritdoc />
    public ProjectPropertyRule (ProjectPropertyRuleConfiguration configuration)
      : base(configuration)
    {
    }

    /// <inheritdoc />
    public override IEnumerable<IRuleViolation> Evaluate (IProject target)
    {
      var actualValue = target.Advanced.Properties.GetValueOrDefault(Configuration.Property)?.DefaultValue;

      if (actualValue != Configuration.ExpectedValue)
        yield return
            new RuleViolation(
              this,
              target,
              $"Unexpected value for property '{Configuration.Property}', was '{actualValue ?? "<null>"}' " +
              $"but should be '{Configuration.ExpectedValue}'.");
    }
  }

  /// <summary>
  ///   Configuration for the <see cref="ProjectPropertyRule" />.
  /// </summary>
  public class ProjectPropertyRuleConfiguration : ConfigurationElement
  {
    /// <summary>
    ///   The property to check.
    /// </summary>
    [ConfigurationValue]
    [Description ("The property to check.")]
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