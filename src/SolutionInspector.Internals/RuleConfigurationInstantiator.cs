using System;
using System.Xml.Linq;
using JetBrains.Annotations;
using SolutionInspector.Configuration;

namespace SolutionInspector.Internals
{
  /// <summary>
  ///   Utility to instantiate rule configurations.
  /// </summary>
  public interface IRuleConfigurationInstantiator
  {
    /// <summary>
    ///   Instantiates a rule configuration.
    /// </summary>
    [CanBeNull]
    ConfigurationElement Instantiate([CanBeNull] Type configurationType, XElement configurationElement);
  }

  internal class RuleConfigurationInstantiator : IRuleConfigurationInstantiator
  {
    [CanBeNull]
    public ConfigurationElement Instantiate([CanBeNull] Type configurationType, XElement configurationElement)
    {
      if (configurationType == null)
        return null;

      return ConfigurationElement.Load(configurationType, configurationElement);
    }
  }
}