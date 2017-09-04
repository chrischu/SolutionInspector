using System;
using System.Collections.Generic;

namespace SolutionInspector.ConfigurationUi.Features.Undo.Actions.Collection
{
  internal static class AddExtensions
  {
    public static IUndoableAction Add<T> (this IFutureUndoableCollectionActionFactory<T> factory, T element, int? index = null)
    {
      return new UndoableCollectionAddAction<T>(factory.Collection, element, index ?? factory.Collection.Count);
    }

    public static IUndoableAction Added<T>(this IPastUndoableCollectionActionFactory<T> factory, T element, int index)
    {
      return new UndoableCollectionAddAction<T>(factory.Collection, element, index);
    }

    private class UndoableCollectionAddAction<T> : IUndoableAction
    {
      private readonly IList<T> _collection;
      private readonly T _element;
      private readonly int _index;

      public UndoableCollectionAddAction (IList<T> collection, T element, int index)
      {
        _collection = collection;
        _element = element;
        _index = index;
      }

      public void Undo ()
      {
        _collection.RemoveAt(_index);
      }

      public void Redo ()
      {
        _collection.Insert(_index, _element);
      }
    }
  }
}