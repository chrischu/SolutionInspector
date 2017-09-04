using System.Threading.Tasks;
using SolutionInspector.ConfigurationUi.Features.Dialogs;
using SolutionInspector.ConfigurationUi.Features.Undo;
using SolutionInspector.ConfigurationUi.Infrastructure.AdvancedObservableCollections;

namespace SolutionInspector.ConfigurationUi.Features.Ruleset.ViewModels
{
  internal class SolutionRuleGroupViewModel : RuleGroupViewModel
  {
    public SolutionRuleGroupViewModel (IUndoContext undoContext, IDialogManager dialogManager, AdvancedObservableCollection<RuleViewModel> rules)
      : base(undoContext, dialogManager, rules)
    {
    }

    protected override Task<DialogResult<RuleViewModel>> AddRule (IDialogManager dialogManager)
    {
      return dialogManager.AddSolutionRule();
    }
  }
}