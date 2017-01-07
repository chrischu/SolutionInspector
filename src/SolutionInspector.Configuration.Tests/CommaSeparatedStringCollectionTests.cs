using System;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;

namespace SolutionInspector.Configuration.Tests
{
  public class CommaSeparatedStringCollectionTests
  {
    private CommaSeparatedStringCollection _sut;
    private Action<string> _updateAction;

    [SetUp]
    public void SetUp ()
    {
      _updateAction = A.Fake<Action<string>>();

      _sut = new CommaSeparatedStringCollection(_updateAction);
    }

    [Test]
    public void Count ()
    {
      // ACT
      _sut = new CommaSeparatedStringCollection(_updateAction);

      // ASSERT
      _sut.Count.Should().Be(0);

      // ACT
      _sut.Add("X");

      // ASSERT
      _sut.Count.Should().Be(1);

      // ACT 
      _sut.Remove("X");

      // ASSERT
      _sut.Count.Should().Be(0);
    }

    [Test]
    public void Ctor_WithElements ()
    {
      // ACT
      _sut = new CommaSeparatedStringCollection(_updateAction, new[] { "A", "B" });

      // ASSERT
      _sut.Count.Should().Be(2);
      _sut.Contains("A").Should().BeTrue();
      _sut.Contains("B").Should().BeTrue();
      _sut.Contains("C").Should().BeFalse();

      A.CallTo(() => _updateAction("A,B")).MustHaveHappened(Repeated.Exactly.Once);
    }

    [Test]
    public void Serialize ()
    {
      _sut = new CommaSeparatedStringCollection(_updateAction, new[] { "A", "B" });

      // ACT
      var result = _sut.Serialize();

      // ASSERT
      result.Should().Be("A,B");
    }

    [Test]
    public void Deserialize ()
    {
      _sut = new CommaSeparatedStringCollection(_updateAction, new[] { "A", "B" });

      var input = "C,D";

      // ACT
      _sut.Deserialize(input);

      // ASSERT
      _sut.Count.Should().Be(2);
      _sut.Contains("C").Should().BeTrue();
      _sut.Contains("D").Should().BeTrue();
      _sut.Contains("A").Should().BeFalse();

      A.CallTo(() => _updateAction("C,D")).MustHaveHappened(Repeated.Exactly.Once);
    }

    [Test]
    public void Deserialize_WithEmptyString_LeavesCollectionEmpty ()
    {
      // ACT
      _sut.Deserialize("");

      // ASSERT
      _sut.Count.Should().Be(0);

      A.CallTo(() => _updateAction("")).MustHaveHappened(Repeated.Exactly.Once);
    }

    [Test]
    public void GetEnumerator ()
    {
      _sut = new CommaSeparatedStringCollection(_updateAction, new[] { "A", "B" });

      // ACT
      using (var result = _sut.GetEnumerator())
      {
        // ASSERT
        result.MoveNext().Should().BeTrue();
        result.Current.Should().Be("A");
        result.MoveNext().Should().BeTrue();
        result.Current.Should().Be("B");
        result.MoveNext().Should().BeFalse();
      }
    }

    [Test]
    public void Add_Single ()
    {
      // ACT
      _sut.Add("A");

      // ASSERT
      _sut.Count.Should().Be(1);
      _sut.Serialize().Should().Be("A");

      A.CallTo(() => _updateAction("A")).MustHaveHappened(Repeated.Exactly.Once);
    }

    [Test]
    public void Add_Multiple ()
    {
      // ACT
      _sut.Add("A", "B");

      // ASSERT
      _sut.Count.Should().Be(2);
      _sut.Serialize().Should().Be("A,B");

      A.CallTo(() => _updateAction("A,B")).MustHaveHappened(Repeated.Exactly.Once);
    }

    [Test]
    public void AddRange ()
    {
      // ACT
      _sut.AddRange(new[] { "A", "B" });

      // ASSERT
      _sut.Count.Should().Be(2);
      _sut.Serialize().Should().Be("A,B");

      A.CallTo(() => _updateAction("A,B")).MustHaveHappened(Repeated.Exactly.Once);
    }

    [Test]
    public void Clear ()
    {
      _sut = new CommaSeparatedStringCollection(_updateAction, new[] { "A", "B" });

      // ACT
      _sut.Clear();

      // ASSERT
      _sut.Count.Should().Be(0);
      _sut.Serialize().Should().Be("");

      A.CallTo(() => _updateAction("")).MustHaveHappened(Repeated.Exactly.Once);
    }

    [Test]
    [TestCase ("A", true)]
    [TestCase ("C", false)]
    public void Contains (string s, bool expectedResult)
    {
      _sut = new CommaSeparatedStringCollection(_updateAction, new[] { "A", "B" });

      // ACT
      var result = _sut.Contains(s);

      // ASSERT
      result.Should().Be(expectedResult);
    }

    [Test]
    [TestCase ("A", true, 1)]
    [TestCase ("C", false, 2)]
    public void Remove (string s, bool expectedResult, int expectedCount)
    {
      _sut = new CommaSeparatedStringCollection(_updateAction, new[] { "A", "B" });

      // ACT
      var result = _sut.Remove(s);

      // ASSERT
      result.Should().Be(expectedResult);
      _sut.Count.Should().Be(expectedCount);
    }
  }
}