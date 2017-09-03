using System;
using JetBrains.Annotations;

namespace SolutionInspector.Configuration
{
  /// <summary>
  ///   Represents a complex configuration value that can be serialized to a <see cref="string" />.
  /// </summary>
  public interface IConfigurationValue
  {
    /// <summary>
    ///   Serialize the <see cref="IConfigurationValue" /> to a <see cref="string" />.
    /// </summary>
    string Serialize ();

    /// <summary>
    ///   Deserialize the <see cref="IConfigurationValue" /> from a <see cref="string" />.
    /// </summary>
    void Deserialize (string serialized);
  }

  /// <summary>
  ///   Base class for complex configuration values that can be serialized to a <see cref="string" />.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public abstract class ConfigurationValue<T> : IConfigurationValue
    where T : ConfigurationValue<T>
  {
    private readonly Action<string> _updateValue;

    /// <summary>
    ///   Constructs a new <see cref="ConfigurationValue{T}" />. The given <paramref name="updateValue" /> action is triggered whenever the
    ///   <see cref="Update" /> method is called.
    /// </summary>
    protected ConfigurationValue (Action<string> updateValue)
    {
      _updateValue = updateValue;
    }

    /// <inheritdoc />
    public abstract string Serialize ();

    /// <inheritdoc />
    [UsedImplicitly /* via Reflection */]
    public abstract void Deserialize (string serialized);

    /// <summary>
    ///   Call this method when the configuration value has changed and the changes should get forwarded to the XML representation.
    /// </summary>
    protected void Update ()
    {
      _updateValue(Serialize());
    }

    /// <inheritdoc />
    public sealed override string ToString ()
    {
      return Serialize();
    }
  }
}