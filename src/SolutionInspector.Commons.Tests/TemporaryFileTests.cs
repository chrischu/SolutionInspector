using System;
using System.IO;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.TestInfrastructure;

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
      var content = Some.ByteArray;

      // ACT
      using (var stream = _sut.GetStream())
        stream.Write(content, 0, content.Length);

      // ASSERT
      File.ReadAllBytes(_sut.Path).Should().Equal(content);
    }

    [Test]
    public void Write ()
    {
      var content = Some.String;

      // ACT
      _sut.Write(content);

      // ASSERT
      File.ReadAllText(_sut.Path).Should().Be(content);
    }

    [Test]
    public void Read ()
    {
      var content = Some.String;
      _sut.Write(content);

      // ACT
      var result = _sut.Read();

      // ASSERT
      result.Should().Be(content);
    }

    [Test]
    public void Dispose ()
    {
      File.WriteAllText(_sut.Path, "");

      // ACT
      _sut.Dispose();

      // ASSERT
      File.Exists(_sut.Path).Should().BeFalse();
    }
  }
}