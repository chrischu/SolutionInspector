using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;

namespace SolutionInspector.ConfigurationUi.Features.Undo
{
  internal abstract class UndoContextBase : ObservableObject, IUndoContext
  {
    private readonly Stack<IUndoableAction> _undoStack = new Stack<IUndoableAction>();
    private readonly Stack<IUndoableAction> _redoStack = new Stack<IUndoableAction>();

    private ChildUndoContext _activeChildContext;
    private UndoActionCombiningSection _activeActionCombiningSection;

    public bool CanUndo => _activeChildContext?.CanUndo ?? _undoStack.Any();
    public bool CanRedo => _activeChildContext?.CanRedo ?? _redoStack.Any();

    public void Do (Func<IFutureUndoableActionFactory, IUndoableAction> createUndoableAction)
    {
      CheckNoChildUndoContext();

      var action = createUndoableAction(new UndoableActionFactory());
      action.Redo();

      Done(action);
    }

    public void Done (Func<IPastUndoableActionFactory, IUndoableAction> createUndoableAction)
    {
      CheckNoChildUndoContext();

      var action = createUndoableAction(new UndoableActionFactory());
      Done(action);
    }

    private void Done(IUndoableAction undoableAction)
    {
      CheckNoChildUndoContext();
      
      if (_activeActionCombiningSection != null)
      {
        _activeActionCombiningSection.Done(undoableAction);
        return;
      }

      _undoStack.Push(undoableAction);
      _redoStack.Clear();
      RaiseStackPropertiesChanged();
    }

    public void Undo()
    {
      if(_activeChildContext != null)
      {
        _activeChildContext.Undo();
        return;
      }

      CheckNoCombineActions();

      if (!CanUndo)
        throw new InvalidOperationException("Cannot undo, since there are no actions to be undone.");

      var action = _undoStack.Pop();
      action.Undo();
      _redoStack.Push(action);
      RaiseStackPropertiesChanged();
    }

    public void Redo()
    {
      if (_activeChildContext != null)
      {
        _activeChildContext.Redo();
        return;
      }

      CheckNoCombineActions();

      if (!CanRedo)
        throw new InvalidOperationException("Cannot redo, since there are no actions to be redone.");

      var action = _redoStack.Pop();
      action.Redo();
      _undoStack.Push(action);
      RaiseStackPropertiesChanged();
    }

    public void Reset()
    {
      CheckNoCombineActions();

      _undoStack.Clear();
      _redoStack.Clear();
      _activeChildContext?.Dispose();
      RaiseStackPropertiesChanged();
    }

    public IDisposable CombineActions()
    {
      CheckNoChildUndoContext();

      if (_activeActionCombiningSection != null)
        throw new InvalidOperationException("Combining is already active.");

      return new UndoActionCombiningSection(this);
    }

    public IChildUndoContext OpenChildContext()
    {
      CheckNoChildUndoContext();
      
      return _activeChildContext = new ChildUndoContext(this);
    }

    private void CheckNoChildUndoContext()
    {
      if (_activeChildContext != null)
        throw new InvalidOperationException("While a child undo context is active, it is not allowed to use the parent undo context.");
    }

    private void CheckNoCombineActions()
    {
      if (_activeActionCombiningSection != null)
        throw new InvalidOperationException("While combining actions is active, it is not allowed to undo/redo/reset.");
    }

    protected virtual void RaiseStackPropertiesChanged()
    {
      RaisePropertyChanged(() => CanUndo);
      RaisePropertyChanged(() => CanRedo);
    }

    private class ChildUndoContext : UndoContextBase, IChildUndoContext
    {
      private readonly UndoContextBase _parentContext;

      public ChildUndoContext(UndoContextBase parentContext)
      {
        _parentContext = parentContext;
      }

      public void Dispose()
      {
        _activeChildContext?.Dispose();
        _parentContext._activeChildContext = null;
      }

      protected override void RaiseStackPropertiesChanged ()
      {
        base.RaiseStackPropertiesChanged();
        _parentContext.RaiseStackPropertiesChanged();
      }
    }

    private class UndoableActionFactory : IPastUndoableActionFactory, IFutureUndoableActionFactory
    {
      IPastUndoableCollectionActionFactory<T> IPastUndoableActionFactory.Collection<T>(IList<T> collection)
      {
        return new UndoableCollectionActionFactory<T>(collection);
      }

      IFutureUndoableCollectionActionFactory<T> IFutureUndoableActionFactory.Collection<T>(IList<T> collection)
      {
        return new UndoableCollectionActionFactory<T>(collection);
      }

      IPastUndoableObjectActionFactory<T> IPastUndoableActionFactory.Object<T>(T @object)
      {
        return new UndoableObjectActionFactory<T>(@object);
      }

      IFutureUndoableObjectActionFactory<T> IFutureUndoableActionFactory.Object<T>(T @object)
      {
        return new UndoableObjectActionFactory<T>(@object);
      }

      private class UndoableCollectionActionFactory<T> : IPastUndoableCollectionActionFactory<T>, IFutureUndoableCollectionActionFactory<T>
      {
        public UndoableCollectionActionFactory(IList<T> collection)
        {
          Collection = collection;
        }

        public IList<T> Collection { get; }
      }

      private class UndoableObjectActionFactory<T> : IPastUndoableObjectActionFactory<T>, IFutureUndoableObjectActionFactory<T>
      {
        public UndoableObjectActionFactory(T @object)
        {
          Object = @object;
        }

        public T Object { get; }
      }
    }

    private class UndoActionCombiningSection : IDisposable
    {
      private readonly UndoContextBase _undoContext;
      private readonly List<IUndoableAction> _capturedActions = new List<IUndoableAction>();

      public UndoActionCombiningSection(UndoContextBase undoContext)
      {
        _undoContext = undoContext;
        _undoContext._activeActionCombiningSection = this;
      }

      public void Done(IUndoableAction undoable)
      {
        _capturedActions.Add(undoable);
      }

      public void Dispose()
      {
        _undoContext._activeActionCombiningSection = null;
        _undoContext.Done(new CompoundUndoableAction(_capturedActions));
      }
    }
  }
}