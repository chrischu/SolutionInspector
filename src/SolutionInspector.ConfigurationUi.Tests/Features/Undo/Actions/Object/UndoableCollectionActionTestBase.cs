using System;
using NUnit.Framework;

namespace SolutionInspector.ConfigurationUi.Tests.Features.Undo.Actions.Object
{
  internal abstract class UndoableObjectActionTestBase : UndoableActionTestBase
  {
    protected DummyObject Object { get; private set; }

    [SetUp]
    public new void SetUp ()
    {
      Object = new DummyObject();
    }

    protected class DummyObject
    {
      public string Property { get; set; }
    }
  }
}