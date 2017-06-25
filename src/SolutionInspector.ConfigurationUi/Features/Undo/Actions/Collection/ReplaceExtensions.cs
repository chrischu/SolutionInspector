using System;
using System.Collections.Generic;

namespace SolutionInspector.ConfigurationUi.Features.Undo.Actions.Collection
{
  internal static class ReplaceExtensions
  {
    public static IUndoableAction Replace<T>(this IFutureUndoableCollectionActionFactory<T> factory, int index, T newElement)
    {
      return new UndoableCollectionReplaceAction<T>(factory.Collection, index, newElement, factory.Collection[index]);
    }

    public static IUndoableAction Replaced<T>(this IPastUndoableCollectionActionFactory<T> factory, int index, T oldElement)
    {
      return new UndoableCollectionReplaceAction<T>(factory.Collection, index, factory.Collection[index], oldElement);
    }

    private class UndoableCollectionReplaceAction<T> : IUndoableAction
    {
      private readonly IList<T> _collection;
      private readonly int _index;
      private readonly T _newElement;
      private readonly T _oldElement;

      public UndoableCollectionReplaceAction(IList<T> collection, int index, T newElement, T oldElement)
      {
        _collection = collection;
        _index = index;
        _newElement = newElement;
        _oldElement = oldElement;
      }

      public void Undo()
      {
        _collection[_index] = _oldElement;
      }

      public void Redo()
      {
        _collection[_index] = _newElement;
      }
    }
  }
}