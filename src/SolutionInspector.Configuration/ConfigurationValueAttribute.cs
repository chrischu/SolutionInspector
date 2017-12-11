using System;
using JetBrains.Annotations;

namespace SolutionInspector.Configuration
{
  /// <summary>
  ///   Marks a property as a configuration value.
  /// </summary>
  public class ConfigurationValueAttribute : ConfigurationPropertyAttribute
  {
    /// <summary>
    ///   The name of the XML attribute representing the value.
    /// </summary>
    [CanBeNull]
    public string AttributeName { get; set; }

    /// <summary>
    ///   If set, the given <see cref="ConfigurationConverter" /> is used for converting the value.
    /// </summary>
    [CanBeNull]
    public Type ConfigurationConverter { get; set; }

    /// <summary>
    ///   If set, this value is used when the value is accessed for the first time and it is not contained in the XML.
    /// </summary>
    [CanBeNull]
    public string DefaultValue { get; set; }

    [CanBeNull]
    internal override string XmlName => AttributeName;
  }
}