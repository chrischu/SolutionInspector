using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using SolutionInspector.ConfigurationUi.Infrastructure;
using SolutionInspector.ConfigurationUi.ViewModel;

namespace SolutionInspector.ConfigurationUi.Controls.CommaDelimitedStringCollection
{
  internal class CommaDelimitedStringCollectionViewModel : ViewModelBase
  {
    private readonly Stack<CommaDelimitedStringValueViewModel> _removeStack = new Stack<CommaDelimitedStringValueViewModel>();

    public CommaDelimitedStringCollectionViewModel (System.Configuration.CommaDelimitedStringCollection value)
    {
      Values =
          new AdvancedObservableCollection<CommaDelimitedStringValueViewModel>(
              value.Cast<string>().Select((s, i) => new CommaDelimitedStringValueViewModel(this, s, i)));
      Values.ElementAdded += s => value.Add(s.Value);
      Values.ElementRemoved += s => value.Remove(s.Value);
      Values.ElementChanged += e => value[e.Index] = e.Value;
    }

    public AdvancedObservableCollection<CommaDelimitedStringValueViewModel> Values { get; }
    public string LastRemovedValue => _removeStack.Count > 0 ? _removeStack.Peek().Value : null;

    public ICommand AddCommand => new RelayCommand(ExecuteAdd);
    public ICommand RemoveCommand => new RelayCommand<int>(ExecuteRemove);
    public ICommand UndoRemoveCommand => new RelayCommand(ExecuteUndo, () => _removeStack.Count > 0);

    private async void ExecuteAdd ()
    {
      var value = await ViewModelLocator.DialogManager.RequestInput("Adding new value", "Enter the new value");
      if (value != null)
        Values.Add(new CommaDelimitedStringValueViewModel(this, value, Values.Count));
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

  internal class CommaDelimitedStringValueViewModel : ViewModelBase
  {
    public CommaDelimitedStringValueViewModel (CommaDelimitedStringCollectionViewModel parent, string value, int index)
    {
      Parent = parent;
      Value = value;
      Index = index;
    }

    public CommaDelimitedStringCollectionViewModel Parent { get; }
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
    public override ViewModelBase CreateViewModel (object value)
    {
      return new CommaDelimitedStringCollectionViewModel((System.Configuration.CommaDelimitedStringCollection) value);
    }

    public override Type ValueType => typeof(System.Configuration.CommaDelimitedStringCollection);
  }
}