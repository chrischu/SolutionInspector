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
  ///   Verifies that every project has a unique GUID.
  /// </summary>
  [Description ("Verifies that every project has a unique GUID.")]
  public class ProjectGuidsMustBeUniqueRule : SolutionRule
  {
    /// <inheritdoc />
    public override IEnumerable<IRuleViolation> Evaluate ([NotNull] ISolution target)
    {
      return target.Projects.GroupBy(p => p.Guid).Where(g => g.ContainsMoreThanOne()).Select(
        g =>
        {
          var projectNames = g.ConvertAndJoin(p => $"'{p.Name}'", ", ");
          return new RuleViolation(this, target, $"The project GUID '{g.Key}' is used in multiple projects ({projectNames}).");
        });
    }
  }
}