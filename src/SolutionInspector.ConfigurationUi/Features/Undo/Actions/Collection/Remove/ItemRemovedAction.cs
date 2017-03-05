using System.Collections.Generic;

namespace SolutionInspector.ConfigurationUi.Features.Undo.Actions.Collection.Remove
{
  internal class ItemRemovedAction<T> : IndexedCollectionActionBase<T>, IUndoableAction
  {
    public ItemRemovedAction (IList<T> collection, T item, int index)
      : base(collection, item, index)
    {
    }

    public IDoableAction Undo ()
    {
      Collection.Insert(Index, Item);
      return new RemoveItemAction<T>(Collection, Item, Index);
    }
  }
}