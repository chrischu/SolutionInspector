using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.TestInfrastructure.AssertionExtensions;
using SolutionInspector.Utilities;

#region R# preamble for Machine.Specifications files

// ReSharper disable ArrangeTypeModifiers
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable ClassNeverInstantiated.Local
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Local
// ReSharper disable NotAccessedField.Local
// ReSharper disable StaticMemberInGenericType
// ReSharper disable UnassignedField.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable UnassignedGetOnlyAutoProperty

#endregion

namespace SolutionInspector.Tests.Utilities
{
  [Subject (typeof(TableWriter))]
  class TableWriterSpec
  {
    static ITableWriter SUT;

    Establish ctx = () => { SUT = new TableWriter(); };

    class when_writing
    {
      Establish ctx = () =>
      {
        TextWriter = new StringWriter();
        Rows = new[]
               {
                   new Something { A = "One", B = "Uno" },
                   new Something { A = "Two", B = "Dos" }
               };
      };

      Because of = () => SUT.Write(TextWriter, Rows, x => x.A, x => x.B);

      It writes_table = () =>
          TextWriter.ToString().Should().BeWithDiff(@"
+-----+-----+
│  A  │  B  │
+-----+-----+
│ One │ Uno │
+-----+-----+
│ Two │ Dos │
+-----+-----+
");

      static TextWriter TextWriter;
      static IEnumerable<Something> Rows;
    }

    class when_writing_with_explicit_headers
    {
      Establish ctx = () =>
      {
        TextWriter = new StringWriter();
        Rows = new[]
               {
                   new Something { A = "One" }
               };
      };

      Because of = () => SUT.Write(TextWriter, Rows, new[] { "HEADER" }, x => x.A);

      It writes_table = () =>
          TextWriter.ToString().Should().BeWithDiff(@"
+--------+
│ HEADER │
+--------+
│ One    │
+--------+
");

      static TextWriter TextWriter;
      static IEnumerable<Something> Rows;
    }

    class when_writing_and_text_has_to_be_wrapped
    {
      Establish ctx = () =>
      {
        SUT = new TableWriter(new TableWriterOptions { PreferredTableWidth = 4 });
        TextWriter = new StringWriter();
        Rows = new[]
               {
                   new Something { A = "LONG LONG LONG" }
               };
      };

      Because of = () => SUT.Write(TextWriter, Rows, x => x.A);

      It writes_table = () =>
          TextWriter.ToString().Should().BeWithDiff(@"
+------+
│   A  │
+------+
│ LONG │
│ LONG │
│ LONG │
+------+
");

      static TextWriter TextWriter;
      static IEnumerable<Something> Rows;
    }

    class when_writing_and_extra_space_has_to_be_divided
    {
      Establish ctx = () =>
      {
        SUT = new TableWriter(new TableWriterOptions { PreferredTableWidth = 50 });
        TextWriter = new StringWriter();
        Rows = new[]
               {
                   new Something { A = "NOT SO LONG", B = "LONG LONG LONG LONG LONG LONG LONG LONG LONG" }
               };
      };

      Because of = () => SUT.Write(TextWriter, Rows, x => x.A, x => x.B);

      It writes_table = () =>
          TextWriter.ToString().Should().BeWithDiff(@"
+-------------+--------------------------------+
│      A      │                B               │
+-------------+--------------------------------+
│ NOT SO LONG │ LONG LONG LONG LONG LONG LONG  │
│             │ LONG LONG LONG                 │
+-------------+--------------------------------+
");

      static TextWriter TextWriter;
      static IEnumerable<Something> Rows;
    }

    class when_writing_with_different_characters
    {
      Establish ctx = () =>
      {
        SUT = new TableWriter(new TableWriterOptions { Characters = TableWriterCharacters.AdvancedAscii });
        TextWriter = new StringWriter();
        Rows = new[]
               {
                   new Something { A = "One", B = "Uno" },
                   new Something { A = "Two", B = "Dos" }
               };
      };

      Because of = () => SUT.Write(TextWriter, Rows, x => x.A, x => x.B);

      It writes_table = () =>
          TextWriter.ToString().Should().BeWithDiff(@"
┌─────┬─────┐
│  A  │  B  │
├─────┼─────┤
│ One │ Uno │
├─────┼─────┤
│ Two │ Dos │
└─────┴─────┘
");

      static TextWriter TextWriter;
      static IEnumerable<Something> Rows;
    }

    class Something
    {
      public string A { get; set; }
      public string B { get; set; }
    }
  }
}