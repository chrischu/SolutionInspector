using System;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.ConfigurationUi.Features.Undo.Actions.Collection;

namespace SolutionInspector.ConfigurationUi.Tests.Features.Undo.Actions.Collection
{
  internal class UndoableCollectionClearActionTests : UndoableCollectionActionTestBase<string>
  {
    [Test]
    public void Do ()
    {
      Collection.Add("A");
      Collection.Add("B");
      Collection.Add("C");

      // ACT & ASSERT
      Do(
        f => f.Collection(Collection).Clear(),
        done: () => Collection.Should().Equal(),
        undone: () => Collection.Should().Equal("A", "B", "C"));
    }

    [Test]
    public void Done ()
    {
      // ACT & ASSERT
      Done(
        f => f.Collection(Collection).Cleared(new[] { "A", "B", "C" }),
        done: () => Collection.Should().Equal(),
        undone: () => Collection.Should().Equal("A", "B", "C"));
    }
  }
}