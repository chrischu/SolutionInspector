using System;
using System.Collections.Generic;
using System.Linq;
using SolutionInspector.ConfigurationUi.Infrastructure.AdvancedObservableCollections;

namespace SolutionInspector.ConfigurationUi.Infrastructure
{
  internal static class MirroringObservableCollection
  {
    public static MirroringObservableCollection<TObservableElement, TMirrorElement> Create<TObservableElement, TMirrorElement> (
      IList<TMirrorElement> mirroredCollection,
      Func<TObservableElement, TMirrorElement> convertObservableToMirrorElement,
      Func<TMirrorElement, TObservableElement> convertMirrorToObservableElement)
    {
      return new MirroringObservableCollection<TObservableElement, TMirrorElement>(
        mirroredCollection,
        convertObservableToMirrorElement,
        convertMirrorToObservableElement);
    }

    public static MirroringObservableCollection<TElement, TElement> Create<TElement> (IList<TElement> mirroredCollection)
    {
      return new MirroringObservableCollection<TElement, TElement>(mirroredCollection, x => x, x => x);
    }
  }

  internal class MirroringObservableCollection<TObservableElement, TMirrorElement> : AdvancedObservableCollection<TObservableElement>
  {
    public MirroringObservableCollection (
      IList<TMirrorElement> mirrorCollection,
      Func<TObservableElement, TMirrorElement> convertObservableToMirrorElement,
      Func<TMirrorElement, TObservableElement> convertMirrorToObservableElement)
      : base(
        mirrorCollection.Select(convertMirrorToObservableElement),
        new MirroringAdvancedObservableCollectionChangeHandler<TObservableElement, TMirrorElement>(
          mirrorCollection,
          convertObservableToMirrorElement,
          convertMirrorToObservableElement)
      )
    {
    }
  }
}