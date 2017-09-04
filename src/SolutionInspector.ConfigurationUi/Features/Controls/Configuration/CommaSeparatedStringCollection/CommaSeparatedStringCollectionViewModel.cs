using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using SolutionInspector.ConfigurationUi.Features.Dialogs;
using SolutionInspector.ConfigurationUi.Features.Undo;
using SolutionInspector.ConfigurationUi.Features.Undo.Actions.Collection;
using SolutionInspector.ConfigurationUi.Infrastructure;
using SolutionInspector.ConfigurationUi.Infrastructure.AdvancedObservableCollections;
using SolutionInspector.ConfigurationUi.Infrastructure.ValidationRules;

namespace SolutionInspector.ConfigurationUi.Features.Controls.Configuration.CommaSeparatedStringCollection
{
  internal class CommaSeparatedStringCollectionViewModel : ViewModelBase
  {
    private readonly IDialogManager _dialogManager;
    private readonly IUndoContext _undoContext;

    public CommaSeparatedStringCollectionViewModel (
      SolutionInspector.Configuration.CommaSeparatedStringCollection value,
      IDialogManager dialogManager,
      IUndoContext undoContext)
    {
      _dialogManager = dialogManager;
      _undoContext = undoContext;

      Values = MirroringObservableCollection.Create(value);
    }

    public AdvancedObservableCollection<string> Values { get; }
    public int SelectedIndex { get; set; }

    public ICommand AddCommand => new RelayCommand(ExecuteAdd);
    public ICommand EditCommand => new RelayCommand<int>(ExecuteEdit, CanEditOrRemove);
    public ICommand RemoveCommand => new RelayCommand<int>(ExecuteRemove, CanEditOrRemove);

    private async void ExecuteAdd()
    {
      var result = await ShowAddNewItemDialog();

      if (!result.WasCancelled)
        _undoContext.Do(f => f.Collection(Values).Add(result.Value));
    }

    private async void ExecuteEdit (int index)
    {
      var result = await ShowEditItemDialog(Values[index]);

      if (!result.WasCancelled)
        _undoContext.Do(f => f.Collection(Values).Replace(index, result.Value));
    }

    private void ExecuteRemove (int index)
    {
      _undoContext.Do(f => f.Collection(Values).Remove(index));

      if (Values.Count > 0)
        SelectedIndex = Math.Max(Values.Count - 1, Math.Min(0, index));
    }

    private bool CanEditOrRemove (int index)
    {
      return index >= 0 && index < Values.Count;
    }

    private Task<DialogResult<string>> ShowAddNewItemDialog ()
    {
      return _dialogManager.RequestInput(
        "Add a new item:",
        validationRules: new ValidationRule[]
                         {
                           new NonEmptyValidationRule(),
                           new NoDuplicatesValidationRule<string>(Values)
                         });
    }

    private Task<DialogResult<string>> ShowEditItemDialog (string value)
    {
      return _dialogManager.RequestInput(
        $"Edit value '{value}':",
        initialValue: value,
        validationRules: new ValidationRule[]
                         {
                           new NonEmptyValidationRule(),
                           new NoDuplicatesValidationRule<string>(Values.Except(new[] { value }))
                         });
    }
  }
}