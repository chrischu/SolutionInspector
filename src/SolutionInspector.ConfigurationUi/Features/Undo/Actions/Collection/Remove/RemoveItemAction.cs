using System.Collections.Generic;

namespace SolutionInspector.ConfigurationUi.Features.Undo.Actions.Collection.Remove
{
  internal class RemoveItemAction<T> : IndexedCollectionActionBase<T>, IDoableAction
  {
    public RemoveItemAction (IList<T> collection, int index)
      : this(collection, collection[index], index)
    {
    }

    public RemoveItemAction (IList<T> collection, T item, int index)
      : base(collection, item, index)
    {
    }

    public IUndoableAction Do()
    {
      Collection.RemoveAt(Index);
      return new ItemRemovedAction<T>(Collection, Item, Index);
    }
  }
}