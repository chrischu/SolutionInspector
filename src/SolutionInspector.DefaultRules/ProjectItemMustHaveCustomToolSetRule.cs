using System.Collections.Generic;
using System.ComponentModel;
using JetBrains.Annotations;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;
using SolutionInspector.Configuration;

namespace SolutionInspector.DefaultRules
{
  /// <summary>
  ///   Verifies project items have custom tool and custom tool namespace set that is configured via
  ///   <see cref="ProjectItemMustHaveCustomToolSetRuleConfiguration" />.
  /// </summary>
  [Description ("Verifies project items have custom tool and custom tool namespace set that is configured via 'expectedCustomTool' " +
                "and 'expectedCustomToolNamespace'.")]
  public class ProjectItemMustHaveCustomToolSetRule : ConfigurableProjectItemRule<ProjectItemMustHaveCustomToolSetRuleConfiguration>
  {
    /// <inheritdoc />
    public ProjectItemMustHaveCustomToolSetRule (ProjectItemMustHaveCustomToolSetRuleConfiguration configuration)
      : base(configuration)
    {
    }

    /// <inheritdoc />
    public override IEnumerable<IRuleViolation> Evaluate ([NotNull] IProjectItem target)
    {
      if ((target.CustomTool ?? "") != Configuration.ExpectedCustomTool)
        yield return
            new RuleViolation(
              this,
              target,
              $"Unexpected value for custom tool, was '{target.CustomTool}' but should be '{Configuration.ExpectedCustomTool}'.");

      if ((target.CustomToolNamespace ?? "") != Configuration.ExpectedCustomToolNamespace)
        yield return
            new RuleViolation(
              this,
              target,
              $"Unexpected value for custom tool namespace, was '{target.CustomToolNamespace}' " +
              $"but should be '{Configuration.ExpectedCustomToolNamespace}'.");
    }
  }

  /// <summary>
  ///   Configuration for the <see cref="ProjectItemMustHaveCustomToolSetRule" />.
  /// </summary>
  public class ProjectItemMustHaveCustomToolSetRuleConfiguration : ConfigurationElement
  {
    /// <summary>
    ///   The expected custom tool.
    /// </summary>
    [ConfigurationValue]
    [Description ("The expected custom tool.")]
    public string ExpectedCustomTool
    {
      get => GetConfigurationValue<string>();
      set => SetConfigurationValue(value);
    }

    /// <summary>
    ///   The expected custom tool namespace.
    /// </summary>
    [ConfigurationValue]
    [Description ("The expected custom tool namespace.")]
    public string ExpectedCustomToolNamespace
    {
      get => GetConfigurationValue<string>();
      set => SetConfigurationValue(value);
    }
  }
}