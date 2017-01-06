using System.IO;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.TestInfrastructure.AssertionExtensions;
using SolutionInspector.Utilities;

namespace SolutionInspector.Tests.Utilities
{
  public class TableWriterTests
  {
    static ITableWriter _sut;

    [SetUp]
    public void SetUp ()
    {
      _sut = new TableWriter();
    }

    [Test]
    public void Write ()
    {
      var textWriter = new StringWriter();
      var rows = new[]
                 {
                   new Something { A = "One", B = "Uno" },
                   new Something { A = "Two", B = "Dos" }
                 };

      // ACT
      _sut.Write(textWriter, rows, x => x.A, x => x.B);

      // ASSERT
      textWriter.ToString().Should().BeWithDiff(@"
+-----+-----+
│  A  │  B  │
+-----+-----+
│ One │ Uno │
+-----+-----+
│ Two │ Dos │
+-----+-----+
");
    }

    [Test]
    public void Write_WithExplicitHeaders ()
    {
      var textWriter = new StringWriter();
      var rows = new[]
                 {
                   new Something { A = "One" }
                 };

      // ACT
      _sut.Write(textWriter, rows, new[] { "HEADER" }, x => x.A);

      // ASSERT
      textWriter.ToString().Should().BeWithDiff(@"
+--------+
│ HEADER │
+--------+
│ One    │
+--------+
");
    }

    [Test]
    public void Write_WithTextThatHasToBeWrapped ()
    {
      _sut = new TableWriter(new TableWriterOptions { PreferredTableWidth = 4 });

      var textWriter = new StringWriter();
      var rows = new[]
                 {
                   new Something { A = "LONG LONG LONG" }
                 };

      // ACT
      _sut.Write(textWriter, rows, x => x.A);

      // ASSERT
      textWriter.ToString().Should().BeWithDiff(@"
+------+
│   A  │
+------+
│ LONG │
│ LONG │
│ LONG │
+------+
");
    }

    [Test]
    public void Write_ExtraSpaceThatHasToBeDividedAmongColumns ()
    {
      _sut = new TableWriter(new TableWriterOptions { PreferredTableWidth = 50 });

      var textWriter = new StringWriter();
      var rows = new[]
                 {
                   new Something { A = "NOT SO LONG", B = "LONG LONG LONG LONG LONG LONG LONG LONG LONG" }
                 };

      // ACT
      _sut.Write(textWriter, rows, x => x.A, x => x.B);

      // ASSERT
      textWriter.ToString().Should().BeWithDiff(@"
+-------------+--------------------------------+
│      A      │                B               │
+-------------+--------------------------------+
│ NOT SO LONG │ LONG LONG LONG LONG LONG LONG  │
│             │ LONG LONG LONG                 │
+-------------+--------------------------------+
");
    }

    [Test]
    public void Write_WithDifferentCharacters ()
    {
      _sut = new TableWriter(new TableWriterOptions { Characters = TableWriterCharacters.AdvancedAscii });

      var textWriter = new StringWriter();
      var rows = new[]
                 {
                   new Something { A = "One", B = "Uno" },
                   new Something { A = "Two", B = "Dos" }
                 };

      // ACT
      _sut.Write(textWriter, rows, x => x.A, x => x.B);

      // ASSERT
      textWriter.ToString().Should().BeWithDiff(@"
┌─────┬─────┐
│  A  │  B  │
├─────┼─────┤
│ One │ Uno │
├─────┼─────┤
│ Two │ Dos │
└─────┴─────┘
");
    }

    private class Something
    {
      public string A { get; set; }
      public string B { get; set; }
    }
  }
}