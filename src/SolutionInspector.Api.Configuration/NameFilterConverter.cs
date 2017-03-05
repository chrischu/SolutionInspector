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

      return NameFilter.Parse(value);
    }
  }
}