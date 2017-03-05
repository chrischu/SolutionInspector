using System.Collections.Generic;

namespace SolutionInspector.ConfigurationUi.Features.Undo.Actions.Collection.Replace
{
  internal class ItemReplacedAction<T> : IndexedCollectionActionBase<T>, IUndoableAction
  {
    private readonly T _oldItem;

    public ItemReplacedAction (IList<T> collection, T item, int index, T oldItem)
      : base(collection, item, index)
    {
      _oldItem = oldItem;
    }

    public IDoableAction Undo ()
    {
      Collection[Index] = _oldItem;
      return new ReplaceItemAction<T>(Collection, Item, Index);
    }
  }
}