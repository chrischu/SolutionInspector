using System;
using System.Configuration;
using System.IO;
using SolutionInspector.Api.Configuration;
using Wrapperator.Interfaces.Configuration;
using Wrapperator.Interfaces.IO;

namespace SolutionInspector.Utilities
{
  internal interface IConfigurationLoader
  {
    ISolutionInspectorRuleset LoadRulesConfig (string configurationFile);
  }

  internal class ConfigurationLoader : IConfigurationLoader
  {
    private readonly IFileStatic _file;
    private readonly IConfigurationManagerStatic _configurationManager;

    public ConfigurationLoader (IFileStatic file, IConfigurationManagerStatic configurationManager)
    {
      _file = file;
      _configurationManager = configurationManager;
    }

    public ISolutionInspectorRuleset LoadRulesConfig (string configurationFile)
    {
      var configurationSectionGroup = LoadConfigurationFromExternalConfigFile(configurationFile).GetSectionGroup("solutionInspector");
      return (ISolutionInspectorRuleset)configurationSectionGroup;
    }

    private IConfiguration LoadConfigurationFromExternalConfigFile (string configurationFile)
    {
      if (!_file.Exists(configurationFile))
        throw new FileNotFoundException($"Could not find configuration file '{configurationFile}'.");

      var map = new ExeConfigurationFileMap { ExeConfigFilename = configurationFile };
      return _configurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
    }
  }
}