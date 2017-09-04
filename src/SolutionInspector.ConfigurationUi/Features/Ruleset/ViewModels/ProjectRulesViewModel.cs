using System.Collections.Generic;

namespace SolutionInspector.ConfigurationUi.Features.Ruleset.ViewModels
{
  internal class ProjectRulesViewModel : RuleTypeViewModelBase<ProjectRuleGroupViewModel>
  {
    public ProjectRulesViewModel (IEnumerable<ProjectRuleGroupViewModel> ruleGroups)
      : base(ruleGroups)
    {
    }
  }
}