using System;
using System.Collections.Generic;

namespace SolutionInspector.ConfigurationUi.Features.Undo
{
  internal interface IPastUndoableCollectionActionFactory<T>
  {
    IList<T> Collection { get; }
  }

  internal interface IFutureUndoableCollectionActionFactory<T>
  {
    IList<T> Collection { get; }
  }

  internal interface IPastUndoableObjectActionFactory<out T>
  {
    T Object { get; }
  }

  internal interface IFutureUndoableObjectActionFactory<out T>
  {
    T Object { get; }
  }

  internal interface IPastUndoableActionFactory
  {
    IPastUndoableCollectionActionFactory<T> Collection<T>(IList<T> collection);
    IPastUndoableObjectActionFactory<T> Object<T>(T @object);
  }

  internal interface IFutureUndoableActionFactory
  {
    IFutureUndoableCollectionActionFactory<T> Collection<T> (IList<T> collection);
    IFutureUndoableObjectActionFactory<T> Object<T> (T @object);
  }
}