using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using SolutionInspector.Api;
using SolutionInspector.Api.Configuration;
using SolutionInspector.ConfigurationUi.Features.Undo;
using SolutionInspector.ConfigurationUi.Features.Undo.Actions.Collection.Add;
using SolutionInspector.ConfigurationUi.Features.Undo.Actions.Collection.Remove;
using SolutionInspector.ConfigurationUi.Features.Undo.Actions.Collection.Replace;

namespace SolutionInspector.ConfigurationUi.Features.Dialogs.EditGroupFilter
{
  internal class NameFilterEditViewModel : ViewModelBase
  {
    public NameFilterEditViewModel (INameFilter nameFilter, IUndoManager undoManager)
    {
      Includes = new FilterCollection(undoManager, nameFilter.Includes);
      Excludes = new FilterCollection(undoManager, nameFilter.Excludes);
    }

    public FilterCollection Includes { get; }
    public FilterCollection Excludes { get; }

    public NameFilter GetNameFilter ()
    {
      return new NameFilter(Includes, Excludes);
    }
  }

  internal class FilterCollection : ObservableCollection<string>
  {
    private readonly IUndoManager _undoManager;

    public FilterCollection (IUndoManager undoManager, IEnumerable<string> collection) : base(collection)
    {
      _undoManager = undoManager;
    }

    public ICommand AddCommand => new RelayCommand(() => _undoManager.Do(new AddItemAction<string>(this, "")));
    public ICommand RemoveCommand => new RelayCommand<int>(i => _undoManager.Do(new RemoveItemAction<string>(this, i)));

    public ICommand EditCommand => new RelayCommand<Tuple<string, object>>(
      t =>
      {
        var index = (int)t.Item2;
        _undoManager.Do(new ReplaceItemAction<string>(this, t.Item1, index));
      });
  }
}