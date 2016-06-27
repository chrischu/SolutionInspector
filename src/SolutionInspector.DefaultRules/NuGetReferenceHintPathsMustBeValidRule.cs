using System;
using System.Collections.Generic;
using System.Linq;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;

namespace SolutionInspector.DefaultRules
{
  /// <summary>
  ///   Verifies that all <see cref="INuGetReference" />s in the project have correct hint paths (pointing to an actually
  ///   existing file).
  /// </summary>
  public class NuGetReferenceHintPathsMustBeValidRule : ProjectRule
  {
    /// <inheritdoc />
    public override IEnumerable<IRuleViolation> Evaluate (IProject target)
    {
      return target.NuGetReferences.Where(r => !r.DllFile.Exists).Select(
          r => new RuleViolation(
              this,
              target,
              $"The NuGet reference to package '{r.Package.Id}' has an invalid hint path ('{r.HintPath}')."));
    }
  }
}