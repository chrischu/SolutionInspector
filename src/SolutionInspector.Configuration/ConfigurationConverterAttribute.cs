using System;

namespace SolutionInspector.Configuration
{
  [AttributeUsage (AttributeTargets.Class)]
  public class ConfigurationConverterAttribute : Attribute
  {
    public Type ConfigurationConverterType { get; }

    public ConfigurationConverterAttribute (Type configurationConverterType)
    {
      if (!typeof(IConfigurationConverter).IsAssignableFrom(configurationConverterType))
        throw new ArgumentException($"The given type must derive from '{typeof(IConfigurationConverter)}'.", nameof(configurationConverterType));

      if (configurationConverterType.GetConstructor(new Type[0]) == null)
        throw new ArgumentException("The given type must provide a public default constructor.", nameof(configurationConverterType));


      ConfigurationConverterType = configurationConverterType;
    }
  }
}