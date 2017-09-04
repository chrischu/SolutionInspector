using System;

namespace SolutionInspector.ConfigurationUi.Features.Undo
{
  internal interface IUndoableAction
  {
    void Redo ();
    void Undo ();
  }
}