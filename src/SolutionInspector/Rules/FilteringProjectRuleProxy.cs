using System.Collections.Generic;
using System.Linq;
using SolutionInspector.Api;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;

namespace SolutionInspector.Rules
{
  internal class FilteringProjectRuleProxy : IProjectRule
  {
    private readonly INameFilter _filter;
    private readonly IProjectRule _rule;

    public FilteringProjectRuleProxy (INameFilter filter, IProjectRule rule)
    {
      _filter = filter;
      _rule = rule;
    }

    public IEnumerable<IRuleViolation> Evaluate (IProject target)
    {
      if (_filter.IsMatch(target.Name))
        return _rule.Evaluate(target);

      return Enumerable.Empty<IRuleViolation>();
    }
  }
}