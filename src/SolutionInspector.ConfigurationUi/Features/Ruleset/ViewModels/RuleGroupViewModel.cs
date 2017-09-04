using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using JetBrains.Annotations;
using SolutionInspector.ConfigurationUi.Features.Dialogs;
using SolutionInspector.ConfigurationUi.Features.Undo;
using SolutionInspector.ConfigurationUi.Features.Undo.Actions.Collection;
using SolutionInspector.ConfigurationUi.Infrastructure.AdvancedObservableCollections;

namespace SolutionInspector.ConfigurationUi.Features.Ruleset.ViewModels
{
  internal abstract class RuleGroupViewModel : ViewModelBase
  {
    private readonly IUndoContext _undoContext;
    private string _globalFilter;
    private string _localFilter;

    protected RuleGroupViewModel (
      IUndoContext undoContext,
      IDialogManager dialogManager,
      AdvancedObservableCollection<RuleViewModel> rules)
    {
      _undoContext = undoContext;
      DialogManager = dialogManager;
      Rules = rules;
    }

    protected IDialogManager DialogManager { get; }

    public AdvancedObservableCollection<RuleViewModel> Rules { get; }

    public string GlobalFilter
    {
      get { return _globalFilter; }
      set
      {
        if (_globalFilter != value)
        {
          _globalFilter = value;
          foreach (var ruleViewModel in Rules)
            ruleViewModel.GlobalFilter = value;
          RaisePropertyChanged(() => IsFiltered);
        }
      }
    }

    public string LocalFilter
    {
      get { return _localFilter; }
      set
      {
        if (_localFilter != value)
        {
          _localFilter = value;
          foreach (var ruleViewModel in Rules)
            ruleViewModel.GroupFilter = value;
          RaisePropertyChanged(() => IsFiltered);
        }
      }
    }

    public bool IsFiltered => Rules.All(r => r.IsFiltered);

    public ICommand AddRuleCommand => new RelayCommand(ExecuteAddRule);

    [UsedImplicitly /* by binding */]
    public ICommand RemoveRuleCommand => new RelayCommand<int>(ExecuteRemoveRule);

    private async void ExecuteAddRule ()
    {
      var dialogResult = await AddRule(DialogManager);

      if (dialogResult.WasCancelled)
        return;

      var ruleViewModel = dialogResult.Value;
      _undoContext.Do(f => f.Collection(Rules).Add(ruleViewModel));
    }

    protected abstract Task<DialogResult<RuleViewModel>> AddRule (IDialogManager dialogManager);

    private void ExecuteRemoveRule (int index)
    {
      _undoContext.Do(f => f.Collection(Rules).Remove(index));
    }
  }
}