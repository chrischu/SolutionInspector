using System.Collections.Generic;

namespace SolutionInspector.ConfigurationUi.Features.Undo.Actions.Collection.Add
{
  internal class AddItemAction<T> : IndexedCollectionActionBase<T>, IDoableAction
  {
    public AddItemAction (IList<T> collection, T item, int? index = null) : base(collection, item, index ?? collection.Count - 1)
    {
    }

    public IUndoableAction Do ()
    {
      Collection.Insert(Index, Item);
      return new ItemAddedAction<T>(Collection, Item, Index);
    }
  }
}