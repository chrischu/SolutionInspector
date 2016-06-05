using System;
using System.Collections.Generic;
using System.Linq;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Utilities;

namespace SolutionInspector.Api.Rules
{
  internal class FilteringProjectItemRuleProxy : IProjectItemRule
  {
    private readonly INameFilter _appliesTo;
    private readonly INameFilter _inProject;
    private readonly IProjectItemRule _rule;

    public FilteringProjectItemRuleProxy (INameFilter appliesTo, INameFilter inProject, IProjectItemRule rule)
    {
      _appliesTo = appliesTo;
      _inProject = inProject;
      _rule = rule;
    }

    public IEnumerable<IRuleViolation> Evaluate (IProjectItem target)
    {
      if (_inProject.IsMatch(target.Project.Name) && _appliesTo.IsMatch(target.Name))
        return _rule.Evaluate(target);

      return Enumerable.Empty<IRuleViolation>();
    }
  }
}