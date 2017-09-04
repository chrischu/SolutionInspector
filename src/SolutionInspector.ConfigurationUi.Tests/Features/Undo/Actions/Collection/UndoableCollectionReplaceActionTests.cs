using System;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.ConfigurationUi.Features.Undo.Actions.Collection;

namespace SolutionInspector.ConfigurationUi.Tests.Features.Undo.Actions.Collection
{
  internal class UndoableCollectionReplaceActionTests : UndoableCollectionActionTestBase<string>
  {
    [Test]
    public void Do ()
    {
      Collection.Add("A");
      Collection.Add("B");
      Collection.Add("C");

      // ACT & ASSERT
      Do(
        f => f.Collection(Collection).Replace(1, "b"),
        done: () => Collection.Should().Equal("A", "b", "C"),
        undone: () => Collection.Should().Equal("A", "B", "C"));
    }

    [Test]
    public void Done ()
    {
      Collection.Add("A");
      Collection.Add("b");
      Collection.Add("C");

      // ACT
      Done(
        f => f.Collection(Collection).Replaced(1, "B"),
        done: () => Collection.Should().Equal("A", "b", "C"),
        undone: () => Collection.Should().Equal("A", "B", "C"));
    }
  }
}