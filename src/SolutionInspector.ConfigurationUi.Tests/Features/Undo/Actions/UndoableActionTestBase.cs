using System;
using NUnit.Framework;
using SolutionInspector.ConfigurationUi.Features.Undo;

namespace SolutionInspector.ConfigurationUi.Tests.Features.Undo.Actions
{
  internal abstract class UndoableActionTestBase
  {
    private IUndoContext _undoManager;

    [SetUp]
    public void SetUp ()
    {
      _undoManager = new TopLevelUndoContext();
    }

    protected void Do(Func<IFutureUndoableActionFactory, IUndoableAction> factory, Action done, Action undone)
    {
      _undoManager.Do(factory);
      done();
      _undoManager.Undo();
      undone();
    }

    protected void Done(Func<IPastUndoableActionFactory, IUndoableAction> factory, Action done, Action undone)
    {
      _undoManager.Done(factory);
      done();
      _undoManager.Undo();
      undone();
      _undoManager.Redo();
      done();
    }
  }
}