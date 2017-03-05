using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using SolutionInspector.Api.Configuration;
using SolutionInspector.Api.Configuration.Ruleset;
using SolutionInspector.ConfigurationUi.Features.Dialogs;
using SolutionInspector.ConfigurationUi.Features.Undo;
using SolutionInspector.ConfigurationUi.Infrastructure.AdvancedObservableCollections;

namespace SolutionInspector.ConfigurationUi.Features.Ruleset.ViewModels
{
  internal class ProjectRuleGroupViewModel : RuleGroupViewModel
  {
    public ProjectRuleGroupViewModel (
      IUndoManager undoManager,
      IDialogManager dialogManager,
      ProjectRuleGroupConfigurationElement ruleGroup,
      AdvancedObservableCollection<RuleViewModel> rules)
      : base(undoManager, dialogManager, rules)
    {
      RuleGroup = ruleGroup;
    }

    public ProjectRuleGroupConfigurationElement RuleGroup { get; }

    public string Name
    {
      get { return RuleGroup.Name; }
      set { RuleGroup.Name = value; }
    }

    public NameFilter AppliesTo => RuleGroup.AppliesTo;

    public ICommand EditCommand => new RelayCommand<SolutionViewModel>(ExecuteEdit);

    private async void ExecuteEdit (SolutionViewModel solution)
    {
      var result = await DialogManager.EditProjectRuleGroupFilter(solution, AppliesTo);
      if (!result.WasCancelled)
        RuleGroup.AppliesTo = result.Value;
    }

    protected override Task<DialogResult<RuleViewModel>> AddRule (IDialogManager dialogManager)
    {
      return dialogManager.AddProjectRule();
    }
  }
}