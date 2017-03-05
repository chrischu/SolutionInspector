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
using SolutionInspector.Internals;

namespace SolutionInspector.ConfigurationUi.Features.Dialogs.EditGroupFilter.EditProjectRuleGroupFilter
{
  internal class ProjectRuleGroupFilterViewModel : ViewModelBase
  {
    private readonly IUndoManager _undoManager;
    private bool _isCheckedUpdateSuspended;

    public ProjectRuleGroupFilterViewModel (SolutionViewModel solutionViewModel, INameFilter appliesTo, IUndoManager undoManager)
    {
      _undoManager = undoManager;

      Projects = new ObservableCollection<ProjectTreeViewModel>(
        solutionViewModel.Projects
            .Select(p => new ProjectTreeViewModel(p.Name, p.ProjectFileExtension, appliesTo.IsMatch(p.Name), Enumerable.Empty<TreeViewModelBase>()))
            .OrderBy(p => p.Name));

      foreach (var project in Projects)
        project.IsCheckedChanged += x => ProjectOnIsCheckedChanged();

      AppliesTo = new NameFilterEditViewModel(appliesTo, _undoManager);
    }

    private async void ProjectOnIsCheckedChanged()
    {
      if (_isCheckedUpdateSuspended)
        return;

      var task = Task.Run(() => NameFilterGenerator.Generate(Projects.Select(p => Tuple.Create(p.Name, p.IsChecked)).ToList()));
      AppliesTo = new NameFilterEditViewModel(await task, _undoManager);
    }

    public ICommand CheckAllCommand => new RelayCommand(() => ChangeCheckState(x => true));
    public ICommand UncheckAllCommand => new RelayCommand(() => ChangeCheckState(x => false));
    public ICommand InvertSelectionCommand => new RelayCommand(() => ChangeCheckState(x => !x));

    private void ChangeCheckState(Func<bool, bool> convertPreviousStateToNewState)
    {
      _isCheckedUpdateSuspended = true;
      Projects.ForEach(p => p.IsChecked = convertPreviousStateToNewState(p.IsChecked));
      _isCheckedUpdateSuspended = false;
      ProjectOnIsCheckedChanged();
    }

    public bool IsInManualEditMode { get; set; }
    public ObservableCollection<ProjectTreeViewModel> Projects { get; }
    public NameFilterEditViewModel AppliesTo { get; set; }

    public ICommand SwitchEditModeCommand => new RelayCommand(() => IsInManualEditMode = !IsInManualEditMode);
  }
}