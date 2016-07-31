using System;
using System.Collections.Generic;
using System.Linq;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;

namespace SolutionInspector.DefaultRules
{
  /// <summary>
  ///   Verifies that all non-development <see cref="INuGetReference" />s in the project have their 'IsPrivate' flag (also referred to as
  ///   'Copy Local') set to true.
  /// </summary>
  public class NonDevelopmentNuGetReferencesShouldHaveIsPrivateSetToTrueRule : ProjectRule
  {
    /// <inheritdoc />
    public override IEnumerable<IRuleViolation> Evaluate (IProject target)
    {
      return target.NuGetReferences.Where(r => !r.Package.IsDevelopmentDependency && !r.IsPrivate).Select(
          r => new RuleViolation(
              this,
              target,
              $"The NuGet reference to package '{r.Package.Id}' is not a development dependency and therefore should has its 'IsPrivate' flag set " +
              "to true."));
    }
  }
}