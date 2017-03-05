using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using SolutionInspector.Commons.Extensions;

namespace SolutionInspector.ConfigurationUi.Infrastructure.AdvancedObservableCollections
{
  internal class AdvancedObservableCollection<T> : ObservableCollection<T>
  {
    private readonly IReadOnlyCollection<IAdvancedObservableCollectionChangeHandler<T>> _changeHandlers;

    [PublicAPI]
    public AdvancedObservableCollection(params IAdvancedObservableCollectionChangeHandler<T>[] changeHandlers)
      : this(Enumerable.Empty<T>(), changeHandlers)
    {
    }

    public AdvancedObservableCollection(IEnumerable<T> elements, params IAdvancedObservableCollectionChangeHandler<T>[] changeHandlers)
      : base(elements)
    {
      _changeHandlers = changeHandlers;
    }

    protected override void ClearItems ()
    {
      _changeHandlers.ForEach(h => h.ElementsCleared(this.ToList()));
      base.ClearItems();
    }

    protected sealed override void OnCollectionChanged (NotifyCollectionChangedEventArgs e)
    {
      base.OnCollectionChanged(e);

      switch (e.Action)
      {
        case NotifyCollectionChangedAction.Add:
          HandleAdd(e);
          break;
        case NotifyCollectionChangedAction.Remove:
          HandleRemove(e);
          break;
        case NotifyCollectionChangedAction.Replace:
          HandleReplace(e);
          break;
        case NotifyCollectionChangedAction.Move:
          HandleMove(e);
          break;
        case NotifyCollectionChangedAction.Reset: // already handled by the ClearItems override
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    private void HandleAdd (NotifyCollectionChangedEventArgs e)
    {
      for (var i = 0; i < e.NewItems.Count; i++)
        OnElementAdded((T) e.NewItems[i], e.NewStartingIndex + i);
    }

    protected void OnElementAdded (T element, int index)
    {
      _changeHandlers.ForEach(h => h.ElementAdded(element, index));
    }

    private void HandleRemove (NotifyCollectionChangedEventArgs e)
    {
      for (var i = 0; i < e.OldItems.Count; i++)
        OnElementRemoved((T) e.OldItems[i], e.OldStartingIndex + i);
    }

    protected void OnElementRemoved (T element, int index)
    {
      _changeHandlers.ForEach(h => h.ElementRemoved(element, index));
    }

    private void HandleReplace (NotifyCollectionChangedEventArgs e)
    {
      for (var i = 0; i < e.NewItems.Count; i++)
        OnElementReplaced((T) e.NewItems[i], (T) e.OldItems[i], e.NewStartingIndex + i);
    }

    protected void OnElementReplaced (T newElement, T oldElement, int index)
    {
      _changeHandlers.ForEach(h => h.ElementReplaced(newElement, oldElement, index));
    }

    private void HandleMove (NotifyCollectionChangedEventArgs e)
    {
      Trace.Assert(e.NewItems.Count == e.OldItems.Count, "Both item collections must have the same number of elements");

      for (var i = 0; i < e.NewItems.Count; i++)
      {
        OnElementReplaced(Items[e.NewStartingIndex + i], Items[e.OldStartingIndex + i], e.NewStartingIndex + i);
        OnElementReplaced(Items[e.OldStartingIndex + i], Items[e.NewStartingIndex + i], e.OldStartingIndex + i);
      }
    }
  }

  internal static class AdvancedObservableCollection
  {

  }
}