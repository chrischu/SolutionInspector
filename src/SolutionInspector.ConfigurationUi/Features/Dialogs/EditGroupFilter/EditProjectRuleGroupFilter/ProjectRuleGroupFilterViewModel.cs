using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using SolutionInspector.Api;
using SolutionInspector.Commons.Extensions;
using SolutionInspector.ConfigurationUi.Features.Ruleset.ViewModels;
using SolutionInspector.ConfigurationUi.Features.Undo;
using SolutionInspector.ConfigurationUi.Features.Undo.Actions.Object;
using SolutionInspector.ConfigurationUi.Infrastructure;
using SolutionInspector.Internals;

namespace SolutionInspector.ConfigurationUi.Features.Dialogs.EditGroupFilter.EditProjectRuleGroupFilter
{
  internal class ProjectRuleGroupFilterViewModel : ViewModelBase
  {
    private readonly IUndoContext _undoContext;
    private bool _isCheckedUpdateSuspended;

    public ProjectRuleGroupFilterViewModel (SolutionViewModel solutionViewModel, INameFilter appliesTo, IUndoContext undoContext)
    {
      _undoContext = undoContext;

      Projects = new ObservableCollection<ProjectTreeViewModel>(
        solutionViewModel.Projects
            .Select(p => new ProjectTreeViewModel(p.Name, p.ProjectFileExtension, appliesTo.IsMatch(p.Name), Enumerable.Empty<TreeViewModelBase>()))
            .OrderBy(p => p.Name));

      foreach (var project in Projects)
        project.IsCheckedChanged += (proj, isChecked) => ProjectOnIsCheckedChanged(proj);

      AppliesTo = new NameFilterEditViewModel(appliesTo, _undoContext);
    }

    private void ProjectOnIsCheckedChanged (TreeViewModelBase proj)
    {
      _undoContext.Done(f => f.Object(proj).PropertyChanged(p => p.IsChecked, !proj.IsChecked));

      if (_isCheckedUpdateSuspended)
        return;

      UpdateAppliesTo();
    }

    private async void UpdateAppliesTo ()
    {
      var task = Task.Run(() => NameFilterGenerator.Generate(Projects.Select(p => Tuple.Create(p.Name, p.IsChecked)).ToList()));
      AppliesTo = new NameFilterEditViewModel(await task, _undoContext);
    }

    [UsedByView]
    public ICommand CheckAllCommand => new RelayCommand(() => ChangeCheckState(x => true));

    [UsedByView]
    public ICommand UncheckAllCommand => new RelayCommand(() => ChangeCheckState(x => false));

    [UsedByView]
    public ICommand InvertSelectionCommand => new RelayCommand(() => ChangeCheckState(x => !x));

    private void ChangeCheckState (Func<bool, bool> convertPreviousStateToNewState)
    {
      _isCheckedUpdateSuspended = true;
      using (_undoContext.CombineActions())
        Projects.ForEach(p => p.IsChecked = convertPreviousStateToNewState(p.IsChecked));
      _isCheckedUpdateSuspended = false;

      UpdateAppliesTo();
    }

    public bool IsInManualEditMode { get; set; }
    public ObservableCollection<ProjectTreeViewModel> Projects { get; }
    public NameFilterEditViewModel AppliesTo { get; set; }

    public ICommand SwitchEditModeCommand => new RelayCommand(() => IsInManualEditMode = !IsInManualEditMode);
  }
}