using System.Collections.Generic;
using SolutionInspector.Commons.Extensions;

namespace SolutionInspector.ConfigurationUi.Features.Undo.Actions.Collection.Clear
{
  internal class ItemsClearedAction<T> : CollectionActionBase<T>, IUndoableAction
  {
    private readonly IReadOnlyCollection<T> _removedItems;

    public ItemsClearedAction (IList<T> collection, IReadOnlyCollection<T> removedItems)
      : base(collection)
    {
      _removedItems = removedItems;
    }

    public IDoableAction Undo ()
    {
      Collection.AddRange(_removedItems);
      return new ClearItemsAction<T>(Collection);
    }
  }
}