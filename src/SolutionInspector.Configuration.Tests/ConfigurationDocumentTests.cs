using System;
using System.Xml.Linq;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Commons;
using SolutionInspector.Configuration.Attributes;
using SolutionInspector.Configuration.Validation;
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
    public void Create ()
    {
      // ACT
      var result = ConfigurationDocument.Create<DummyConfigurationDocument>("configuration");

      // ASSERT
      result.Element.Should().NotBeNull();
    }

    [Test]
    public void Validate_WithDocumentWithInvalidRootElementName_Throws ()
    {
      // ACT
      Action act = () => ConfigurationDocument.Create<DummyConfigurationDocument>("invalid");

      // ASSERT
      act.ShouldThrow<ConfigurationValidationException>()
          .And.DocumentValidationErrors.Should().BeEquivalentTo(
              "The root element of this configuration document must be named 'configuration' but it is named 'invalid'.");
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

    [Test]
    public void Save_WhenDocumentWasCreated_AndNoPathIsGiven_Throws ()
    {
      var document = ConfigurationDocument.Create<DummyConfigurationDocument>("configuration");
      
      // ACT
      Action act = () =>document.Save();

      // ASSERT
      act.ShouldThrowArgumentException(
          "The configuration document was not loaded from a path and therefore it is necessary to specify the save path explicitly.",
          "path");
    }

    private class DummyConfigurationDocument : ConfigurationDocument
    {
      public DummyConfigurationDocument () : base("configuration")
      {
      }

      [ConfigurationValue(IsOptional = true)]
      public string Simple
      {
        get => GetConfigurationValue<string>();
        set => SetConfigurationValue(value);
      }
    }
  }
}