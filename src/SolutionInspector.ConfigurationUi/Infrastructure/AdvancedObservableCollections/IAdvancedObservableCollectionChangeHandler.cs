using System.Collections.Generic;
using JetBrains.Annotations;

namespace SolutionInspector.ConfigurationUi.Infrastructure.AdvancedObservableCollections
{
  [PublicAPI]
  internal interface IAdvancedObservableCollectionChangeHandler<T>
  {
    void Initialize (AdvancedObservableCollection<T> observableCollection);
    void ElementAdded (T element, int index);
    void ElementRemoved (T element, int index);
    void ElementReplaced (T newElement, T oldElement, int index);
    void ElementsCleared (IReadOnlyCollection<T> removedElements);
  }
}