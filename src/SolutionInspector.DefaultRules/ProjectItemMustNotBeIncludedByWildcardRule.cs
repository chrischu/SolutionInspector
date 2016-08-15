using System.Collections.Generic;
using System.ComponentModel;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;

namespace SolutionInspector.DefaultRules
{
  /// <summary>
  ///   Verifies that the <see cref="IProjectItem" /> is NOT included via wildcard.
  /// </summary>
  [Description ("Verifies that the project item is NOT included via wildcard.")]
  public class ProjectItemMustNotBeIncludedByWildcardRule : ProjectItemRule
  {
    /// <inheritdoc />
    public override IEnumerable<IRuleViolation> Evaluate (IProjectItem target)
    {
      if (target.IsIncludedByWildcard)
        yield return new RuleViolation(this, target, $"Project item '{target.Identifier}' must NOT be included via wildcard.");
    }
  }
}