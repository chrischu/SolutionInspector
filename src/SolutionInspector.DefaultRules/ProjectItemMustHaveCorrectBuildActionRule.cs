using System.Collections.Generic;
using System.ComponentModel;
using JetBrains.Annotations;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;
using SolutionInspector.Configuration.Attributes;

namespace SolutionInspector.DefaultRules
{
  /// <summary>
  ///   Verifies project items have the build action set that is configured via <see cref="ExpectedBuildAction" />.
  /// </summary>
  [Description ("Verifies project items have the build action set that is configured in 'expectedBuildAction'.")]
  public class ProjectItemMustHaveCorrectBuildActionRule : ProjectItemRule
  {
    /// <summary>
    ///   The expected build action.
    /// </summary>
    [ConfigurationValue]
    [Description ("The expected build action.")]
    public string ExpectedBuildAction
    {
      get => GetConfigurationValue<string>();
      set => SetConfigurationValue(value);
    }

    /// <inheritdoc />
    public override IEnumerable<IRuleViolation> Evaluate ([NotNull] IProjectItem target)
    {
      if (target.BuildAction != ExpectedBuildAction)
        yield return
            new RuleViolation(
              this,
              target,
              $"Unexpected build action was '{target.BuildAction}', but should be '{ExpectedBuildAction}'.");
    }
  }
}