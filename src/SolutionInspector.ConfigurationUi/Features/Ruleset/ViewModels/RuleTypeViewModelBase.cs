using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;

namespace SolutionInspector.ConfigurationUi.Features.Ruleset.ViewModels
{
  internal class RuleTypeViewModelBase<T> : ViewModelBase
    where T : RuleGroupViewModel
  {
    private string _globalRulesFilter;
    private string _localRulesFilter;

    public RuleTypeViewModelBase (IEnumerable<T> ruleGroups)
    {
      RuleGroups = new ObservableCollection<T>(ruleGroups);
    }

    public ObservableCollection<T> RuleGroups { get; }

    public string GlobalFilter
    {
      get { return _globalRulesFilter; }
      set
      {
        if (_globalRulesFilter != value)
        {
          _globalRulesFilter = value;
          foreach (var ruleGroup in RuleGroups)
            ruleGroup.GlobalFilter = value;
        }
      }
    }

    public string LocalFilter
    {
      get { return _localRulesFilter; }
      set
      {
        if (_localRulesFilter != value)
        {
          _localRulesFilter = value;
          foreach (var ruleGroup in RuleGroups)
            ruleGroup.LocalFilter = value;
        }
      }
    }
  }
}