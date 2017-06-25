using System;
using System.Diagnostics;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.ConfigurationUi.Features.Undo;

namespace SolutionInspector.ConfigurationUi.Tests.Features.Undo
{
  public class UndoContextBaseTests
  {
    private IUndoContext _sut;

    [SetUp]
    public void SetUp ()
    {
      _sut = new UndoContext();
    }

    [Test]
    public void CanUndo_WithoutAnyActions_ReturnsFalse()
    {
      // ACT
      var result = _sut.CanUndo;

      // ASSERT
      result.Should().BeFalse();
    }

    [Test]
    public void CanUndo_AfterActionWasDone_ReturnsTrue()
    {
      _sut.Do(f => A.Dummy<IUndoableAction>());

      // ACT
      var result = _sut.CanUndo;

      // ASSERT
      result.Should().BeTrue();
    }

    [Test]
    public void CanRedo_WithoutAnyActions_ReturnsFalse()
    {
      // ACT
      var result = _sut.CanRedo;

      // ASSERT
      result.Should().BeFalse();
    }

    [Test]
    public void CanRedo_AfterActionWasUndone_ReturnsTrue()
    {
      _sut.Do(f => A.Dummy<IUndoableAction>());
      _sut.Undo();

      // ACT
      var result = _sut.CanRedo;

      // ASSERT
      result.Should().BeTrue();
    }

    [Test]
    public void Do_ExecuteActionAndChangesCanUndo ()
    {
      var action = A.Fake<IUndoableAction>();

      Trace.Assert(!_sut.CanUndo);

      // ACT
      _sut.Do(f => action);

      // ASSERT
      A.CallTo(() => action.Redo()).MustHaveHappened();
      _sut.CanUndo.Should().BeTrue();
    }

    [Test]
    public void Done_DoesNotExecuteActionAndChangesCanUndo()
    {
      var action = A.Fake<IUndoableAction>();

      Trace.Assert(!_sut.CanUndo);

      // ACT
      _sut.Done(f => action);

      // ASSERT
      A.CallTo(() => action.Redo()).MustNotHaveHappened();
      _sut.CanUndo.Should().BeTrue();
    }

    [Test]
    public void Undo_UnexecutesActionAndChangesCanProperties ()
    {
      var action = A.Fake<IUndoableAction>();

      _sut.Do(f => action);

      Trace.Assert(!_sut.CanRedo);
      Trace.Assert(_sut.CanUndo);

      // ACT
      _sut.Undo();

      // ASSERT
      A.CallTo(() => action.Undo()).MustHaveHappened();
      _sut.CanUndo.Should().BeFalse();
      _sut.CanRedo.Should().BeTrue();
    }

    [Test]
    public void Undo_WithoutAnyUndoableActions ()
    {
      // ACT
      Action act = () => _sut.Undo();

      // ASSERT
      act.ShouldThrow<InvalidOperationException>().WithMessage("Cannot undo, since there are no actions to be undone.");
    }

    [Test]
    public void Redo_ExecutesActionAndChangesCanProperties ()
    {
      var action = A.Fake<IUndoableAction>();

      _sut.Do(f => action);
      _sut.Undo();

      Trace.Assert(_sut.CanRedo);
      Trace.Assert(!_sut.CanUndo);

      // ACT
      _sut.Redo();

      // ASSERT
      A.CallTo(() => action.Redo()).MustHaveHappened(Repeated.Exactly.Twice);
      _sut.CanUndo.Should().BeTrue();
      _sut.CanRedo.Should().BeFalse();
    }

    [Test]
    public void Redo_WithoutAnyRedoableActions()
    {
      // ACT
      Action act = () => _sut.Redo();

      // ASSERT
      act.ShouldThrow<InvalidOperationException>().WithMessage("Cannot redo, since there are no actions to be redone.");
    }

    [Test]
    public void Reset_ClearsAllActions ()
    {
      var action = A.Fake<IUndoableAction>();
      _sut.Do(f => action);
      _sut.Do(f => action);
      _sut.Undo();

      Trace.Assert(_sut.CanRedo);
      Trace.Assert(_sut.CanUndo);

      // ACT
      _sut.Reset();

      // ASSERT
      _sut.CanUndo.Should().BeFalse();
      _sut.CanRedo.Should().BeFalse();
    }

