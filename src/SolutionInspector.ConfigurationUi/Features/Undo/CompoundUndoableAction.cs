using System;
using System.Collections.Generic;
using System.Linq;
using SolutionInspector.Commons.Extensions;

namespace SolutionInspector.ConfigurationUi.Features.Undo
{
  internal class CompoundUndoableAction : IUndoableAction
  {
    private readonly IReadOnlyCollection<IUndoableAction> _actions;

    public CompoundUndoableAction (IEnumerable<IUndoableAction> actions)
    {
      _actions = actions.ToList();
    }

    public void Redo ()
    {
      _actions.ForEach(a => a.Redo());
    }

    public void Undo ()
    {
      _actions.Reverse().ForEach(a => a.Undo());
    }
  }
}