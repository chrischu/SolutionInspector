using System;
using System.Collections.Generic;
using System.Linq;
using SolutionInspector.Api.Extensions;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;

namespace SolutionInspector.DefaultRules
{
  /// <summary>
  ///   Verifies that all <see cref="IProjectItem" />s included in the project are unique.
  /// </summary>
  public class ProjectShouldNotContainProjectItemsWithDuplicateIncludesRule : ProjectRule
  {
    /// <inheritdoc />
    public override IEnumerable<IRuleViolation> Evaluate (IProject target)
    {
      return target.ProjectItems.GroupBy(i => i.Include.Evaluated).Where(g => g.ContainsMoreThanOne()).Select(
          group => new RuleViolation(
              this,
              target,
              $"There are multiple project items with include '{group.Key}' in the following locations: " +
              $"{string.Join(", ", group.Select(i => i.Location))}."));
    }
  }
}