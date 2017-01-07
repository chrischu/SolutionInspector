using System.IO;
using SolutionInspector.Api.Configuration.Ruleset;
using SolutionInspector.Configuration;
using Wrapperator.Interfaces.IO;

namespace SolutionInspector.Utilities
{
  internal interface IConfigurationLoader
  {
    IRulesetConfiguration LoadRulesConfig (string configurationFile);
  }

  internal class ConfigurationLoader : IConfigurationLoader
  {
    private readonly IFileStatic _file;
    private readonly IConfigurationManager _configurationManager;

    public ConfigurationLoader (IFileStatic file, IConfigurationManager configurationManager)
    {
      _file = file;
      _configurationManager = configurationManager;
    }

    public IRulesetConfiguration LoadRulesConfig (string configurationFile)
    {
      if (!_file.Exists(configurationFile))
        throw new FileNotFoundException($"Could not find configuration file '{configurationFile}'.");

      return _configurationManager.LoadDocument<RulesetConfigurationDocument>(configurationFile);
    }
  }
}