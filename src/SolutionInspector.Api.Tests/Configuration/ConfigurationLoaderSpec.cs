using System;
using System.Configuration;
using System.IO;
using SystemInterface.Configuration;
using SystemInterface.IO;
using FakeItEasy;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.Api.Configuration;
using SolutionInspector.TestInfrastructure.AssertionExtensions;

#region R# preamble for Machine.Specifications files

// ReSharper disable ArrangeTypeModifiers
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable ClassNeverInstantiated.Local
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Local
// ReSharper disable NotAccessedField.Local
// ReSharper disable StaticMemberInGenericType
// ReSharper disable UnassignedField.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable UnassignedGetOnlyAutoProperty

#endregion

namespace SolutionInspector.Api.Tests.Configuration
{
  [Subject (typeof(ConfigurationLoader))]
  class ConfigurationLoaderSpec
  {
    static IFile File;
    static IConfigurationManager ConfigurationManager;

    static IConfiguration Configuration;
    static SolutionInspectorConfiguration SolutionInspectorConfiguration;

    static ConfigurationLoader SUT;

    Establish ctx = () =>
    {
      File = A.Fake<IFile>();
      A.CallTo(() => File.Exists(A<string>._)).Returns(true);

      ConfigurationManager = A.Fake<IConfigurationManager>();

      Configuration = A.Fake<IConfiguration>();
      A.CallTo(() => ConfigurationManager.OpenExeConfiguration(A<ConfigurationUserLevel>._)).Returns(Configuration);
      A.CallTo(() => ConfigurationManager.OpenMappedExeConfiguration(A<ExeConfigurationFileMap>._, A<ConfigurationUserLevel>._))
          .Returns(Configuration);

      SolutionInspectorConfiguration = new SolutionInspectorConfiguration();
      A.CallTo(() => Configuration.GetSectionGroup(A<string>._)).Returns(SolutionInspectorConfiguration);

      SUT = new ConfigurationLoader(File, ConfigurationManager);
    };

    class when_loading_with_AppConfig
    {
      Because of = () => SUT.LoadAppConfigFile();

      It loads_configuration = () =>
          A.CallTo(() => ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None)).MustHaveHappened();

      It gets_SolutionInspector_section_group = () =>
          A.CallTo(() => Configuration.GetSectionGroup("solutionInspector")).MustHaveHappened();
    }

    class when_loading_with_config_file
    {
      Because of = () => SUT.Load("configFile");

      It loads_configuration = () =>
          A.CallTo(
              () =>
                  ConfigurationManager.OpenMappedExeConfiguration(
                      A<ExeConfigurationFileMap>.That.Matches(m => m.ExeConfigFilename == "configFile"),
                      ConfigurationUserLevel.None)).MustHaveHappened();

      It gets_SolutionInspector_section_group = () =>
          A.CallTo(() => Configuration.GetSectionGroup("solutionInspector")).MustHaveHappened();
    }

    class when_loading_with_config_file_and_file_cannot_be_found
    {
      Establish ctx = () => { A.CallTo(() => File.Exists(A<string>._)).Returns(false); };

      Because of =
          () => Exception = Catch.Exception(() => SUT.Load("configFile"));

      It throws = () =>
          Exception.Should().Be<FileNotFoundException>().WithMessage("Could not find configuration file 'configFile'.");

      static Exception Exception;
    }
  }
}