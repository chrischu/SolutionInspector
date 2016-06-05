using System;
using System.Collections.Generic;
using System.Linq;
using SolutionInspector.Api.Extensions;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;

namespace SolutionInspector.DefaultRules
{
  /// <summary>
  ///   Verifies that all <see cref="IProjectProperty" />s included in the project have unique names.
  /// </summary>
  public class ProjectShouldNotContainProjectPropertiesWithDuplicateNameRule : ProjectRule
  {
    /// <inheritdoc />
    public override IEnumerable<IRuleViolation> Evaluate (IProject target)
    {
      foreach (var property in target.Advanced.Properties.Values)
      {
        var groupedByCondition = property.Occurrences.GroupBy(o => o.Condition);

        foreach (var group in groupedByCondition.Where(g => g.ContainsMoreThanOne()))
        {
          yield return new RuleViolation(
              this,
              target,
              $"There are multiple project properties with name '{property.Name}' and the same conditions in the following locations: " +
              $"{string.Join(", ", group.Select(i => i.Location))}.");
        }
      }
    }
  }
}