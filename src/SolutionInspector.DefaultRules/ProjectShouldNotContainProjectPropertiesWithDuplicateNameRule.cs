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
  ///   Verifies that all <see cref="IProjectProperty" />s included in the project have unique names.
  /// </summary>
  [Description ("Verifies that all project properties included in the project have unique names.")]
  public class ProjectShouldNotContainProjectPropertiesWithDuplicateNameRule : ProjectRule
  {
    /// <inheritdoc />
    public override IEnumerable<IRuleViolation> Evaluate ([NotNull] IProject target)
    {
      foreach (var property in target.Advanced.Properties.Values)
      {
        var groupedByCondition = property.Occurrences.GroupBy(o => o.Condition);

        foreach (var group in groupedByCondition.Where(g => g.ContainsMoreThanOne()))
          yield return new RuleViolation(
            this,
            target,
            $"There are multiple project properties with name '{property.Name}' and the same conditions in the following locations: " +
            $"{group.ConvertAndJoin(i => i.Location, ", ")}.");
      }
    }
  }
}