using System.Collections.Generic;

namespace SolutionInspector.ConfigurationUi.Features.Undo.Actions.Collection.Replace
{
  internal class ReplaceItemAction<T> : IndexedCollectionActionBase<T>, IDoableAction
  {
    public ReplaceItemAction (IList<T> collection, T item, int index)
      : base(collection, item, index)
    {
    }

    public IUndoableAction Do ()
    {
      var oldItem = Collection[Index];
      Collection[Index] = Item;
      return new ItemReplacedAction<T>(Collection, Item, Index, oldItem);
    }
  }
}