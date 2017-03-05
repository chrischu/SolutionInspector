using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.ConfigurationUi.Infrastructure;

namespace SolutionInspector.ConfigurationUi.Tests.Infrastructure
{
  public class MirroringObservableCollectionTests
  {
    private IList<string> _mirroredCollection;

    private MirroringObservableCollection<int, string> _sut;

    [SetUp]
    public void SetUp ()
    {
      _mirroredCollection = new List<string>();

      _sut = new MirroringObservableCollection<int, string>(_mirroredCollection, i => i.ToString(), int.Parse);
    }

    [Test]
    public void Ctor ()
    {
      var mirroredCollection = new[] { "1", "2" };

      // ACT
      var result = new MirroringObservableCollection<int, string>(mirroredCollection, i => i.ToString(), int.Parse);

      // ASSERT
      result.Should().BeEquivalentTo(new[] { 1, 2 });
    }

    [Test]
    public void Add_InsertsIntoSourceCollection ()
    {
      // ACT
      _sut.Add(1);

      // ASSERT
      _mirroredCollection.Should().BeEquivalentTo("1");
    }

    [Test]
    public void Insert_InsertsIntoSourceCollection ()
    {
      _sut.Add(1);
      _sut.Add(2);

      // ACT
      _sut.Insert(index: 1, item: 3);

      // ASSERT
      _mirroredCollection.Should().BeEquivalentTo("1", "3", "2");
    }

    [Test]
    public void Remove_RemovesElementFromSourceCollection ()
    {
      _sut.Add(1);
      _sut.Add(2);

      // ACT
      _sut.Remove(2);

      // ASSERT
      _mirroredCollection.Should().BeEquivalentTo("1");
    }

    [Test]
    public void RemoveAt_RemovesElementFromSourceCollection()
    {
      _sut.Add(1);
      _sut.Add(2);

      // ACT
      _sut.RemoveAt(1);

      // ASSERT
      _mirroredCollection.Should().BeEquivalentTo("1");
    }

    [Test]
    public void AssignAtIndex_UpdatesElementInSourceCollection ()
    {
      _sut.Add(1);
      _sut.Add(2);

      // ACT
      _sut[1] = 3;

      // ASSERT
      _mirroredCollection.Should().BeEquivalentTo("1", "3");
    }

    [Test]
    public void Clear_ClearsSourceCollection ()
    {
      _sut.Add(1);
      _sut.Add(2);

      // ACT
      _sut.Clear();

      // ASSERT
      _mirroredCollection.Should().BeEmpty();
    }

    [Test]
    public void Move_CallsElementChanged ()
    {
      _sut.Add(1);
      _sut.Add(2);
      _sut.Add(3);

      // ACT
      _sut.Move(1, 2);

      // ASSERT
      _mirroredCollection.Should().BeEquivalentTo("1", "3", "2");
    }
  }
}