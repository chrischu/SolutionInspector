using System;
using System.IO;
using FakeItEasy;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.Api.Configuration.Ruleset;
using SolutionInspector.Configuration;
using SolutionInspector.TestInfrastructure.AssertionExtensions;
using SolutionInspector.Utilities;
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
    static IConfigurationManager ConfigurationManager;

    static ConfigurationLoader SUT;

    Establish ctx = () =>
    {
      File = A.Fake<IFileStatic>();
      A.CallTo(() => File.Exists(A<string>._)).Returns(true);

      ConfigurationManager = A.Fake<IConfigurationManager>();

      SUT = new ConfigurationLoader(File, ConfigurationManager);
    };

    class when_loading_rules_config
    {
      Because of = () => SUT.LoadRulesConfig("configFile");

      It loads_configuration = () =>
            A.CallTo(() => ConfigurationManager.LoadSection<SolutionInspectorRulesetConfigurationDocument>("configFile")).MustHaveHappened();
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
  }
}