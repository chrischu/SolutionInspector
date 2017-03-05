using System.Collections.Generic;

namespace SolutionInspector.ConfigurationUi.Features.Undo.Actions.Collection
{
  internal abstract class CollectionActionBase<T>
  {
    protected IList<T> Collection { get; }

    protected CollectionActionBase (IList<T> collection)
    {
      Collection = collection;
    }
  }
}