using System;
using System.Configuration;
using System.IO;
using FakeItEasy;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.Api.Configuration;
using SolutionInspector.Api.Configuration.RuleAssemblyImports;
using SolutionInspector.Api.Configuration.Rules;
using SolutionInspector.TestInfrastructure.AssertionExtensions;
using SolutionInspector.Utilities;
using Wrapperator.Interfaces.Configuration;
using Wrapperator.Interfaces.IO;

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

namespace SolutionInspector.Tests.Configuration
{
  [Subject (typeof(ConfigurationLoader))]
  class ConfigurationLoaderSpec
  {
    static IFileStatic File;
    static IConfigurationManagerStatic ConfigurationManager;

    static IConfiguration Configuration;

    static ConfigurationLoader SUT;

    Establish ctx = () =>
    {
      File = A.Fake<IFileStatic>();
      A.CallTo(() => File.Exists(A<string>._)).Returns(true);

      ConfigurationManager = A.Fake<IConfigurationManagerStatic>();

      Configuration = A.Fake<IConfiguration>();
      A.CallTo(() => ConfigurationManager.OpenExeConfiguration(A<ConfigurationUserLevel>._)).Returns(Configuration);
      A.CallTo(() => ConfigurationManager.OpenMappedExeConfiguration(A<ExeConfigurationFileMap>._, A<ConfigurationUserLevel>._))
          .Returns(Configuration);

      A.CallTo(() => Configuration.GetSectionGroup(A<string>._)).Returns(new DummyRuleset());

      SUT = new ConfigurationLoader(File, ConfigurationManager);
    };

    class when_loading_rules_config
    {
      Because of = () => SUT.LoadRulesConfig("configFile");

      It loads_configuration = () =>
          A.CallTo(
              () =>
                  ConfigurationManager.OpenMappedExeConfiguration(
                      A<ExeConfigurationFileMap>.That.Matches(m => m.ExeConfigFilename == "configFile"),
                      ConfigurationUserLevel.None)).MustHaveHappened();

      It gets_SolutionInspector_section_group = () =>
          A.CallTo(() => Configuration.GetSectionGroup("solutionInspector")).MustHaveHappened();
    }

    class when_loading_rules_config_file_and_file_cannot_be_found
    {
      Establish ctx = () => { A.CallTo(() => File.Exists(A<string>._)).Returns(false); };

      Because of =
          () => Exception = Catch.Exception(() => SUT.LoadRulesConfig("configFile"));

      It throws = () =>
          Exception.Should().Be<FileNotFoundException>().WithMessage("Could not find configuration file 'configFile'.");

      static Exception Exception;
    }

    private class DummyRuleset : ConfigurationSectionGroup, ISolutionInspectorRuleset
    {
      public IRuleAssemblyImportsConfiguration RuleAssemblyImports { get; }
      public IRulesConfiguration Rules { get; }
    }
  }
}