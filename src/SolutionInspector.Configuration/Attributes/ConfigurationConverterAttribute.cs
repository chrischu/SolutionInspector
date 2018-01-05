using System;

namespace SolutionInspector.Configuration.Attributes
{
  /// <summary>
  ///   Associates a <see cref="IConfigurationConverter" /> with the attribute this type is applied to.
  /// </summary>
  [AttributeUsage (AttributeTargets.Class)]
  public class ConfigurationConverterAttribute : Attribute
  {
    /// <summary>
    ///   Creates a new <see cref="ConfigurationConverterAttribute" />.
    /// </summary>
    public ConfigurationConverterAttribute (Type configurationConverterType)
    {
      if (!typeof(IConfigurationConverter).IsAssignableFrom(configurationConverterType))
        throw new ArgumentException($"The given type must derive from '{typeof(IConfigurationConverter)}'.", nameof(configurationConverterType));

      if (configurationConverterType.GetConstructor(new Type[0]) == null)
        throw new ArgumentException("The given type must provide a public default constructor.", nameof(configurationConverterType));

      ConfigurationConverterType = configurationConverterType;
    }

    /// <summary>
    ///   The CLR type of the <see cref="IConfigurationConverter" />.
    /// </summary>
    public Type ConfigurationConverterType { get; }
  }
}