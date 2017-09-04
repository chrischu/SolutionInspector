﻿using System;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.ConfigurationUi.Features.Undo.Actions.Collection;

namespace SolutionInspector.ConfigurationUi.Tests.Features.Undo.Actions.Collection
{
  internal class UndoableCollectionRemoveActionTests : UndoableCollectionActionTestBase<string>
  {
    [Test]
    public void Do ()
    {
      Collection.Add("A");
      Collection.Add("B");
      Collection.Add("C");

      // ACT & ASSERT
      Do(
        f => f.Collection(Collection).Remove(1),
        done: () => Collection.Should().Equal("A", "C"),
        undone: () => Collection.Should().Equal("A", "B", "C"));
    }

    [Test]
    public void Done()
    {
      Collection.Add("A");
      Collection.Add("C");

      // ACT & ASSERT
      Done(
        f => f.Collection(Collection).Removed(1, "B"),
        done: () => Collection.Should().Equal("A", "C"),
        undone: () => Collection.Should().Equal("A", "B", "C"));
    }
  }
}
