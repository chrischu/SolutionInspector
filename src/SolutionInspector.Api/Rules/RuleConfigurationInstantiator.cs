using System;
using System.Configuration;
using System.Xml;
using Fasterflect;
using JetBrains.Annotations;
using SolutionInspector.Api.Extensions;

namespace SolutionInspector.Api.Rules
{
  internal interface IRuleConfigurationInstantiator
  {
    ConfigurationElement Instantiate([CanBeNull] Type configurationType, XmlElement configurationXml);
  }

  internal class RuleConfigurationInstantiator : IRuleConfigurationInstantiator
  {
    public ConfigurationElement Instantiate([CanBeNull] Type configurationType, XmlElement configurationXml)
    {
      if (configurationType == null)
        return null;

      var configuration = (ConfigurationElement) Activator.CreateInstance(configurationType);

      using (var reader = configurationXml.Read())
        configuration.CallMethod("DeserializeElement", reader, false);

      return configuration;
    }
  }
}