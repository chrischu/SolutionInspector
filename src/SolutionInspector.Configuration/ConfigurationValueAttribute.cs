using System;
using JetBrains.Annotations;

namespace SolutionInspector.Configuration
{
  public class ConfigurationValueAttribute : ConfigurationPropertyAttribute
  {
    [CanBeNull]
    public string AttributeName { get; set; }

    [CanBeNull]
    public Type ConfigurationConverter { get; set; }

    [CanBeNull]
    public string DefaultValue { get; set; }

    internal override string XmlName => AttributeName;
  }
}