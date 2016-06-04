using System;
using System.Collections.Generic;
using System.Linq;
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
      return target.Advanced.Properties.Values.Where(p => p.Occurrences.Count > 1).Select(
          property => new RuleViolation(
              this,
              target,
              $"There are multiple project properties with name '{property.Name}' in the following locations: " +
              $"{string.Join(", ", property.Occurrences.Select(i => i.Location))}."));
    }
  }
}