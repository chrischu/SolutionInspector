using System.Collections.Generic;

namespace SolutionInspector.ConfigurationUi.Features.Ruleset.ViewModels
{
  internal class ProjectItemRulesViewModel : RuleTypeViewModelBase<ProjectItemRuleGroupViewModel>
  {
    public ProjectItemRulesViewModel (IEnumerable<ProjectItemRuleGroupViewModel> ruleGroups)
      : base(ruleGroups)
    {
    }
  }
}