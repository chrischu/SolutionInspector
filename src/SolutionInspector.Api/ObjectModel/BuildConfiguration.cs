using System;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace SolutionInspector.Api.ObjectModel
{
  /// <summary>
  /// Represents a MSBuild build configuration consisting of the configuration (e.g. Debug) and the platform (e.g. AnyCPU).
  /// </summary>
  [PublicAPI]
  public class BuildConfiguration : IEquatable<BuildConfiguration>
  {
    private static Regex s_regex = new Regex(@"[A-Za-z0-9 *]+\|[A-Za-z0-9 *]+", RegexOptions.Compiled);

    /// <summary>
    /// The name of the configuration (e.g. Debug).
    /// </summary>
    public string ConfigurationName { get; }

    /// <summary>
    /// The name of the platform (e.g. AnyCPU).
    /// </summary>
    public string PlatformName { get; }

    /// <summary>
    /// The full name of the <see cref="BuildConfiguration"/>.
    /// </summary>
    public string Name => $"{ConfigurationName}|{PlatformName}";

    /// <summary>
    /// Creates a new <see cref="BuildConfiguration"/>.
    /// </summary>
    public BuildConfiguration(string configurationName, string platform)
    {
      ConfigurationName = configurationName;
      PlatformName = platform;
    }

    /// <inheritdoc />
    public override string ToString()
    {
      return Name;
    }

    /// <inheritdoc />
    public bool Equals([CanBeNull] BuildConfiguration other)
    {
      if (ReferenceEquals(null, other))
        return false;
      if (ReferenceEquals(this, other))
        return true;
      return string.Equals(ConfigurationName, other.ConfigurationName) && string.Equals(PlatformName, other.PlatformName);
    }

    /// <inheritdoc />
    public override bool Equals([CanBeNull] object obj)
    {
      if (ReferenceEquals(null, obj))
        return false;
      if (ReferenceEquals(this, obj))
        return true;
      if (obj.GetType() != GetType())
        return false;
      return Equals((BuildConfiguration) obj);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
      unchecked
      {
        return (ConfigurationName.GetHashCode() * 397) ^ PlatformName.GetHashCode();
      }
    }

    /// <summary>
    /// Converts the string representation of a <see cref="BuildConfiguration"/> to its <see cref="BuildConfiguration"/> equivalent.
    /// </summary>
    public static BuildConfiguration Parse(string s)
    {
      if (!s_regex.IsMatch(s))
        throw new ArgumentException($"The value '{s}' is not a valid string representation of a {nameof(BuildConfiguration)}");

      var split = s.Split('|');
      return new BuildConfiguration(split[0], split[1]);
    }
  }
}