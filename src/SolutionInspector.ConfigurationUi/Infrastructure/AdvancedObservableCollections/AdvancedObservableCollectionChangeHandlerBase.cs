
using System.Collections.Generic;

namespace SolutionInspector.ConfigurationUi.Infrastructure.AdvancedObservableCollections
{
  internal abstract class AdvancedObservableCollectionChangeHandlerBase<T> : IAdvancedObservableCollectionChangeHandler<T>
  {
    public virtual void Initialize (AdvancedObservableCollection<T> observableCollection)
    {
    }

    public virtual void ElementAdded (T element, int index)
    {
    }

    public virtual void ElementRemoved (T element, int index)
    {
    }

    public virtual void ElementReplaced (T newElement, T oldElement, int index)
    {
    }

    public virtual void ElementsCleared (IReadOnlyCollection<T> clearedElements)
    {
    }
  }
}