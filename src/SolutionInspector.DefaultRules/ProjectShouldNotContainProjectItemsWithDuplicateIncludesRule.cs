using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using JetBrains.Annotations;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;
using SolutionInspector.Commons.Extensions;

namespace SolutionInspector.DefaultRules
{
  /// <summary>
  ///   Verifies that all <see cref="IProjectItem" />s included in the project are unique.
  /// </summary>
  [Description ("Verifies that all project items included in the project are unique.")]
  public class ProjectShouldNotContainProjectItemsWithDuplicateIncludesRule : ProjectRule
  {
    /// <inheritdoc />
    public override IEnumerable<IRuleViolation> Evaluate ([NotNull] IProject target)
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