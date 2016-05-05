using System.Collections.Generic;
using System.Linq;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;

namespace SolutionInspector.DefaultRules
{
  /// <summary>
  /// Verifies that every NuGet package is only referenced in one version across all projects.
  /// </summary>
  public class NuGetPackagesShouldHaveOneVersionAcrossSolutionRule : SolutionRule
  {
    /// <inheritdoc />
    public override IEnumerable<IRuleViolation> Evaluate(ISolution target)
    {
      var nuGetPackagesWithMultipleVersions =
          target.Projects.SelectMany(p => p.NuGetPackages)
              .GroupBy(p => p.Id, p => p.FullVersionString)
              .Where(g => g.Distinct().Count() > 1);

      return from @group in nuGetPackagesWithMultipleVersions
        let versions = @group.Select(v => $"'{v}'")
        select new RuleViolation(
            this,
            target,
            $"The NuGet package '{@group.Key}' is referenced " +
            $"in more than one version ({string.Join(", ", versions)}).");
    }
  }
}