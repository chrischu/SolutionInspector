using System;
using System.Configuration;
using System.IO;
using SystemInterface.Configuration;
using SystemInterface.IO;

namespace SolutionInspector.Api.Configuration
{
  internal interface IConfigurationLoader
  {
    ISolutionInspectorConfiguration Load (string configurationFile);
    ISolutionInspectorConfiguration LoadAppConfigFile ();
  }

  internal class ConfigurationLoader : IConfigurationLoader
  {
    private readonly IFile _file;
    private readonly IConfigurationManager _configurationManager;

    public ConfigurationLoader (IFile file, IConfigurationManager configurationManager)
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