using System;
using System.Collections.Generic;
using System.Linq;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;

namespace SolutionInspector.DefaultRules
{
  /// <summary>
  ///   Verifies that every non-development NuGet package reference is also represented by a DLL reference in the <see cref="IProject" />.
  /// </summary>
  public class NonDevelopmentNuGetPackagesMustAlsoBeRepresentedByAReferenceRule : ProjectRule
  {
    /// <inheritdoc />
    public override IEnumerable<IRuleViolation> Evaluate (IProject target)
    {
      var references = target.NuGetReferences.ToDictionary(r => r.Package);
      var nonDevelopmentNuGetPackages = target.NuGetPackages.Where(p => !p.IsDevelopmentDependency);

      return from nuGetPackage in nonDevelopmentNuGetPackages
        where !references.ContainsKey(nuGetPackage)
        select new RuleViolation(this, target, $"For the NuGet package '{nuGetPackage.PackageDirectoryName}', no DLL reference could be found.");
    }
  }
}