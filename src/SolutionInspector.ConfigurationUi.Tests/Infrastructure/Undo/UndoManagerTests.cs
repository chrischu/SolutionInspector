using System;
using System.Diagnostics;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.ConfigurationUi.Features.Undo;
using SolutionInspector.ConfigurationUi.Features.Undo.Actions;

namespace SolutionInspector.ConfigurationUi.Tests.Infrastructure.Undo
{
  public class UndoManagerTests
  {
    private IUndoManager _sut;

    [SetUp]
    public void SetUp ()
    {
      _sut = new UndoManager();
    }

    //[Test]
    //public void CanUndo_WithoutAnyActions_ReturnsFalse ()
    //{
    //  // ACT
    //  var result = _sut.CanUndo;

    //  // ASSERT
    //  result.Should().BeFalse();
    //}

    //[Test]
    //public void CanUndo_AfterActionWasDone_ReturnsTrue ()
    //{
    //  _sut.Do(A.Dummy<IUndoableAction>());

    //  // ACT
    //  var result = _sut.CanUndo;

    //  // ASSERT
    //  result.Should().BeTrue();
    //}

    //[Test]
    //public void CanRedo_WithoutAnyActions_ReturnsFalse()
    //{
    //  // ACT
    //  var result = _sut.CanRedo;

    //  // ASSERT
    //  result.Should().BeFalse();
    //}

    //[Test]
    //public void CanRedo_AfterActionWasUndone_ReturnsTrue()
    //{
    //  _sut.Do(A.Dummy<IUndoableAction>());
    //  _sut.Undo();

    //  // ACT
    //  var result = _sut.CanRedo;

    //  // ASSERT
    //  result.Should().BeTrue();
    //}

    [Test]
    public void Do_ExecuteActionsAndChangesCanUndo ()
    {
      var action = A.Fake<IDoableAction>();

      Trace.Assert(!_sut.CanUndo);

      // ACT
      _sut.Do(action);

      // ASSERT
      A.CallTo(() => action.Do()).MustHaveHappened();
      _sut.CanUndo.Should().BeTrue();
    }

    [Test]
    public void Undo_UnexecutesActionAndChangesCanProperties ()
    {
      var doable = A.Fake<IDoableAction>();
      var undoable = A.Fake<IUndoableAction>();
      A.CallTo(() => doable.Do()).Returns(undoable);

      _sut.Do(doable);

      Trace.Assert(!_sut.CanRedo);
      Trace.Assert(_sut.CanUndo);

      // ACT
      _sut.Undo();

      // ASSERT
      A.CallTo(() => undoable.Undo()).MustHaveHappened();
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
      var doable = A.Fake<IDoableAction>();
      var undoable = A.Fake<IUndoableAction>();
      var doable2 = A.Fake<IDoableAction>();
      A.CallTo(() => doable.Do()).Returns(undoable);
      A.CallTo(() => undoable.Undo()).Returns(doable2);

      _sut.Do(doable);
      _sut.Undo();

      Trace.Assert(_sut.CanRedo);
      Trace.Assert(!_sut.CanUndo);

      // ACT
      _sut.Redo();

      // ASSERT
      A.CallTo(() => doable2.Do()).MustHaveHappened();
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
      var action = A.Fake<IDoableAction>();
      _sut.Do(action);
      _sut.Do(action);
      _sut.Undo();

      Trace.Assert(_sut.CanRedo);
      Trace.Assert(_sut.CanUndo);

      // ACT
      _sut.Reset();

      // ASSERT
      _sut.CanUndo.Should().BeFalse();
      _sut.CanRedo.Should().BeFalse();
    }
  }
}