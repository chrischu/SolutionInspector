using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.TestInfrastructure;

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

namespace SolutionInspector.Configuration.Tests
{
  [Subject (typeof(ConfigurationDocument))]
  class ConfigurationDocumentSpec
  {
    static TemporaryFile TemporaryFile;

    Establish ctx = () =>
    {
      TemporaryFile = new TemporaryFile();
      TemporaryFile.Write(@"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration simple=""simpleValue"" />");
    };

    Cleanup stuff = () => { TemporaryFile.Dispose(); };

    class when_loading
    {
      Because of = () => Result = ConfigurationDocument.Load<DummyConfigurationDocument>(TemporaryFile.Path);

      It loads_file = () =>
            Result.Simple.Should().Be("simpleValue");

      static DummyConfigurationDocument Result;
    }

    class when_saving
    {
      Establish ctx = () =>
      {
        Document = ConfigurationDocument.Load<DummyConfigurationDocument>(TemporaryFile.Path);
        Document.Simple = "newValue";
      };

      Because of = () => Document.Save();

      It updates_file = () =>
            TemporaryFile.Read().Should().Be(@"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration simple=""newValue"" />");

      static DummyConfigurationDocument Document;
    }

    class when_saving_with_different_path
    {
      Establish ctx = () =>
      {
        Document = ConfigurationDocument.Load<DummyConfigurationDocument>(TemporaryFile.Path);
        Document.Simple = "newValue";

        OtherTemporaryFile = new TemporaryFile();
      };

      Cleanup stuff = () =>
      {
        OtherTemporaryFile.Dispose();
      };

      Because of = () => Document.Save(OtherTemporaryFile.Path);

      It does_not_update_original_file = () =>
            TemporaryFile.Read().Should().Be(@"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration simple=""simpleValue"" />");

      It writes_new_file = () =>
            OtherTemporaryFile.Read().Should().Be(@"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration simple=""newValue"" />");

      static TemporaryFile OtherTemporaryFile;
      static DummyConfigurationDocument Document;
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