using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using NUnit.Framework;
using SolutionInspector.ConfigurationUi.Infrastructure.AdvancedObservableCollections;

namespace SolutionInspector.ConfigurationUi.Tests.Infrastructure
{
  public class AdvancedObservableCollectionTests
  {
    private IAdvancedObservableCollectionChangeHandler<string> _changeHandler;

    private AdvancedObservableCollection<string> _sut;

    [SetUp]
    public void SetUp ()
    {
      _changeHandler = A.Fake<IAdvancedObservableCollectionChangeHandler<string>>();

      _sut = new AdvancedObservableCollection<string>(_changeHandler);
    }

    [Test]
    public void Add_CallsElementAdded ()
    {
      // ACT
      _sut.Add("A");

      // ASSERT
      A.CallTo(() => _changeHandler.ElementAdded("A", 0)).MustHaveHappened(Repeated.Exactly.Once);
    }

    [Test]
    public void Insert_CallsElementAdded ()
    {
      _sut.Add("A");
      _sut.Add("B");

      // ACT
      _sut.Insert(1, "C");

      // ASSERT
      A.CallTo(() => _changeHandler.ElementAdded("C", 1)).MustHaveHappened(Repeated.Exactly.Once);
    }

    [Test]
    public void Remove_CallsElementRemoved ()
    {
      _sut.Add("A");
      _sut.Add("B");

      // ACT
      _sut.Remove("B");

      // ASSERT
      A.CallTo(() => _changeHandler.ElementRemoved("B", 1)).MustHaveHappened(Repeated.Exactly.Once);
    }

    [Test]
    public void RemoveAt_CallsElementRemoved()
    {
      _sut.Add("A");
      _sut.Add("B");

      // ACT
      _sut.RemoveAt(1);

      // ASSERT
      A.CallTo(() => _changeHandler.ElementRemoved("B", 1)).MustHaveHappened(Repeated.Exactly.Once);
    }

    [Test]
    public void AssignAtIndex_CallsElementChanged ()
    {
      _sut.Add("A");
      _sut.Add("B");

      // ACT
      _sut[1] = "C";

      // ASSERT
      A.CallTo(() => _changeHandler.ElementReplaced("C", "B", 1)).MustHaveHappened(Repeated.Exactly.Once);
    }

    [Test]
    public void Clear_CallsElementsCleared ()
    {
      _sut.Add("A");
      _sut.Add("B");

      // ACT
      _sut.Clear();

      // ASSERT
      A.CallTo(
        () => _changeHandler.ElementsCleared(
          A<IReadOnlyCollection<string>>.That.Matches(
            c => c.Count == 2 && c.First() == "A" && c.Last() == "B"))).MustHaveHappened(Repeated.Exactly.Once);
    }

    [Test]
    public void Move_CallsElementChanged ()
    {
      _sut.Add("A");
      _sut.Add("B");
      _sut.Add("C");

      // ACT
      _sut.Move(1, 2);

      // ASSERT
      A.CallTo(() => _changeHandler.ElementReplaced("B", "C", 2)).MustHaveHappened(Repeated.Exactly.Once);
      A.CallTo(() => _changeHandler.ElementReplaced("C", "B", 1)).MustHaveHappened(Repeated.Exactly.Once);
    }
  }
}