using System.Collections.Generic;

namespace SolutionInspector.ConfigurationUi.Features.Undo.Actions.Collection.Add
{
  internal class ItemAddedAction<T> : IndexedCollectionActionBase<T>, IUndoableAction
  {
    public ItemAddedAction(IList<T> collection, T item, int index) : base(collection, item, index)
    {
    }

    public IDoableAction Undo()
    {
      Collection.RemoveAt(Index);
      return new AddItemAction<T>(Collection, Item, Index);
    }
  }
}