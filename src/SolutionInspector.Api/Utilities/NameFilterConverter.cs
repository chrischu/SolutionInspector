using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace SolutionInspector.Api.Utilities
{
  /// <summary>
  ///   Configuration converter that converts between <see cref="string" /> and <see cref="NameFilter" />.
  /// </summary>
  public class NameFilterConverter : ConfigurationConverterBase
  {
    /// <summary>
    ///   Converts a <see cref="NameFilter" /> to a <see cref="string" />.
    /// </summary>
    public override object ConvertTo ([CanBeNull] ITypeDescriptorContext ctx, [CanBeNull] CultureInfo ci, [CanBeNull] object value, Type type)
    {
      if (value == null)
        return null;

      if (value.GetType() != typeof (NameFilterConverter))
        throw new ArgumentException($"Unsupported type '{value.GetType()}', expected type '{typeof (NameFilterConverter)}'.", nameof(value));

      return value.ToString();
    }

    /// <summary>
    ///   Converts a <see cref="string" /> to a <see cref="NameFilter" />.
    /// </summary>
    public override object ConvertFrom ([CanBeNull] ITypeDescriptorContext ctx, [CanBeNull] CultureInfo ci, [CanBeNull] object data)
    {
      if (data == null)
        return null;

      var filterString = (string) data;
      var partRegex = @"((\+?|-)[\w.*]+)";
      var regex = new Regex($@"^{partRegex}(;{partRegex})*$");

      if (!regex.IsMatch(filterString))
        throw new FormatException($"The filter string '{filterString}' is not in the correct format.");

      var parts = filterString.Split(';');

      var includes = new List<string>();
      var excludes = new List<string>();

      foreach (var part in parts)
        if (part[0] == '-')
          excludes.Add(part.TrimStart('-'));
        else
          includes.Add(part.TrimStart('+'));

      return new NameFilter(includes, excludes);
    }
  }
}