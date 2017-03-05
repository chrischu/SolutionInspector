namespace SolutionInspector.ConfigurationUi.Features.Ruleset.ViewModels
{
  internal class SolutionRulesViewModel : RuleTypeViewModelBase<SolutionRuleGroupViewModel>
  {
    public SolutionRulesViewModel (SolutionRuleGroupViewModel ruleGroup) : base(new[] { ruleGroup})
    {
    }
  }
}