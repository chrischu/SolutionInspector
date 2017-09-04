using System;

namespace SolutionInspector.ConfigurationUi.Features.Undo
{
  internal interface IUndoContext
  {
    bool CanUndo { get; }
    bool CanRedo { get; }

    void Do (Func<IFutureUndoableActionFactory, IUndoableAction> createUndoableAction);
    void Done (Func<IPastUndoableActionFactory, IUndoableAction> createUndoableAction);
    void Undo ();
    void Redo ();
    void Reset ();

    IDisposable CombineActions ();

    IChildUndoContext OpenChildContext ();
  }
}