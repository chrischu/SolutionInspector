namespace SolutionInspector.ConfigurationUi.ViewModel
{
  //internal class RulesetViewModel : ViewModelBase
  //{
  //  private readonly ISaveableSolutionInspectorRuleset _ruleset;

  //  public RulesetViewModel (ISaveableSolutionInspectorRuleset ruleset)
  //  {
  //    _ruleset = ruleset;
  //    Imports = new ImportsViewModel(_ruleset.RuleAssemblyImports);
  //    Rules = new RulesViewModel(_ruleset.Rules);
  //  }

  //  public ImportsViewModel Imports { get; }
  //  public RulesViewModel Rules { get; }
  //}

  //internal class RulesViewModel : ViewModelBase
  //{
  //  private readonly IMutableRulesConfiguration _rules;

  //  public RulesViewModel (IMutableRulesConfiguration rules)
  //  {
  //    _rules = rules;
  //    SolutionRules = new SolutionRulesViewModel(rules.SolutionRules);
  //  }

  //  public SolutionRulesViewModel SolutionRules { get; }
  //}

  //internal class SolutionRulesViewModel : ViewModelBase
  //{
  //  private readonly IConfigurationCollection<IRuleConfiguration> _solutionRules;

  //  public SolutionRulesViewModel (IConfigurationCollection<IRuleConfiguration> solutionRules)
  //  {
  //    _solutionRules = solutionRules;
  //    Rules = new AdvancedObservableCollection<RuleConfigurationViewModel>(solutionRules.Select(r => new RuleConfigurationViewModel(r)));
  //  }

  //  public AdvancedObservableCollection<RuleConfigurationViewModel> Rules { get; }
  //}

  //internal class RuleConfigurationViewModel : ViewModelBase
  //{
  //  private readonly IRuleConfiguration _ruleConfiguration;

  //  public RuleConfigurationViewModel(IRuleConfiguration ruleConfiguration)
  //  {
  //    _ruleConfiguration = ruleConfiguration;
  //  }

  //  public string Name { get; }
  //}
}