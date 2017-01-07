using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;
using SolutionInspector.Commons.Extensions;

namespace SolutionInspector.DefaultRules
{
  /// <summary>
  ///   Verifies that every project has a unique GUID.
  /// </summary>
  [Description ("Verifies that every project has a unique GUID.")]
  public class ProjectGuidsMustBeUniqueRule : SolutionRule
  {
    /// <inheritdoc />
    public override IEnumerable<IRuleViolation> Evaluate (ISolution target)
    {
      return target.Projects.GroupBy(p => p.Guid).Where(g => g.ContainsMoreThanOne()).Select(
        g =>
        {
          var projectNames = string.Join(", ", g.Select(p => $"'{p.Name}'"));
          return new RuleViolation(this, target, $"The project GUID '{g.Key}' is used in multiple projects ({projectNames}).");
        });
    }
  }
}