using System;
using System.Collections.Generic;
using System.Linq;
using SolutionInspector.Commons.Extensions;

namespace SolutionInspector.ConfigurationUi.Features.Undo.Actions.Collection
{
  internal static class ClearExtensions
  {
    public static IUndoableAction Clear<T>(this IFutureUndoableCollectionActionFactory<T> factory)
    {
      return new UndoableCollectionClearAction<T>(factory.Collection, factory.Collection.ToList());
    }

    public static IUndoableAction Cleared<T>(this IPastUndoableCollectionActionFactory<T> factory, IReadOnlyCollection<T> clearedElements)
    {
      return new UndoableCollectionClearAction<T>(factory.Collection, clearedElements);
    }

    private class UndoableCollectionClearAction<T> : IUndoableAction
    {
      private readonly IList<T> _collection;
      private readonly IReadOnlyCollection<T> _clearedElements;

      public UndoableCollectionClearAction (IList<T> collection, IReadOnlyCollection<T> clearedElements)
      {
        _collection = collection;
        _clearedElements = clearedElements;
      }

      public void Redo ()
      {
        _collection.Clear();
      }

      public void Undo ()
      {
        _collection.AddRange(_clearedElements);
      }
    }
  }
}