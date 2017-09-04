using System;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.ConfigurationUi.Features.Undo.Actions.Collection;

namespace SolutionInspector.ConfigurationUi.Tests.Features.Undo.Actions.Collection
{
  internal class UndoableCollectionAddActionTests : UndoableCollectionActionTestBase<string>
  {
    [Test]
    public void Do_WithoutIndex_AddsElementAtEndOfCollection ()
    {
      Collection.Add("A");

      // ACT & ASSERT
      Do(
        f => f.Collection(Collection).Add("B"),
        done: () => Collection.Should().Equal("A", "B"),
        undone: () => Collection.Should().Equal("A"));
    }

    [Test]
    public void Do_WithIndex_AddsElementAtSpecifiedIndex ()
    {
      Collection.Add("A");

      // ACT & ASSERT
      Do(
        f => f.Collection(Collection).Add("B", index: 0),
        done: () => Collection.Should().Equal("B", "A"),
        undone: () => Collection.Should().Equal("A"));
    }

    [Test]
    public void Done ()
    {
      Collection.Add("A");
      Collection.Add("B");
      Collection.Add("C");

      // ACT & ASSERT
      Done(f => f.Collection(Collection).Added("B", index: 1),
        done: () => Collection.Should().Equal("A", "B", "C"),
        undone: () => Collection.Should().Equal("A", "C"));
    }
  }
}