using System;
using System.Reflection;
using JetBrains.Annotations;

namespace SolutionInspector.Configuration
{
  public class ConfigurationSubelementAttribute : ConfigurationPropertyAttribute
  {
    [CanBeNull]
    public string ElementName { get; set; }

    internal override string XmlName => ElementName;

    public Type GetSubelementType(PropertyInfo property)
    {
      return property.PropertyType;
    }
  }
}