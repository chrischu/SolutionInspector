using System.Collections.Generic;
using System.ComponentModel;
using JetBrains.Annotations;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;

namespace SolutionInspector.DefaultRules
{
  /// <summary>
  ///   Verifies that the <see cref="IProjectItem" /> is included via wildcard.
  /// </summary>
  [Description ("Verifies that the project item is included via wildcard.")]
  public class ProjectItemMustBeIncludedByWildcardRule : ProjectItemRule
  {
    /// <inheritdoc />
    public override IEnumerable<IRuleViolation> Evaluate ([NotNull] IProjectItem target)
    {
      if (!target.IsIncludedByWildcard)
        yield return new RuleViolation(this, target, $"Project item '{target.Identifier}' must be included via wildcard.");
    }
  }
}