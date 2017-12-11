using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.SolutionParsing;
using SolutionInspector.TestInfrastructure;
using SolutionInspector.Utilities;

namespace SolutionInspector.Tests.Utilities
{
  [TestFixture]
  public class LineEnumeratorTests
  {
    private IEnumerator<string> _innerEnumerator;

    private LineEnumerator _sut;

    [SetUp]
    public void SetUp ()
    {
      _innerEnumerator = A.Fake<IEnumerator<string>>();

      _sut = new LineEnumerator(_innerEnumerator);
    }

    [TearDown]
    public void TearDown ()
    {
      _sut.Dispose();
    }

    [Test]
    public void Current ()
    {
      var current = Some.String;
      A.CallTo(() => _innerEnumerator.Current).Returns(current);

      // ACT
      var result = _sut.Current;

      // ASSERT
      result.Should().Be(current);
    }

    [Test]
    public void InitialState ()
    {
      // ACT & ASSERT
      _sut.LineNumber.Should().Be(0);
      _sut.ReachedEnd.Should().BeFalse();
    }

    [Test]
    public void AfterMoveNext_WhenThereAreItems ()
    {
      A.CallTo(() => _innerEnumerator.MoveNext()).Returns(true);

      // ACT
      var result = _sut.MoveNext();

      // ASSERT
      result.Should().BeTrue();
      _sut.LineNumber.Should().Be(1);
      _sut.ReachedEnd.Should().BeFalse();
    }

    [Test]
    public void AfterMoveNext_AndReset ()
    {
      A.CallTo(() => _innerEnumerator.MoveNext()).Returns(true);

      _sut.MoveNext();

      // ACT
      _sut.Reset();

      // ASSERT
      _sut.LineNumber.Should().Be(0);
      _sut.ReachedEnd.Should().BeFalse();
    }

    [Test]
    public void AfterMoveNext_WhenThereAreNoItems()
    {
      A.CallTo(() => _innerEnumerator.MoveNext()).Returns(false);

      // ACT
      var result = _sut.MoveNext();

      // ASSERT
      result.Should().BeFalse();
      _sut.LineNumber.Should().Be(1);
      _sut.ReachedEnd.Should().BeTrue();
    }

    [Test]
    public void Dispose_DisposesUnderlyingEnumerator ()
    {
      // ACT
      _sut.Dispose();

      // ASSERT
      A.CallTo(() => _innerEnumerator.Dispose()).MustHaveHappened();
    }
  }
}