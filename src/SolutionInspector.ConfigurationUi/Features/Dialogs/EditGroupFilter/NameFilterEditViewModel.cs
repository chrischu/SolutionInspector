using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using SolutionInspector.Api;
using SolutionInspector.Api.Configuration;
using SolutionInspector.ConfigurationUi.Features.Undo;
using SolutionInspector.ConfigurationUi.Features.Undo.Actions.Collection;

namespace SolutionInspector.ConfigurationUi.Features.Dialogs.EditGroupFilter
{
  internal class NameFilterEditViewModel : ViewModelBase
  {
    public NameFilterEditViewModel (INameFilter nameFilter, IUndoContext undoContext)
    {
      Includes = new FilterCollection(undoContext, nameFilter.Includes);
      Excludes = new FilterCollection(undoContext, nameFilter.Excludes);
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
    private readonly IUndoContext _undoContext;

    public FilterCollection (IUndoContext undoContext, IEnumerable<string> collection) : base(collection)
    {
      _undoContext = undoContext;
    }

    public ICommand AddCommand => new RelayCommand(() => _undoContext.Do(f => f.Collection(this).Add("")));
    public ICommand RemoveCommand => new RelayCommand<int>(i => _undoContext.Do(f => f.Collection(this).Remove(i)));

    public ICommand EditCommand => new RelayCommand<Tuple<string, object>>(
      t =>
      {
        var index = (int)t.Item2;
        _undoContext.Do(f => f.Collection(this).Replace(index, t.Item1));
      });
  }
}