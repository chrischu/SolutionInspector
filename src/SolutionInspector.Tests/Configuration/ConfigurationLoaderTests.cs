using System;
using System.IO;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Api.Configuration.Ruleset;
using SolutionInspector.Configuration;
using SolutionInspector.Utilities;
using Wrapperator.Interfaces.IO;

namespace SolutionInspector.Tests.Configuration
{
  public class ConfigurationLoaderTests
  {
    private IFileStatic _file;
    private IConfigurationManager _configurationManager;

    private ConfigurationLoader _sut;

    [SetUp]
    public void SetUp ()
    {
      _file = A.Fake<IFileStatic>();
      A.CallTo(() => _file.Exists(A<string>._)).Returns(true);

      _configurationManager = A.Fake<IConfigurationManager>();

      _sut = new ConfigurationLoader(_file, _configurationManager);
    }

    [Test]
    public void LoadRulesConfig_WithExistingConfigFile_LoadsConfiguration ()
    {
      // ACT
      _sut.LoadRulesConfig("configFile");

      // ASSERT
      A.CallTo(() => _configurationManager.LoadDocument<RulesetConfigurationDocument>("configFile")).MustHaveHappened();
    }

    [Test]
    public void LoadRulesConfig_WithNonExistingConfigFile_Throws ()
    {
      A.CallTo(() => _file.Exists(A<string>._)).Returns(false);

      // ACT
      Action act = () => _sut.LoadRulesConfig("configFile");

      // ASSERT
      act.ShouldThrow<FileNotFoundException>().WithMessage("Could not find configuration file 'configFile'.");
    }
  }
}