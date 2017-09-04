using System.Threading.Tasks;
using SolutionInspector.Api;
using SolutionInspector.Api.Configuration.Ruleset;
using SolutionInspector.ConfigurationUi.Features.Dialogs;
using SolutionInspector.ConfigurationUi.Features.Undo;
using SolutionInspector.ConfigurationUi.Infrastructure.AdvancedObservableCollections;

namespace SolutionInspector.ConfigurationUi.Features.Ruleset.ViewModels
{
  internal class ProjectItemRuleGroupViewModel : RuleGroupViewModel
  {
    public ProjectItemRuleGroupViewModel (
      IUndoContext undoContext,
      IDialogManager dialogManager,
      ProjectItemRuleGroupConfigurationElement ruleGroup,
      AdvancedObservableCollection<RuleViewModel> rules)
      : base(undoContext, dialogManager, rules)
    {
      RuleGroup = ruleGroup;
      AppliesTo = ruleGroup.AppliesTo;
      InProject = ruleGroup.InProject;
    }

    public ProjectItemRuleGroupConfigurationElement RuleGroup { get; }

    public string Name
    {
      get { return RuleGroup.Name; }
      set { RuleGroup.Name = value; }
    }

    public INameFilter AppliesTo { get; }
    public INameFilter InProject { get; }

    protected override Task<DialogResult<RuleViewModel>> AddRule (IDialogManager dialogManager)
    {
      return dialogManager.AddProjectItemRule();
    }
  }
}