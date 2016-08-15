using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;

namespace SolutionInspector.DefaultRules
{
  /// <summary>
  ///   Verifies project items have the build action set that is configured via <see cref="ProjectItemMustHaveCorrectBuildActionRuleConfiguration" />.
  /// </summary>
  [Description ("Verifies project items have the build action set that is configured in 'expectedBuildAction'.")]
  public class ProjectItemMustHaveCorrectBuildActionRule : ConfigurableProjectItemRule<ProjectItemMustHaveCorrectBuildActionRuleConfiguration>
  {
    /// <inheritdoc />
    public ProjectItemMustHaveCorrectBuildActionRule (ProjectItemMustHaveCorrectBuildActionRuleConfiguration configuration)
        : base(configuration)
    {
    }

    public override IEnumerable<IRuleViolation> Evaluate (IProjectItem target)
    {
      if (target.BuildAction != Configuration.ExpectedBuildAction)
        yield return
            new RuleViolation(
                this,
                target,
                $"Unexpected build action was '{target.BuildAction}', but should be '{Configuration.ExpectedBuildAction}'.");
    }
  }

  /// <summary>
  ///   Configuration for the <see cref="ProjectItemMustHaveCorrectBuildActionRule" />.
  /// </summary>
  public class ProjectItemMustHaveCorrectBuildActionRuleConfiguration : ConfigurationElement
  {
    /// <summary>
    ///   The expected build action.
    /// </summary>
    [ConfigurationProperty ("expectedBuildAction", DefaultValue = "", IsRequired = true)]
    [Description ("The expected build action.")]
    public string ExpectedBuildAction
    {
      get { return (string) this["expectedBuildAction"]; }
      set { this["expectedBuildAction"] = value; }
    }
  }
}