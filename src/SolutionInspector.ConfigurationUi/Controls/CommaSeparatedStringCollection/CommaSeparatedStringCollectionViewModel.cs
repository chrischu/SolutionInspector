using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using SolutionInspector.Configuration;
using SolutionInspector.ConfigurationUi.Infrastructure;
using SolutionInspector.ConfigurationUi.ViewModel;

namespace SolutionInspector.ConfigurationUi.Controls.CommaSeparatedStringCollection
{
  internal class CommaSeparatedStringCollectionViewModel : ViewModelBase
  {
    private readonly Stack<CommaSeparatedStringValueViewModel> _removeStack = new Stack<CommaSeparatedStringValueViewModel>();

    public CommaSeparatedStringCollectionViewModel (SolutionInspector.Configuration.CommaSeparatedStringCollection value)
    {
      Values =
          new AdvancedObservableCollection<CommaSeparatedStringValueViewModel>(
            value.Select((s, i) => new CommaSeparatedStringValueViewModel(this, s, i)));
      Values.ElementAdded += s => value.Add(s.Value);
      Values.ElementRemoved += s => value.Remove(s.Value);
      Values.ElementChanged += e => value[e.Index] = e.Value;
    }

    public AdvancedObservableCollection<CommaSeparatedStringValueViewModel> Values { get; }
    public string LastRemovedValue => _removeStack.Count > 0 ? _removeStack.Peek().Value : null;

    public ICommand AddCommand => new RelayCommand(ExecuteAdd);
    public ICommand RemoveCommand => new RelayCommand<int>(ExecuteRemove);
    public ICommand UndoRemoveCommand => new RelayCommand(ExecuteUndo, () => _removeStack.Count > 0);

    private async void ExecuteAdd ()
    {
      var value = await ViewModelLocator.DialogManager.RequestInput("Adding new value", "Enter the new value");
      if (value != null)
        Values.Add(new CommaSeparatedStringValueViewModel(this, value, Values.Count));
    }

    private void ExecuteRemove (int index)
    {
      _removeStack.Push(Values[index]);
      Values.RemoveAt(index);
      UpdateIndicesAndLastRemovedValue();
    }

    private void ExecuteUndo ()
    {
      var removed = _removeStack.Pop();
      Values.Insert(removed.Index, removed);
      UpdateIndicesAndLastRemovedValue();
    }

    private void UpdateIndicesAndLastRemovedValue ()
    {
      for (var i = 0; i < Values.Count; i++)
        Values[i].Index = i;
      RaisePropertyChanged(() => LastRemovedValue);
    }
  }

  internal class CommaSeparatedStringValueViewModel : ViewModelBase
  {
    public CommaSeparatedStringValueViewModel (CommaSeparatedStringCollectionViewModel parent, string value, int index)
    {
      Parent = parent;
      Value = value;
      Index = index;
    }

    public CommaSeparatedStringCollectionViewModel Parent { get; }
    public string Value { get; set; }
    public int Index { get; set; }

    public ICommand EditCommand => new RelayCommand(ExecuteEdit);

    private async void ExecuteEdit ()
    {
      Value = await ViewModelLocator.DialogManager.RequestInput($"Editing value '{Value}'", "Enter the new value", Value);
    }
  }

  public class CommaDelimitedStringCollectionControl : ConfigurationControlBase
  {
    public override Type ValueType => typeof(SolutionInspector.Configuration.CommaSeparatedStringCollection);

    public override ViewModelBase CreateViewModel (object value)
    {
      return new CommaSeparatedStringCollectionViewModel((SolutionInspector.Configuration.CommaSeparatedStringCollection) value);
    }
  }
}