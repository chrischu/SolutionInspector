using System;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using SolutionInspector.Api.ObjectModel;

namespace SolutionInspector.Api.Utilities
{
  /// <summary>
  ///   Configuration converter that converts between <see cref="string" /> and <see cref="BuildConfigurationFilter" />.
  /// </summary>
  public class BuildConfigurationFilterConverter : ConfigurationConverterBase
  {
    /// <summary>
    ///   Converts a <see cref="BuildConfigurationFilter" /> to a <see cref="string" />.
    /// </summary>
    public override object ConvertTo ([CanBeNull] ITypeDescriptorContext ctx, [CanBeNull] CultureInfo ci, [CanBeNull] object value, Type type)
    {
      if (value == null)
        return null;

      if (value.GetType() != typeof(BuildConfigurationFilter))
        throw new ArgumentException($"Unsupported type '{value.GetType()}', expected type '{typeof(BuildConfigurationFilter)}'.", nameof(value));

      return value.ToString();
    }

    /// <summary>
    ///   Converts a <see cref="string" /> to a <see cref="BuildConfigurationFilter" />.
    /// </summary>
    public override object ConvertFrom ([CanBeNull] ITypeDescriptorContext ctx, [CanBeNull] CultureInfo ci, [CanBeNull] object data)
    {
      if (data == null)
        return null;

      if (data.GetType() != typeof(string))
        throw new ArgumentException($"Unsupported type '{data.GetType()}', expected type '{typeof(string)}'.", nameof(data));

      var split = ((string) data).Split(',');

      return new BuildConfigurationFilter(split.Select(BuildConfiguration.Parse));
    }
  }
}