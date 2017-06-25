using System;
using System.Collections.Generic;

namespace SolutionInspector.ConfigurationUi.Features.Undo.Actions.Collection
{
  internal static class RemoveExtensions
  {
    public static IUndoableAction Remove<T> (this IFutureUndoableCollectionActionFactory<T> factory, int index)
    {
      return new UndoableCollectionRemoveAction<T>(factory.Collection, index, factory.Collection[index]);
    }

    public static IUndoableAction Removed<T> (this IPastUndoableCollectionActionFactory<T> factory, int index, T removedElement)
    {
      return new UndoableCollectionRemoveAction<T>(factory.Collection, index, removedElement);
    }

    private class UndoableCollectionRemoveAction<T> : IUndoableAction
    {
      private readonly IList<T> _collection;
      private readonly int _index;
      private readonly T _removedElement;

      public UndoableCollectionRemoveAction (IList<T> collection, int index, T removedElement)
      {
        _collection = collection;
        _index = index;
        _removedElement = removedElement;
      }

      public void Undo ()
      {
        _collection.Insert(_index, _removedElement);
      }

      public void Redo ()
      {
        _collection.RemoveAt(_index);
      }
    }
  }
}