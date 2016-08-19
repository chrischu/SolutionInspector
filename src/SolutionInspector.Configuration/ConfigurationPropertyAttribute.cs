using System;

namespace SolutionInspector.Configuration
{
  [AttributeUsage (AttributeTargets.Property)]
  public abstract class ConfigurationPropertyAttribute : Attribute
  {
    internal abstract string XmlName { get; }

    public bool IsOptional { get; set; }
    public bool IsRequired => !IsOptional;

    public string GetXmlName(string clrPropertyName)
    {
      return XmlName ?? char.ToLower(clrPropertyName[0]) + clrPropertyName.Substring(1);
    }
  }
}