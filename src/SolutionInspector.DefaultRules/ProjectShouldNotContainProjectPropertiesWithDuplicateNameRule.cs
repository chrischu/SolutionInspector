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
    public override IEnumerable<IRuleViolation> Evaluate(IProject target)
    {
      var allProperties = target.Advanced.Properties.Cast<IProjectPropertyBase>().Concat(target.Advanced.ConditionalProperties);

      return allProperties.GroupBy(p => p.Name).Where(g => g.ContainsMoreThanOne()).Select(
          group => new RuleViolation(
              this,
              target,
              $"There are multiple project properties with name '{group.Key}' in the following locations: " +
              $"{string.Join(", ", group.Select(i => i.Location))}."));
    }
  }
}