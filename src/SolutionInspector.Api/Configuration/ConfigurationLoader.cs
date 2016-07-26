using System;
using System.Configuration;
using System.IO;
using Wrapperator.Interfaces.Configuration;
using Wrapperator.Interfaces.IO;

namespace SolutionInspector.Api.Configuration
{
  internal interface IConfigurationLoader
  {
    ISolutionInspectorConfiguration Load (string configurationFile);
    ISolutionInspectorConfiguration LoadAppConfigFile ();
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

    public ISolutionInspectorConfiguration Load (string configurationFile)
    {
      return Load(() => LoadConfigurationFromExternalConfigFile(configurationFile));
    }

    public ISolutionInspectorConfiguration LoadAppConfigFile ()
    {
      return Load(() => _configurationManager.OpenExeConfiguration(ConfigurationUserLevel.None));
    }

    private ISolutionInspectorConfiguration Load (Func<IConfiguration> loadConfiguration)
    {
      return (ISolutionInspectorConfiguration) loadConfiguration().GetSectionGroup("solutionInspector");
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