using System;
using System.IO;
using FluentAssertions;
using NUnit.Framework;

namespace SolutionInspector.Commons.Tests
{
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
  public class TemporaryFileTests
  {
    private TemporaryFile _sut;

    [SetUp]
    public void SetUp ()
    {
      _sut = new TemporaryFile();
    }

    [TearDown]
    public void TearDown ()
    {
      _sut.Dispose();
    }

    [Test]
    public void GetStream ()
    {
      // ACT
      using (var stream = _sut.GetStream())
      using (var streamWriter = new StreamWriter(stream))
        streamWriter.Write("ABC");

      // ASSERT
      File.ReadAllText(_sut.Path).Should().Be("ABC");
    }
  }
}