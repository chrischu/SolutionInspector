using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using JetBrains.Annotations;
using SolutionInspector.ConfigurationUi.Features.Undo.Actions;
using SolutionInspector.ConfigurationUi.Features.Undo.Actions.Collection.Add;
using SolutionInspector.ConfigurationUi.Features.Undo.Actions.Collection.Clear;
using SolutionInspector.ConfigurationUi.Features.Undo.Actions.Collection.Remove;
using SolutionInspector.ConfigurationUi.Features.Undo.Actions.Collection.Replace;
using SolutionInspector.ConfigurationUi.Infrastructure.AdvancedObservableCollections;

namespace SolutionInspector.ConfigurationUi.Features.Undo
{
  internal class UndoAdvancedObservableCollectionChangeHandler<T> : AdvancedObservableCollectionChangeHandlerBase<T>
  {
    private readonly IUndoManager _undoManager;
    private AdvancedObservableCollection<T> _observableCollection;

    public UndoAdvancedObservableCollectionChangeHandler (IUndoManager undoManager)
    {
      _undoManager = undoManager;
    }

    public override void Initialize (AdvancedObservableCollection<T> observableCollection)
    {
      _observableCollection = observableCollection;
    }

    public override void ElementAdded (T element, int index)
    {
      _undoManager.Done(new ItemAddedAction<T>(_observableCollection, element, index));
    }

    public override void ElementRemoved (T element, int index)
    {
      _undoManager.Done(new ItemRemovedAction<T>(_observableCollection, element, index));
    }

    public override void ElementReplaced (T newElement, T oldElement, int index)
    {
      _undoManager.Done(new ItemReplacedAction<T>(_observableCollection, newElement, index, oldElement));
    }

    public override void ElementsCleared (IReadOnlyCollection<T> removedElements)
    {
      _undoManager.Done(new ItemsClearedAction<T>(_observableCollection, removedElements));
    }
  }

  internal interface IUndoManager
  {
    bool CanUndo { get; }
    bool CanRedo { get; }

    void Do(IDoableAction doableAction);
    void Done (IUndoableAction undoableAction);
    void Undo();
    void Redo();
    void Reset ();
  }

  [UsedImplicitly /* by Autofac */]
  internal class UndoManager : ObservableObject, IUndoManager
  {
    private readonly Stack<IUndoableAction> _undoStack = new Stack<IUndoableAction>();
    private readonly Stack<IDoableAction> _redoStack = new Stack<IDoableAction>();

    public bool CanUndo => _undoStack.Any();
    public bool CanRedo => _redoStack.Any();

    public void Do(IDoableAction doableAction)
    {
      var undoable = doableAction.Do();
      Done(undoable);
    }

    public void Done (IUndoableAction undoableAction)
    {
      _undoStack.Push(undoableAction);
      _redoStack.Clear();
      RaiseStackPropertiesChanged();
    }

    public void Undo()
    {
      if (!CanUndo)
        throw new InvalidOperationException("Cannot undo, since there are no actions to be undone.");

      var undoable = _undoStack.Pop();
      var doable = undoable.Undo();
      _redoStack.Push(doable);
      RaiseStackPropertiesChanged();
    }

    public void Redo()
    {
      if (!CanRedo)
        throw new InvalidOperationException("Cannot redo, since there are no actions to be redone.");

      var doable = _redoStack.Pop();
      Do(doable);
    }

    public void Reset ()
    {
      _undoStack.Clear();
      _redoStack.Clear();
      RaiseStackPropertiesChanged();
    }

    private void RaiseStackPropertiesChanged ()
    {
      RaisePropertyChanged(() => CanUndo);
      RaisePropertyChanged(() => CanRedo);
    }
  }
}
