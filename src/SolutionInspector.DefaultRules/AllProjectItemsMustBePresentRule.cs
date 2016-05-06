using System;
using System.Collections.Generic;
using System.Linq;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;

namespace SolutionInspector.DefaultRules
{
  /// <summary>
  ///   Verifies that all <see cref="IProjectItem" />s included in the project also exist in the file system.
  /// </summary>
  public class AllProjectItemsMustBePresentRule : ProjectRule
  {
    /// <inheritdoc />
    public override IEnumerable<IRuleViolation> Evaluate (IProject target)
    {
      return target.ProjectItems.Where(i => !i.File.Exists)
          .Select(i => new RuleViolation(this, target, $"Could not find project item '{i.OriginalInclude}'."));
    }
  }
}