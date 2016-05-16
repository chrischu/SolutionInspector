using System;
using System.Collections.Generic;
using SolutionInspector.Api.Extensions;

namespace SolutionInspector.Api.ObjectModel
{
  /// <summary>
  /// Provides convenience methods to access a project properties value.
  /// </summary>
  public static class ProjectPropertyExtensions
  {
    /// <summary>
    /// Tries to get the value of the property named <paramref name="propertyName"/> and returns <see langword="null" /> when it cannot be found.
    /// </summary>
    public static string GetPropertyValueOrNull (this IReadOnlyDictionary<string, IProjectProperty> properties, string propertyName)
    {
      return properties.GetValueOrDefault(propertyName)?.Value;
    }
  }
}