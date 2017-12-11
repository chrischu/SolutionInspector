using JetBrains.Annotations;

namespace SolutionInspector.Configuration
{
  /// <summary>
  ///   Marker interface for <see cref="IConfigurationConverter{T}" />s.
  /// </summary>
  public interface IConfigurationConverter
  {
  }

  /// <summary>
  ///   A converter that can convert a given type into a <see cref="string" /> representation and back.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public interface IConfigurationConverter<T> : IConfigurationConverter
  {
    /// <summary>
    ///   Convert the <paramref name="value" /> to its <see cref="string" /> representation.
    /// </summary>
    [CanBeNull] string ConvertTo ([CanBeNull] T value);

    /// <summary>
    ///   Convert the <paramref name="value" /> back from its <see cref="string" /> representation.
    /// </summary>
    [CanBeNull] T ConvertFrom ([CanBeNull] string value);
  }
}