    [Test]
    public void CombineActions ()
    {
      var action1 = A.Fake<IUndoableAction>();
      var action2 = A.Fake<IUndoableAction>();

      Trace.Assert(!_sut.CanRedo);
      Trace.Assert(!_sut.CanUndo);

      // ACT
      using (_sut.CombineActions())
      {
        _sut.Done(f => action1);
        _sut.Done(f => action2);
      }

      // ASSERT
      _sut.CanUndo.Should().BeTrue();

      // ACT
      _sut.Undo();

      // ASSERT
      _sut.CanUndo.Should().BeFalse();
      A.CallTo(() => action1.Undo()).MustHaveHappened();
      A.CallTo(() => action2.Undo()).MustHaveHappened();

      _sut.CanRedo.Should().BeTrue();

      // ACT
      _sut.Redo();

      // ASSERT
      _sut.CanRedo.Should().BeFalse();
      A.CallTo(() => action1.Redo()).MustHaveHappened();
      A.CallTo(() => action2.Redo()).MustHaveHappened();
    }

    [Test]
    public void StackModifyingActions_WhileCombiningActions_Throws ()
    {
      var stackModifyingActions = new Action[]
                         {
                           () => _sut.Undo(),
                           () => _sut.Redo(),
                           () => _sut.Reset()
                         };

      // ACT & ASSERT
      using (_sut.CombineActions())
        foreach(var action in stackModifyingActions)
          action.ShouldThrow<InvalidOperationException>().WithMessage("While combining actions is active, it is not allowed to undo/redo/reset.");
    }

    [Test]
    public void CombineActions_WhileAlreadyCombiningActions_Throws()
    {
      using (_sut.CombineActions())
      {
        // ACT
        Action act = () => _sut.CombineActions();

        // ASSERT
        act.ShouldThrow<InvalidOperationException>().WithMessage("Combining is already active.");
      }
    }

    [Test]
    public void OpenChildContext_ReturnsNewChildContext ()
    {
      _sut.Do(f => new DummyUndoableAction());
      _sut.Do(f => new DummyUndoableAction());
      _sut.Undo();

      Trace.Assert(_sut.CanUndo);
      Trace.Assert(_sut.CanRedo);

      // ACT & ASSERT
      using (var childContext = _sut.OpenChildContext())
      {
        // childContext has no undo/redo => both false
        _sut.CanUndo.Should().BeFalse();
        _sut.CanRedo.Should().BeFalse();

        childContext.Do(f => new DummyUndoableAction());
        childContext.Do(f => new DummyUndoableAction());
        childContext.Undo();

        // Both childContext and parentContext can 
        childContext.CanUndo.Should().BeTrue();
        childContext.CanRedo.Should().BeTrue();
        _sut.CanUndo.Should().BeTrue();
        _sut.CanRedo.Should().BeTrue();
      }

      _sut.CanUndo.Should().BeTrue();
      _sut.CanRedo.Should().BeTrue();
    }

    [Test]
    public void DoActions_WithActiveChildContext_Throws()
    {
      var actions = new Action[]
                    {
                      () => _sut.Do(f => new DummyUndoableAction()),
                      () => _sut.Done(f => new DummyUndoableAction())
                    };

      // ACT & ASSERT
      using (_sut.OpenChildContext())
        foreach (var action in actions)
          action.ShouldThrow<InvalidOperationException>().WithMessage("While a child undo context is active, it is not allowed to use the parent undo context.");
    }

    [Test]
    public void UndoActions_WithActiveChildContext_DoesNotThrow()
    {
      var actions = new Action[]
                    {
                      () => _sut.Undo(),
                      () => _sut.Redo(),
                      () => _sut.Reset()
                    };

      // ACT & ASSERT
      using (var childContext = _sut.OpenChildContext())
      {
        childContext.Do(f => new DummyUndoableAction());
        foreach (var action in actions)
          action.ShouldNotThrow();
      }
    }

    private class UndoContext : UndoContextBase { }

    private class DummyUndoableAction : IUndoableAction
    {
      public void Redo ()
      {
      }

      public void Undo ()
      {
      }
    }
  }
}