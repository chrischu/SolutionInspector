using System;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.ConfigurationUi.Features.Undo.Actions.Object;

namespace SolutionInspector.ConfigurationUi.Tests.Features.Undo.Actions.Object
{
  internal class UndoableObjectPropertyChangeActionTests : UndoableObjectActionTestBase
  {
    [Test]
    public void Do ()
    {
      Object.Property = "OldValue";

      // ACT & ASSERT
      Do(
        f => f.Object(Object).ChangeProperty(o => o.Property, "NewValue"),
        done: () => Object.Property.Should().Be("NewValue"),
        undone: () => Object.Property.Should().Be("OldValue"));
    }

    [Test]
    public void Done ()
    {
      Object.Property = "NewValue";

      // ACT
      Done(
        f => f.Object(Object).PropertyChanged(o => o.Property, "OldValue"),
        done: () => Object.Property.Should().Be("NewValue"),
        undone: () => Object.Property.Should().Be("OldValue"));
    }
  }
}