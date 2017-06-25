using System;
using System.Collections.Generic;
using SolutionInspector.ConfigurationUi.Features.Undo;
using SolutionInspector.ConfigurationUi.Features.Undo.Actions.Collection;

namespace SolutionInspector.ConfigurationUi.Infrastructure.AdvancedObservableCollections
{
  internal class UndoAdvancedObservableCollectionChangeHandler<T> : AdvancedObservableCollectionChangeHandlerBase<T>
  {
    private readonly IUndoContext _undoContext;
    private AdvancedObservableCollection<T> _observableCollection;

    public UndoAdvancedObservableCollectionChangeHandler (IUndoContext undoContext)
    {
      _undoContext = undoContext;
    }

    public override void Initialize (AdvancedObservableCollection<T> observableCollection)
    {
      _observableCollection = observableCollection;
    }

    public override void ElementAdded (T element, int index)
    {
      _undoContext.Done(f => f.Collection(_observableCollection).Added(element, index));
    }

    public override void ElementRemoved (T element, int index)
    {
      _undoContext.Done(f => f.Collection(_observableCollection).Removed(index, element));
    }

    public override void ElementReplaced (T newElement, T oldElement, int index)
    {
      _undoContext.Done(f => f.Collection(_observableCollection).Replaced(index, oldElement));
    }

    public override void ElementsCleared (IReadOnlyCollection<T> clearedElements)
    {
      _undoContext.Done(f => f.Collection(_observableCollection).Cleared(clearedElements));
    }
  }
}