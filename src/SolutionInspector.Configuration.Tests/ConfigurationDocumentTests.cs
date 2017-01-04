using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.TestInfrastructure;

namespace SolutionInspector.Configuration.Tests
{
  public class ConfigurationDocumentTests
  {
    private TemporaryFile _temporaryFile;

    [SetUp]
    public void SetUp ()
    {
      _temporaryFile = new TemporaryFile();
      _temporaryFile.Write(@"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration simple=""simpleValue"" />");
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
      var result = ConfigurationDocument.Load<DummyConfigurationDocument>(_temporaryFile.Path);

      // ASSERT
      result.Simple.Should().Be("simpleValue");
    }

    [Test]
    public void Save ()
    {
      var document = ConfigurationDocument.Load<DummyConfigurationDocument>(_temporaryFile.Path);
      document.Simple = "newValue";

      // ACT
      document.Save();

      // ASSERT
      _temporaryFile.Read().Should().Be(@"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration simple=""newValue"" />");
    }

    [Test]
    public void Save_WithNewPath_WritesNewFileButLeavesOldOneUntouched ()
    {
      var document = ConfigurationDocument.Load<DummyConfigurationDocument>(_temporaryFile.Path);
      document.Simple = "newValue";

      var otherTemporaryFile = new TemporaryFile();

      try
      {
        // ACT
        document.Save(otherTemporaryFile.Path);

        // ASSERT
        _temporaryFile.Read().Should().Be(@"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration simple=""simpleValue"" />");

        otherTemporaryFile.Read().Should().Be(@"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration simple=""newValue"" />");
      }
      finally
      {
        otherTemporaryFile.Dispose();
      }
    }
    
    class DummyConfigurationDocument : ConfigurationDocument
    {
      [ConfigurationValue]
      public string Simple
      {
        get { return GetConfigurationProperty<string>(); }
        set { SetConfigurationProperty(value); }
      }
    }
  }
}