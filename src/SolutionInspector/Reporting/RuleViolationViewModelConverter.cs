using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolutionInspector.Api.Extensions;
using SolutionInspector.Api.Rules;

namespace SolutionInspector.Reporting
{
  internal interface IRuleViolationViewModelConverter
  {
    IEnumerable<RuleViolationViewModel> Convert (IEnumerable<IRuleViolation> violations);
  }

  [UsedImplicitly]
  internal class RuleViolationViewModelConverter : IRuleViolationViewModelConverter
  {
    public IEnumerable<RuleViolationViewModel> Convert (IEnumerable<IRuleViolation> violations)
    {
      return violations.Select(v => new { Rule = v.Rule.GetType().Name.RemoveSuffix("Rule"), Target = v.Target.Identifier, v.Message })
          .OrderBy(v => v.Rule).ThenBy(v => v.Target)
          .Select((x, i) => new RuleViolationViewModel(i + 1, x.Rule, x.Target, x.Message));
    }
  }
}