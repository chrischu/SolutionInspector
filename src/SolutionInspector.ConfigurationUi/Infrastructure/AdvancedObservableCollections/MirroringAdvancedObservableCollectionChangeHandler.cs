using System;
using System.Collections.Generic;
using System.Linq;
using SolutionInspector.Commons.Extensions;

namespace SolutionInspector.ConfigurationUi.Infrastructure.AdvancedObservableCollections
{
  internal class MirroringAdvancedObservableCollectionChangeHandler<TObservableElement, TMirrorElement>
      : AdvancedObservableCollectionChangeHandlerBase<TObservableElement>
  {
    private readonly IList<TMirrorElement> _mirrorCollection;
    private readonly Func<TObservableElement, TMirrorElement> _convertObservableToMirrorElement;
    private readonly Func<TMirrorElement, TObservableElement> _convertMirrorToObservableElement;

    public MirroringAdvancedObservableCollectionChangeHandler (
      IList<TMirrorElement> mirrorCollection,
      Func<TObservableElement, TMirrorElement> convertObservableToMirrorElement,
      Func<TMirrorElement, TObservableElement> convertMirrorToObservableElement)
    {
      _mirrorCollection = mirrorCollection;
      _convertObservableToMirrorElement = convertObservableToMirrorElement;
      _convertMirrorToObservableElement = convertMirrorToObservableElement;
    }

    public override void Initialize (AdvancedObservableCollection<TObservableElement> observableCollection)
    {
      observableCollection.Clear();
      observableCollection.AddRange(_mirrorCollection.Select(_convertMirrorToObservableElement));
    }

    public override void ElementAdded (TObservableElement element, int index)
    {
      _mirrorCollection.Insert(index, _convertObservableToMirrorElement(element));
    }

    public override void ElementRemoved (TObservableElement element, int index)
    {
      _mirrorCollection.RemoveAt(index);
    }

    public override void ElementReplaced (TObservableElement newElement, TObservableElement oldElement, int index)
    {
      _mirrorCollection[index] = _convertObservableToMirrorElement(newElement);
    }

    public override void ElementsCleared (IReadOnlyCollection<TObservableElement> removedElements)
    {
      _mirrorCollection.Clear();
    }
  }
}