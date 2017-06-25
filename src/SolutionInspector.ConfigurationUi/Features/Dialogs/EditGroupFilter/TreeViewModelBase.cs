using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;

namespace SolutionInspector.ConfigurationUi.Features.Dialogs.EditGroupFilter
{
  internal class TreeViewModelBase : ViewModelBase
  {
    public event Action<TreeViewModelBase, bool> IsCheckedChanged;

    private bool _isChecked;

    public TreeViewModelBase (string name, string extension, TreeViewModelType type, bool isChecked, IEnumerable<TreeViewModelBase> children)
    {
      Name = name;
      Extension = extension;
      Type = type;
      IsChecked = isChecked;
      Children = new ObservableCollection<TreeViewModelBase>(children);
    }

    public string Name { get; }
    public string Extension { get; }
    public TreeViewModelType Type { get; }

    public bool IsChecked
    {
      get { return _isChecked; }
      set
      {
        if (_isChecked != value)
        {
          _isChecked = value;
          IsCheckedChanged?.Invoke(this, _isChecked);
        }
      }
    }

    public bool IsSelected { get; set; }

    public bool IsExpanded { get; set; }
    public ObservableCollection<TreeViewModelBase> Children { get; }
  }
}