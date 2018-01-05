using System;
using JetBrains.Annotations;

namespace SolutionInspector.Configuration.Attributes
{
  /// <summary>
  ///   Base class for various configuration property attributes.
  /// </summary>
  [AttributeUsage (AttributeTargets.Property)]
  public abstract class ConfigurationPropertyAttribute : Attribute
  {
    [CanBeNull]
    internal abstract string XmlName { get; }

    /// <summary>
    ///   Controls whether the property is required or not.
    /// </summary>
    public bool IsOptional { get; set; }

    /// <summary>
    ///   Returns <see langword="true" /> if the property is required, <see langword="false" /> otherwise.
    /// </summary>
    public bool IsRequired => !IsOptional;

    internal string GetXmlName (string clrPropertyName)
    {
      return XmlName ?? char.ToLower(clrPropertyName[0]) + clrPropertyName.Substring(1);
    }
  }
}