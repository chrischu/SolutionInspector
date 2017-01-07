using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace SolutionInspector.ConfigurationUi.Infrastructure
{
  internal class AdvancedObservableCollection<T> : ObservableCollection<T>
    where T : INotifyPropertyChanged
  {
    public AdvancedObservableCollection ()
    {
    }

    public AdvancedObservableCollection (IEnumerable<T> elements)
      : base(elements)
    {
      foreach (var element in elements)
        element.PropertyChanged += ElementOnPropertyChanged;
    }

    public event Action<T> ElementAdded;
    public event Action<T> ElementRemoved;
    public event Action<T> ElementChanged;

    private void ElementOnPropertyChanged (object sender, PropertyChangedEventArgs propertyChangedEventArgs)
    {
      ElementChanged?.Invoke((T) sender);
    }

    protected override void OnCollectionChanged (NotifyCollectionChangedEventArgs e)
    {
      base.OnCollectionChanged(e);

      if (e.Action == NotifyCollectionChangedAction.Add)
        foreach (T item in e.NewItems)
        {
          ElementAdded?.Invoke(item);
          item.PropertyChanged += ElementOnPropertyChanged;
        }

      if (e.Action == NotifyCollectionChangedAction.Remove)
        foreach (T item in e.OldItems)
        {
          ElementRemoved?.Invoke(item);
          item.PropertyChanged -= ElementOnPropertyChanged;
        }
    }
  }
}