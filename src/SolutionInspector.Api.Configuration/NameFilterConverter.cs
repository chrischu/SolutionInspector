using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using SolutionInspector.Configuration;

namespace SolutionInspector.Api.Configuration
{
  /// <summary>
  ///   Configuration converter that converts between <see cref="string" /> and <see cref="NameFilter" />.
  /// </summary>
  public class NameFilterConverter : IConfigurationConverter<NameFilter>
  {
    /// <summary>
    ///   Converts a <see cref="NameFilter" /> to a <see cref="string" />.
    /// </summary>
    public string ConvertTo ([CanBeNull] NameFilter value)
    {
      return value?.ToString();
    }

    /// <summary>
    ///   Converts a <see cref="string" /> to a <see cref="NameFilter" />.
    /// </summary>
    public NameFilter ConvertFrom ([CanBeNull] string value)
    {
      if (value == null)
        return null;

      var partRegex = @"((\+?|-)[\w.*]+)";
      var regex = new Regex($"^{partRegex}(;{partRegex})*$");

      if (!regex.IsMatch(value))
        throw new FormatException($"The filter string '{value}' is not in the correct format.");

      var parts = value.Split(';');

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