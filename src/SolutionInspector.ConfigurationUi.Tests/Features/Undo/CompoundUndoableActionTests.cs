using System;
using FakeItEasy;
using NUnit.Framework;
using SolutionInspector.ConfigurationUi.Features.Undo;

namespace SolutionInspector.ConfigurationUi.Tests.Features.Undo
{
  public class CompoundUndoableActionTests
  {
    private IUndoableAction _action1;
    private IUndoableAction _action2;

    private CompoundUndoableAction _sut;

    [SetUp]
    public void SetUp ()
    {
      _action1 = A.Fake<IUndoableAction>();
      _action2 = A.Fake<IUndoableAction>();

      _sut = new CompoundUndoableAction(new[] { _action1, _action2 });
    }

    [Test]
    public void Undo ()
    {
      // ACT
      _sut.Undo();

      // ASSERT
      A.CallTo(() => _action1.Undo()).MustHaveHappened(Repeated.Exactly.Once);
      A.CallTo(() => _action2.Undo()).MustHaveHappened(Repeated.Exactly.Once);
    }

    [Test]
    public void Redo ()
    {
      // ACT
      _sut.Redo();

      // ASSERT
      A.CallTo(() => _action1.Redo()).MustHaveHappened(Repeated.Exactly.Once);
      A.CallTo(() => _action2.Redo()).MustHaveHappened(Repeated.Exactly.Once);
    }
  }
}