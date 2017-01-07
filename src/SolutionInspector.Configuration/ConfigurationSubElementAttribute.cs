using System;
using System.Reflection;
using JetBrains.Annotations;

namespace SolutionInspector.Configuration
{
  /// <summary>
  ///   Marks a property as a configuration subelement.
  /// </summary>
  public class ConfigurationSubelementAttribute : ConfigurationPropertyAttribute
  {
    /// <summary>
    ///   The name of the XML element representing the subelement.
    /// </summary>
    [CanBeNull]
    public string ElementName { get; set; }

    internal override string XmlName => ElementName;

    internal Type GetSubelementType (PropertyInfo property)
    {
      return property.PropertyType;
    }
  }
}