using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace SolutionInspector.ConfigurationUi.Tests.Features.Undo.Actions.Collection
{
  internal abstract class UndoableCollectionActionTestBase<T> : UndoableActionTestBase
  {
    protected IList<T> Collection { get; private set; }

    [SetUp]
    public new void SetUp ()
    {
      Collection = new List<T>();
    }
  }
}