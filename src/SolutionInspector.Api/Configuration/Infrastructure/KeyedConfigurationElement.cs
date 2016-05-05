using System;
using System.Configuration;

namespace SolutionInspector.Api.Configuration.Infrastructure
{
  /// <summary>
  ///   A <see cref="ConfigurationElement" /> that can be identified by a <see cref="Key" /> of type <typeparamref name="TKey" />.
  /// </summary>
  public interface IKeyedConfigurationElement<out TKey>
  {
    /// <summary>
    ///   The name of the configuration property that stores the key.
    /// </summary>
    string KeyName { get; }

    /// <summary>
    ///   The key value that is used to identify this <see cref="ConfigurationElement" />.
    /// </summary>
    TKey Key { get; }
  }

  /// <inheritdoc />
  public abstract class KeyedConfigurationElement<TKey> : ConfigurationElement, IKeyedConfigurationElement<TKey>
  {
    /// <inheritdoc />
    public abstract string KeyName { get; }

    /// <inheritdoc />
    public virtual TKey Key => (TKey) this[KeyName];
  }
}