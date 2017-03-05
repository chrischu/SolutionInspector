using System.Collections.Generic;

namespace SolutionInspector.ConfigurationUi.Features.Undo.Actions.Collection
{
  internal abstract class IndexedCollectionActionBase<T>
      : CollectionActionBase<T>
  {
    public T Item { get; }
    public int Index { get; }

    protected IndexedCollectionActionBase(IList<T> collection, T item, int index) : base(collection)
    {
      Item = item;
      Index = index;
    }
  }
}