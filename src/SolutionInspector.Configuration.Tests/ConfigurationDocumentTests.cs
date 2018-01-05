﻿using System.Xml.Linq;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Commons;
using SolutionInspector.Configuration.Attributes;
using SolutionInspector.TestInfrastructure.AssertionExtensions;

namespace SolutionInspector.Configuration.Tests
{
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
  public class ConfigurationDocumentTests
  {
    private TemporaryFile _temporaryFile;
    private XDocument _xDocument;

    [SetUp]
    public void SetUp ()
    {
      _xDocument = XDocument.Parse(@"<configuration simple=""simpleValue"" />");

      _temporaryFile = new TemporaryFile();
    }

    [TearDown]
    public void TearDown ()
    {
      _temporaryFile.Dispose();
    }

    [Test]
    public void Load ()
    {
      // ACT
      var result = ConfigurationDocument.Load<DummyConfigurationDocument>(_temporaryFile.Path, _xDocument);

      // ASSERT
      result.Simple.Should().Be("simpleValue");
    }

    [Test]
    public void Save ()
    {
      var document = ConfigurationDocument.Load<DummyConfigurationDocument>(_temporaryFile.Path, _xDocument);
      document.Simple = "newValue";

      // ACT
      document.Save();

      // ASSERT
      _temporaryFile.Read().Should().BeIgnoringDifferentLineEnds(@"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration simple=""newValue"" />");
    }

    [Test]
    public void Save_WithNewPath_WritesNewFileButLeavesOldOneUntouched ()
    {
      var document = ConfigurationDocument.Load<DummyConfigurationDocument>(_temporaryFile.Path, _xDocument);
      document.Simple = "newValue";

      using (var otherTemporaryFile = new TemporaryFile())
      {
        // ACT
        document.Save(otherTemporaryFile.Path);

        // ASSERT
        _temporaryFile.Read().Should().BeEmpty();

        otherTemporaryFile.Read().Should().BeIgnoringDifferentLineEnds(@"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration simple=""newValue"" />");
      }
    }

    private class DummyConfigurationDocument : ConfigurationDocument
    {
      [ConfigurationValue]
      public string Simple
      {
        get => GetConfigurationValue<string>();
        set => SetConfigurationValue(value);
      }
    }
  }
}