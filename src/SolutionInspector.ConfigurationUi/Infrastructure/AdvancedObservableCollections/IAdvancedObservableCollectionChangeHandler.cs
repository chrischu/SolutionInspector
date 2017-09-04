using System;
using System.Collections.Generic;

namespace SolutionInspector.ConfigurationUi.Infrastructure.AdvancedObservableCollections
{
  internal interface IAdvancedObservableCollectionChangeHandler<T>
  {
    // ReSharper disable once UnusedMember.Global
    void Initialize (AdvancedObservableCollection<T> observableCollection);

    void ElementAdded (T element, int index);
    void ElementRemoved (T element, int index);
    void ElementReplaced (T newElement, T oldElement, int index);
    void ElementsCleared (IReadOnlyCollection<T> clearedElements);
  }
}