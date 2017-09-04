namespace SolutionInspector.ConfigurationUi.Features.Dialogs.EditGroupFilter.EditProjectRuleGroupFilter
{
  internal class EditProjectRuleGroupFilterDialogViewModel : DialogViewModelBase
  {
    public ProjectRuleGroupFilterViewModel ProjectRuleGroupFilter { get; }

    public EditProjectRuleGroupFilterDialogViewModel (string title, ProjectRuleGroupFilterViewModel projectRuleGroupFilter)
      : base(title)
    {
      ProjectRuleGroupFilter = projectRuleGroupFilter;
    }
  }
}