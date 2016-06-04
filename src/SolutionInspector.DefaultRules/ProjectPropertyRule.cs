using System;
using System.Collections.Generic;
using System.Configuration;
using SolutionInspector.Api.Extensions;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;

namespace SolutionInspector.DefaultRules
{
  /// <summary>
  ///   Verifies that a project's property has the expected value.
  /// </summary>
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