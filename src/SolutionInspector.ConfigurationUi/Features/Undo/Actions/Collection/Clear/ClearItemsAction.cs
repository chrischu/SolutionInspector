using System.Collections.Generic;
using System.Linq;

namespace SolutionInspector.ConfigurationUi.Features.Undo.Actions.Collection.Clear
{
  internal class ClearItemsAction<T> : CollectionActionBase<T>, IDoableAction
  {
    public ClearItemsAction (IList<T> collection)
      : base(collection)
    {
    }

    public IUndoableAction Do ()
    {
      var items = Collection.ToList();
      Collection.Clear();
      return new ItemsClearedAction<T>(Collection, items);
    }
  }
}